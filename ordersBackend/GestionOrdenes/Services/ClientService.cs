using OrderManagement.Common;
using OrderManagement.Models.DTOs.Clients;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories;

namespace OrderManagement.Services;

public class ClientService : IClientService
{
    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        { "image/jpeg", "image/jpg", "image/png" };

    private const long MaxPhotoSize = 5 * 1024 * 1024;

    private readonly IClientRepository    _clientRepo;
    private readonly IWebHostEnvironment  _env;

    public ClientService(IClientRepository clientRepo, IWebHostEnvironment env)
    {
        _clientRepo = clientRepo;
        _env        = env;
    }

    public async Task<ClientResponse> CreateAsync(CreateClientRequest dto)
    {
        if (!AllowedMimeTypes.Contains(dto.Photo.ContentType))
            throw new DomainException("Solo se permiten imágenes JPG o PNG.");

        if (dto.Photo.Length > MaxPhotoSize)
            throw new DomainException("La foto no puede superar los 5 MB.");

        if (await _clientRepo.GetByEmailAsync(dto.Email) is not null)
            throw new ConflictException("El correo electrónico ya está en uso.");

        var ext          = Path.GetExtension(dto.Photo.FileName).ToLowerInvariant();
        var fileName     = $"{Guid.NewGuid()}{ext}";
        var webRoot      = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var uploadFolder = Path.Combine(webRoot, "uploads", "clients");
        Directory.CreateDirectory(uploadFolder);
        var filePath = Path.Combine(uploadFolder, fileName);

        try
        {
            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.Photo.CopyToAsync(stream);
        }
        catch
        {
            throw new DomainException("No se pudo guardar la foto. Inténtalo de nuevo.");
        }

        var client = new Client
        {
            Name         = dto.Name,
            Email        = dto.Email,
            Phone        = dto.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            PhotoUrl     = $"/uploads/clients/{fileName}",
            CreatedAt    = DateTime.UtcNow,
            UpdatedAt    = DateTime.UtcNow
        };

        Client created;
        try
        {
            created = await _clientRepo.CreateAsync(client);
        }
        catch
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            throw;
        }

        return new ClientResponse
        {
            Id       = created.Id,
            Name     = created.Name,
            Email    = created.Email,
            Phone    = created.Phone,
            PhotoUrl = created.PhotoUrl
        };
    }

    public async Task<IEnumerable<ClientResponse>> GetAllAsync()
    {
        var clients = await _clientRepo.GetAllAsync();
        return clients.Select(c => new ClientResponse
        {
            Id        = c.Id,
            Name      = c.Name,
            Email     = c.Email,
            Phone     = c.Phone,
            PhotoUrl  = c.PhotoUrl,
        });
    }
}
