using OrderManagement.Common;
using OrderManagement.Models.DTOs.Drivers;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories;

namespace OrderManagement.Services;

public class DriverService : IDriverService
{
    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        { "image/jpeg", "image/jpg", "image/png" };

    private const long MaxPhotoSize = 5 * 1024 * 1024;

    private readonly IDriverRepository   _driverRepo;
    private readonly IWebHostEnvironment _env;

    public DriverService(IDriverRepository driverRepo, IWebHostEnvironment env)
    {
        _driverRepo = driverRepo;
        _env        = env;
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
        if (!AllowedMimeTypes.Contains(dto.Photo.ContentType))
            throw new DomainException("Only JPG or PNG images are allowed.");

        if (dto.Photo.Length > MaxPhotoSize)
            throw new DomainException("Photo must not exceed 5 MB.");

        if (await _driverRepo.GetByPlatesAsync(dto.Plates) is not null)
            throw new ConflictException("The license plates are already registered.");

        var ext          = Path.GetExtension(dto.Photo.FileName).ToLowerInvariant();
        var fileName     = $"{Guid.NewGuid()}{ext}";
        var webRoot      = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var uploadFolder = Path.Combine(webRoot, "uploads", "drivers");
        Directory.CreateDirectory(uploadFolder);
        var filePath = Path.Combine(uploadFolder, fileName);

        try
        {
            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.Photo.CopyToAsync(stream);
        }
        catch
        {
            throw new DomainException("Failed to save photo. Please try again.");
        }

        var driver = new Driver
        {
            Name      = dto.Name,
            Vehicle   = dto.Vehicle,
            Plates    = dto.Plates,
            Phone     = dto.Phone,
            PhotoUrl  = $"/uploads/drivers/{fileName}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Driver created;
        try
        {
            created = await _driverRepo.CreateAsync(driver);
        }
        catch
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            throw;
        }

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
