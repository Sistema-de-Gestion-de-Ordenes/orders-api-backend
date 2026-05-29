using OrderManagement.Models.Entities;

namespace OrderManagement.Repositories;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(int id);
    Task<Client?> GetByEmailAsync(string email);
    Task<Client> CreateAsync(Client client);
}
