using OrderManagement.Common;
using OrderManagement.Models.DTOs.Customers;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories.Interfaces;
using OrderManagement.Services.Interfaces;

namespace OrderManagement.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepo;
    private readonly IWebHostEnvironment _env;

    public CustomerService(ICustomerRepository customerRepo, IWebHostEnvironment env)
    {
        _customerRepo = customerRepo;
        _env          = env;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync(string? search)
    {
        var customers = await _customerRepo.GetAllAsync(search);
        return customers.Select(ToDto);
    }

    public async Task<CustomerDto> GetByIdAsync(int id)
    {
        var customer = await _customerRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"The client with id {id} does not exist.");
        return ToDto(customer);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        if (await _customerRepo.GetByEmailAsync(dto.Email) is not null)
            throw new DomainException("The email is already in use.", 409);

        var allowedTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "image/jpeg", "image/jpg", "image/png" };

        if (!allowedTypes.Contains(dto.Photo.ContentType))
            throw new DomainException("Only JPG or PNG images are allowed.");

        const long maxSize = 5 * 1024 * 1024;
        if (dto.Photo.Length > maxSize)
            throw new DomainException("Photo must not exceed 5 MB.");

        var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var uploadsFolder = Path.Combine(webRoot, "uploads", "clients");
        Directory.CreateDirectory(uploadsFolder);

        var ext      = Path.GetExtension(dto.Photo.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
            await dto.Photo.CopyToAsync(stream);

        var customer = new Customer
        {
            Name         = dto.Name,
            Email        = dto.Email,
            Phone        = dto.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            PhotoUrl     = $"/uploads/clients/{fileName}",
            CreatedAt    = DateTime.UtcNow
        };

        customer.Id = await _customerRepo.InsertAsync(customer);
        return ToDto(customer);
    }

    public async Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto)
    {
        var customer = await _customerRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"The client with id {id} does not exist.");

        customer.Name  = dto.Name;
        customer.Phone = dto.Phone;

        await _customerRepo.UpdateAsync(customer);
        return ToDto(customer);
    }

    public async Task DeleteAsync(int id)
    {
        _ = await _customerRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"The client with id {id} does not exist.");

        if (await _customerRepo.HasActiveDeliveriesAsync(id))
            throw new DomainException("Cannot delete a client who has active deliveries.");

        await _customerRepo.DeleteAsync(id);
    }

    private static CustomerDto ToDto(Customer c) => new()
    {
        Id       = c.Id,
        Name     = c.Name,
        Email    = c.Email,
        Phone    = c.Phone,
        PhotoUrl = c.PhotoUrl
    };
}
