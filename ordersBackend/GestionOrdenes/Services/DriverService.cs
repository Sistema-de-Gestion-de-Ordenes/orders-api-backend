using OrderManagement.Common;
using OrderManagement.Models.DTOs.Drivers;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories;

namespace OrderManagement.Services;

public class DriverService : IDriverService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png" };

    private readonly IDriverRepository _driverRepo;

    public DriverService(IDriverRepository driverRepo)
    {
        _driverRepo = driverRepo;
    }

    public async Task<List<DriverResponse>> GetAllAsync()
    {
        var drivers = await _driverRepo.GetAllAsync();
        return drivers.Select(d => new DriverResponse
        {
            Id       = d.Id,
            Name     = d.Name,
            Vehicle  = d.Vehicle,
            Plates   = d.Plates,
            Phone    = d.Phone,
            PhotoUrl = d.PhotoUrl
        }).ToList();
    }

    public async Task<DriverResponse> CreateAsync(CreateDriverRequest dto)
    {
        if (dto.Photo is null)
            throw new DomainException("The profile photo is required.");

        var ext = Path.GetExtension(dto.Photo.FileName);
        if (!AllowedExtensions.Contains(ext))
            throw new DomainException("Only JPG or PNG images are allowed.");

        var existing = await _driverRepo.GetByPlatesAsync(dto.Plates);
        if (existing is not null)
            throw new ConflictException("The license plates are already registered.");

        // TODO: Upload to Cloudinary (pending issue)
        var photoUrl = $"https://placeholder.storage.com/drivers/{Guid.NewGuid()}{ext}";

        var driver = new Driver
        {
            Name      = dto.Name,
            Vehicle   = dto.Vehicle,
            Plates    = dto.Plates,
            Phone     = dto.Phone,
            PhotoUrl  = photoUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _driverRepo.CreateAsync(driver);

        return new DriverResponse
        {
            Id       = created.Id,
            Name     = created.Name,
            Vehicle  = created.Vehicle,
            Plates   = created.Plates,
            Phone    = created.Phone,
            PhotoUrl = created.PhotoUrl
        };
    }
}
