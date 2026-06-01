using Microsoft.EntityFrameworkCore;
using OrderManagement.Models.Entities;
using OrderManagement.Persistence;

namespace OrderManagement.Repositories;

public class DriverRepository : IDriverRepository
{
    private readonly OrderManagementDbContext _db;
    public DriverRepository(OrderManagementDbContext db) => _db = db;

    public async Task<List<Driver>> GetAllAsync()
        => await _db.Drivers.OrderBy(d => d.Name).ToListAsync();

    public async Task<Driver?> GetByIdAsync(int id)
        => await _db.Drivers.FirstOrDefaultAsync(d => d.Id == id);

    public async Task<Driver?> GetByPlatesAsync(string plates)
        => await _db.Drivers.FirstOrDefaultAsync(d => d.Plates == plates);

    public async Task<Driver> CreateAsync(Driver driver)
    {
        _db.Drivers.Add(driver);
        await _db.SaveChangesAsync();
        return driver;
    }
}
