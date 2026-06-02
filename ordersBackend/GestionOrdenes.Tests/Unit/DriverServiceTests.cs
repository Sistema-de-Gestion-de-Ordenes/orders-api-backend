using Moq;
using OrderManagement.Models.DTOs.Drivers;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories;
using OrderManagement.Services;

namespace GestionOrdenes.Tests.Unit;

public class DriverServiceTests
{
    private static DriverService CreateService(Mock<IDriverRepository> driverRepo)
        => new(driverRepo.Object);

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
        Assert.Equal(1,           result[0].Id);
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
}
