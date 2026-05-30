using Microsoft.EntityFrameworkCore;
using OrderManagement.Models.Entities;
using OrderManagement.Persistence;

namespace OrderManagement.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly OrderManagementDbContext _db;
    public AuthRepository(OrderManagementDbContext db) => _db = db;

    public async Task<Client?> GetByEmailAsync(string email)
        => await _db.Clients.FirstOrDefaultAsync(c => c.Email == email);
}
