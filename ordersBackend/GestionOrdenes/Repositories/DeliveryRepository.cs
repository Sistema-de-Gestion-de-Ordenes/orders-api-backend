using Microsoft.EntityFrameworkCore;
using OrderManagement.Models.Entities;
using OrderManagement.Persistence;

namespace OrderManagement.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly OrderManagementDbContext _db;
    public DeliveryRepository(OrderManagementDbContext db) => _db = db;

    public async Task<IEnumerable<Delivery>> GetAllAsync()
        => await _db.Deliveries
            .Include(d => d.Client)
            .Include(d => d.Driver)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();

    public async Task<Delivery?> GetByIdAsync(int id)
        => await _db.Deliveries
            .Include(d => d.Client)
            .Include(d => d.Driver)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<Delivery> CreateAsync(Delivery delivery)
    {
        _db.Deliveries.Add(delivery);
        await _db.SaveChangesAsync();
        return delivery;
    }

    public async Task<Delivery> UpdateAsync(Delivery delivery)
    {
        delivery.UpdatedAt = DateTime.UtcNow;
        _db.Deliveries.Update(delivery);
        await _db.SaveChangesAsync();
        return delivery;
    }

    public async Task DeleteAsync(Delivery delivery)
    {
        _db.Deliveries.Remove(delivery);
        await _db.SaveChangesAsync();
    }

    public async Task SaveAsync() => await _db.SaveChangesAsync();
}
