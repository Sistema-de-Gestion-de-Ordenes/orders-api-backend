using OrderManagement.Common;
using OrderManagement.Models.DTOs.Clients;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories;

namespace OrderManagement.Services;

public class ClientService : IClientService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png" };

    private readonly IClientRepository _clientRepo;

    public ClientService(IClientRepository clientRepo)
    {
        _clientRepo = clientRepo;
    }

    public async Task<ClientResponse> CreateAsync(CreateClientRequest dto)
    {
        if (dto.Photo is null)
            throw new DomainException("The profile photo is required.");

        var ext = Path.GetExtension(dto.Photo.FileName);
        if (!AllowedExtensions.Contains(ext))
            throw new DomainException("Only JPG or PNG images are allowed.");

        var existing = await _clientRepo.GetByEmailAsync(dto.Email);
        if (existing is not null)
            throw new ConflictException("The email is already in use.");

        // TODO: Upload to Cloudinary (pending issue)
        var photoUrl = $"https://placeholder.storage.com/clients/{Guid.NewGuid()}{ext}";

        var client = new Client
        {
            Name         = dto.Name,
            Email        = dto.Email,
            Phone        = dto.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            PhotoUrl     = photoUrl,
            CreatedAt    = DateTime.UtcNow,
            UpdatedAt    = DateTime.UtcNow
        };

        var created = await _clientRepo.CreateAsync(client);

        return new ClientResponse
        {
            Id       = created.Id,
            Name     = created.Name,
            Email    = created.Email,
            Phone    = created.Phone,
            PhotoUrl = created.PhotoUrl
        };
    }
}
