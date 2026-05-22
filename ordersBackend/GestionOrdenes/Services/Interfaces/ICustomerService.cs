using OrderManagement.Models.DTOs.Customers;

namespace OrderManagement.Services.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllAsync(string? search);
    Task<CustomerDto> GetByIdAsync(int id);
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
    Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto);
    Task DeleteAsync(int id);
}
