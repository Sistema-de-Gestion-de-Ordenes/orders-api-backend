using OrderManagement.Models.DTOs.Drivers;

namespace OrderManagement.Services.Interfaces;

public interface IDriverService
{
    Task<IEnumerable<DriverDto>> GetAllAsync(string? search);
    Task<DriverDto> GetByIdAsync(int id);
    Task<DriverDto> CreateAsync(CreateDriverDto dto);
    Task<DriverDto> UpdateAsync(int id, UpdateDriverDto dto);
    Task DeleteAsync(int id);
}
