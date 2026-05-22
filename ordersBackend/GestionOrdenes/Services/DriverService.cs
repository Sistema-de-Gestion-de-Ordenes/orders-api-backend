using Dapper;
using MySqlConnector;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Drivers;
using OrderManagement.Models.Entities;
using OrderManagement.Models.Enums;
using OrderManagement.Repositories.Interfaces;
using OrderManagement.Services.Interfaces;

namespace OrderManagement.Services;

public class DriverService : IDriverService
{
    private readonly IDriverRepository _driverRepo;
    private readonly IConfiguration _config;

    public DriverService(IDriverRepository driverRepo, IConfiguration config)
    {
        _driverRepo = driverRepo;
        _config     = config;
    }

    public async Task<IEnumerable<DriverDto>> GetAllAsync(string? search)
    {
        var drivers = await _driverRepo.GetAllAsync(search);
        return drivers.Select(ToDto);
    }

    public async Task<DriverDto> GetByIdAsync(int id)
    {
        var driver = await _driverRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Driver with id {id} not found.");
        return ToDto(driver);
    }

    public async Task<DriverDto> CreateAsync(CreateDriverDto dto)
    {
        if (await _driverRepo.GetByEmailAsync(dto.Email) is not null)
            throw new DomainException("A driver with that email already exists.");

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
                Role         = UserRole.Driver.ToString(),
                CreatedAt    = DateTime.UtcNow
            });

        var driver = new Driver
        {
            UserId    = userId,
            Name      = dto.Name,
            Email     = dto.Email,
            Phone     = dto.Phone,
            Vehicle   = dto.Vehicle,
            Available = true,
            CreatedAt = DateTime.UtcNow
        };

        driver.Id = await _driverRepo.InsertAsync(driver);
        return ToDto(driver);
    }

    public async Task<DriverDto> UpdateAsync(int id, UpdateDriverDto dto)
    {
        var driver = await _driverRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Driver with id {id} not found.");

        driver.Name    = dto.Name;
        driver.Phone   = dto.Phone;
        driver.Vehicle = dto.Vehicle;

        await _driverRepo.UpdateAsync(driver);
        return ToDto(driver);
    }

    public async Task DeleteAsync(int id)
    {
        _ = await _driverRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Driver with id {id} not found.");

        if (await _driverRepo.HasOrdersInProgressAsync(id))
            throw new DomainException("Cannot delete a driver who has orders in progress.");

        await _driverRepo.DeleteAsync(id);
    }

    private static DriverDto ToDto(Driver d) => new()
    {
        Id        = d.Id,
        Name      = d.Name,
        Email     = d.Email,
        Phone     = d.Phone,
        Vehicle   = d.Vehicle,
        Available = d.Available,
        CreatedAt = d.CreatedAt
    };
}
