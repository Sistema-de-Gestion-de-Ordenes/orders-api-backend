using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Drivers;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories;
using OrderManagement.Services;

namespace GestionOrdenes.Tests.Unit;

public class DriverServiceTests
{
    private static Mock<IWebHostEnvironment> MakeEnv()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "gestion_ordenes_driver_tests");
        var env = new Mock<IWebHostEnvironment>();
        env.Setup(e => e.WebRootPath).Returns(tempRoot);
        env.Setup(e => e.ContentRootPath).Returns(tempRoot);
        return env;
    }

    private static Mock<IFormFile> MakePhoto(
        string contentType = "image/jpeg",
        long size          = 1024,
        string fileName    = "photo.jpg")
    {
        var photo = new Mock<IFormFile>();
        photo.Setup(p => p.ContentType).Returns(contentType);
        photo.Setup(p => p.Length).Returns(size);
        photo.Setup(p => p.FileName).Returns(fileName);
        photo.Setup(p => p.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);
        return photo;
    }

    private static DriverService CreateService(
        Mock<IDriverRepository>  driverRepo,
        Mock<IWebHostEnvironment>? env = null)
        => new(driverRepo.Object, (env ?? MakeEnv()).Object);

    // ── GetAllAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDrivers_WhenDriversExist()
    {
        var driverRepo = new Mock<IDriverRepository>();
        driverRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Driver>
        {
            new() { Id = 1, Name = "Carlos Pérez", Vehicle = "Moto Honda", Plates = "ABC-123", Phone = "88001234", PhotoUrl = null },
            new() { Id = 2, Name = "Laura Gómez",  Vehicle = "Bicicleta",  Plates = "XYZ-789", Phone = "88005678", PhotoUrl = "https://example.com/photo.jpg" }
        });

        var result = await CreateService(driverRepo).GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal(1,              result[0].Id);
        Assert.Equal("Carlos Pérez", result[0].Name);
        Assert.Equal("Moto Honda",   result[0].Vehicle);
        Assert.Equal("ABC-123",      result[0].Plates);
        Assert.Equal("88001234",     result[0].Phone);
        Assert.Null(result[0].PhotoUrl);
        Assert.Equal(2,             result[1].Id);
        Assert.Equal("Laura Gómez", result[1].Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoDriversExist()
    {
        var driverRepo = new Mock<IDriverRepository>();
        driverRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Driver>());

        var result = await CreateService(driverRepo).GetAllAsync();

        Assert.Empty(result);
    }

    // ── CreateAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ThrowsDomainException_WhenMimeTypeIsInvalid()
    {
        var driverRepo = new Mock<IDriverRepository>();
        var dto = new CreateDriverRequest
        {
            Name = "Carlos", Vehicle = "Moto", Plates = "ABC-123", Phone = "88001234",
            Photo = MakePhoto(contentType: "image/gif", fileName: "photo.gif").Object
        };

        await Assert.ThrowsAsync<DomainException>(() => CreateService(driverRepo).CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsDomainException_WhenPhotoExceedsMaxSize()
    {
        var driverRepo = new Mock<IDriverRepository>();
        var dto = new CreateDriverRequest
        {
            Name = "Carlos", Vehicle = "Moto", Plates = "ABC-123", Phone = "88001234",
            Photo = MakePhoto(size: 6 * 1024 * 1024).Object
        };

        await Assert.ThrowsAsync<DomainException>(() => CreateService(driverRepo).CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsConflictException_WhenPlatesAlreadyRegistered()
    {
        var driverRepo = new Mock<IDriverRepository>();
        driverRepo.Setup(r => r.GetByPlatesAsync("ABC-123"))
                  .ReturnsAsync(new Driver { Plates = "ABC-123" });

        var dto = new CreateDriverRequest
        {
            Name = "Carlos", Vehicle = "Moto", Plates = "ABC-123", Phone = "88001234",
            Photo = MakePhoto().Object
        };

        await Assert.ThrowsAsync<ConflictException>(() => CreateService(driverRepo).CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ReturnsMappedDriverResponse_WhenAllDataIsValid()
    {
        var driverRepo = new Mock<IDriverRepository>();
        driverRepo.Setup(r => r.GetByPlatesAsync("ABC-123")).ReturnsAsync((Driver?)null);
        driverRepo.Setup(r => r.CreateAsync(It.IsAny<Driver>()))
                  .ReturnsAsync((Driver d) => { d.Id = 1; return d; });

        var dto = new CreateDriverRequest
        {
            Name    = "Carlos Pérez",
            Vehicle = "Moto Honda",
            Plates  = "ABC-123",
            Phone   = "88001234",
            Photo   = MakePhoto().Object
        };

        var result = await CreateService(driverRepo).CreateAsync(dto);

        Assert.Equal(1,              result.Id);
        Assert.Equal("Carlos Pérez", result.Name);
        Assert.Equal("Moto Honda",   result.Vehicle);
        Assert.Equal("ABC-123",      result.Plates);
        Assert.Equal("88001234",     result.Phone);
        Assert.NotNull(result.PhotoUrl);
        Assert.Contains("/uploads/drivers/", result.PhotoUrl);
    }

    [Fact]
    public async Task CreateAsync_DeletesFile_WhenRepositoryThrows()
    {
        var driverRepo = new Mock<IDriverRepository>();
        driverRepo.Setup(r => r.GetByPlatesAsync(It.IsAny<string>())).ReturnsAsync((Driver?)null);
        driverRepo.Setup(r => r.CreateAsync(It.IsAny<Driver>()))
                  .ThrowsAsync(new Exception("DB error"));

        var dto = new CreateDriverRequest
        {
            Name = "Carlos", Vehicle = "Moto", Plates = "ABC-123", Phone = "88001234",
            Photo = MakePhoto().Object
        };

        await Assert.ThrowsAsync<Exception>(() => CreateService(driverRepo).CreateAsync(dto));
    }
}
