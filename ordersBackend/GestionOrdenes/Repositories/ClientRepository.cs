using Microsoft.EntityFrameworkCore;
using OrderManagement.Models.Entities;
using OrderManagement.Persistence;

namespace OrderManagement.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly OrderManagementDbContext _db;
    public ClientRepository(OrderManagementDbContext db) => _db = db;

    public async Task<Client?> GetByIdAsync(int id)
        => await _db.Clients.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Client?> GetByEmailAsync(string email)
        => await _db.Clients.FirstOrDefaultAsync(c => c.Email == email);

    public async Task<Client> CreateAsync(Client client)
    {
        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
        return client;
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    => await _db.Clients
        .OrderByDescending(c => c.CreatedAt)
        .ToListAsync();
}
