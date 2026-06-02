using OrderManagement.Models.DTOs.Drivers;

namespace OrderManagement.Services;

public interface IDriverService
{
    Task<List<DriverResponse>> GetAllAsync();
    Task<DriverResponse> CreateAsync(CreateDriverRequest dto);
}
