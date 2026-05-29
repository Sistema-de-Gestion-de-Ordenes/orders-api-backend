using OrderManagement.Models.DTOs.Drivers;

namespace OrderManagement.Services;

public interface IDriverService
{
    Task<DriverResponse> CreateAsync(CreateDriverRequest dto);
}
