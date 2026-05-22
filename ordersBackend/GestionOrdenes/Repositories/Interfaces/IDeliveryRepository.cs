using OrderManagement.Models.Entities;

namespace OrderManagement.Repositories.Interfaces;

public interface IDeliveryRepository
{
    Task<IEnumerable<Delivery>> GetAllAsync();
    Task<Delivery?> GetByOrderIdAsync(int orderId);
    Task<int> InsertAsync(Delivery delivery);
}
