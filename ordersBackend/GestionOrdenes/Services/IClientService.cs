using OrderManagement.Models.DTOs.Clients;

namespace OrderManagement.Services;

public interface IClientService
{
    Task<ClientResponse> CreateAsync(CreateClientRequest dto);
    Task<IEnumerable<ClientResponse>> GetAllAsync();
}
