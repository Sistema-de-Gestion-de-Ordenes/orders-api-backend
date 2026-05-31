using OrderManagement.Models.Entities;

namespace OrderManagement.Repositories;

public interface IAuthRepository
{
    Task<Client?> GetByEmailAsync(string email);
    Task<User?> GetUserByEmailAsync(string email);
}
