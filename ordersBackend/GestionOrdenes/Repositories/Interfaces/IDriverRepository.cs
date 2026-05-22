using OrderManagement.Models.Entities;

namespace OrderManagement.Repositories.Interfaces;

public interface IDriverRepository
{
    Task<IEnumerable<Driver>> GetAllAsync(string? search);
    Task<Driver?> GetByIdAsync(int id);
    Task<Driver?> GetByEmailAsync(string email);
    Task<int> InsertAsync(Driver driver);
    Task UpdateAsync(Driver driver);
    Task DeleteAsync(int id);
    Task<bool> HasOrdersInProgressAsync(int driverId);
    Task SetAvailableAsync(int id, bool available);
}
