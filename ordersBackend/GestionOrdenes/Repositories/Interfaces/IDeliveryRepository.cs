using OrderManagement.Models.Entities;
using OrderManagement.Repositories.Models;

namespace OrderManagement.Repositories.Interfaces;

public interface IDeliveryRepository
{
    Task<IEnumerable<Delivery>> GetAllAsync();
    Task<Delivery?> GetByOrderIdAsync(int orderId);
    Task<DeliveryDetailRecord?> GetDetailByIdAsync(int id);
    Task<int> InsertAsync(Delivery delivery);
    Task UpdateStatusAsync(int id, string newStatus);
}
