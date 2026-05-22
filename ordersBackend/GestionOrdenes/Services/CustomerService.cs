using Dapper;
using MySqlConnector;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Customers;
using OrderManagement.Models.Entities;
using OrderManagement.Models.Enums;
using OrderManagement.Repositories.Interfaces;
using OrderManagement.Services.Interfaces;

namespace OrderManagement.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepo;
    private readonly IConfiguration _config;

    public CustomerService(ICustomerRepository customerRepo, IConfiguration config)
    {
        _customerRepo = customerRepo;
        _config       = config;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync(string? search)
    {
        var customers = await _customerRepo.GetAllAsync(search);
        return customers.Select(ToDto);
    }

    public async Task<CustomerDto> GetByIdAsync(int id)
    {
        var customer = await _customerRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Customer with id {id} not found.");
        return ToDto(customer);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        if (await _customerRepo.GetByEmailAsync(dto.Email) is not null)
            throw new DomainException("A customer with that email already exists.");

        using var conn = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));

        var userId = await conn.ExecuteScalarAsync<int>(@"
            INSERT INTO users (name, email, password_hash, role, created_at)
            VALUES (@Name, @Email, @PasswordHash, @Role, @CreatedAt);
            SELECT LAST_INSERT_ID();",
            new
            {
                dto.Name,
                dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role         = UserRole.Customer.ToString(),
                CreatedAt    = DateTime.UtcNow
            });

        var customer = new Customer
        {
            UserId    = userId,
            Name      = dto.Name,
            Email     = dto.Email,
            Phone     = dto.Phone,
            Address   = dto.Address,
            CreatedAt = DateTime.UtcNow
        };

        customer.Id = await _customerRepo.InsertAsync(customer);
        return ToDto(customer);
    }

    public async Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto)
    {
        var customer = await _customerRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Customer with id {id} not found.");

        customer.Name    = dto.Name;
        customer.Phone   = dto.Phone;
        customer.Address = dto.Address;

        await _customerRepo.UpdateAsync(customer);
        return ToDto(customer);
    }

    public async Task DeleteAsync(int id)
    {
        _ = await _customerRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Customer with id {id} not found.");

        if (await _customerRepo.HasActiveOrdersAsync(id))
            throw new DomainException("Cannot delete a customer who has active orders.");

        await _customerRepo.DeleteAsync(id);
    }

    private static CustomerDto ToDto(Customer c) => new()
    {
        Id        = c.Id,
        Name      = c.Name,
        Email     = c.Email,
        Phone     = c.Phone,
        Address   = c.Address,
        CreatedAt = c.CreatedAt
    };
}
