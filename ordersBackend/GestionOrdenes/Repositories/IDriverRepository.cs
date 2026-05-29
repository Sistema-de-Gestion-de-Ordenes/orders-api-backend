using OrderManagement.Models.Entities;

namespace OrderManagement.Repositories;

public interface IDriverRepository
{
    Task<Driver?> GetByIdAsync(int id);
    Task<Driver?> GetByPlatesAsync(string plates);
    Task<Driver> CreateAsync(Driver driver);
}
