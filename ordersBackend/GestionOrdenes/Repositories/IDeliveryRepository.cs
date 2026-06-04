using OrderManagement.Models.Entities;

namespace OrderManagement.Repositories;

public interface IDeliveryRepository
{
    Task<IEnumerable<Delivery>> GetAllAsync();
    Task<Delivery?> GetByIdAsync(int id);
    Task<Delivery> CreateAsync(Delivery delivery);
    Task<Delivery> UpdateAsync(Delivery delivery);
    Task DeleteAsync(Delivery delivery);
}
