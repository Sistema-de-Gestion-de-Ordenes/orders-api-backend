using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderManagement.Controllers;
using OrderManagement.Models.DTOs.Drivers;
using OrderManagement.Services;

namespace GestionOrdenes.Tests.Unit;

public class DriversControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOk_WithDriverList()
    {
        var service = new Mock<IDriverService>();
        service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<DriverResponse>
        {
            new() { Id = 1, Name = "Carlos Pérez", Vehicle = "Moto Honda", Plates = "ABC-123", Phone = "88001234", PhotoUrl = null },
            new() { Id = 2, Name = "Laura Gómez",  Vehicle = "Bicicleta",  Plates = "XYZ-789", Phone = "88005678", PhotoUrl = null }
        });
        var controller = new DriversController(service.Object);

        var response = await controller.GetAll();

        var ok      = Assert.IsType<OkObjectResult>(response);
        var payload = Assert.IsType<List<DriverResponse>>(ok.Value);
        Assert.Equal(2, payload.Count);
        Assert.Equal(1,           payload[0].Id);
        Assert.Equal("Carlos Pérez", payload[0].Name);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithEmptyList_WhenNoDriversExist()
    {
        var service = new Mock<IDriverService>();
        service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<DriverResponse>());
        var controller = new DriversController(service.Object);

        var response = await controller.GetAll();

        var ok      = Assert.IsType<OkObjectResult>(response);
        var payload = Assert.IsType<List<DriverResponse>>(ok.Value);
        Assert.Empty(payload);
    }

    [Fact]
    public async Task GetAll_ReturnsInternalServerError_WhenServiceThrows()
    {
        var service = new Mock<IDriverService>();
        service.Setup(s => s.GetAllAsync()).ThrowsAsync(new Exception("Unexpected error"));
        var controller = new DriversController(service.Object);

        var response = await controller.GetAll();

        var result = Assert.IsType<ObjectResult>(response);
        Assert.Equal(500, result.StatusCode);
    }
}
