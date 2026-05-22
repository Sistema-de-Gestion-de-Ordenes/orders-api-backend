using OrderManagement.Models.Entities;
using OrderManagement.Models.Enums;

namespace OrderManagement.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync(string? search);
    Task<Order?> GetByIdAsync(int id);
    Task<int> InsertAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(int id);
    Task ChangeStatusAsync(int id, OrderStatus status);
    Task AssignDriverAsync(int id, int driverId);
    Task<string> GenerateOrderNumberAsync();
}
