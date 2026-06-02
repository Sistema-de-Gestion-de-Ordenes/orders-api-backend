using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderManagement.Common;
using OrderManagement.Controllers;
using OrderManagement.Models.DTOs.Drivers;
using OrderManagement.Services;

namespace GestionOrdenes.Tests.Unit;

public class DriversControllerTests
{
    // ── GetAll ─────────────────────────────────────────────────────────────────

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
        Assert.Equal(1,              payload[0].Id);
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

    // ── Create ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_Returns201_WithDriverResponse_WhenServiceSucceeds()
    {
        var expected = new DriverResponse
        {
            Id = 1, Name = "Carlos Pérez", Vehicle = "Moto Honda",
            Plates = "ABC-123", Phone = "88001234", PhotoUrl = "/uploads/drivers/abc.jpg"
        };
        var service = new Mock<IDriverService>();
        service.Setup(s => s.CreateAsync(It.IsAny<CreateDriverRequest>())).ReturnsAsync(expected);
        var controller = new DriversController(service.Object);

        var response = await controller.Create(new CreateDriverRequest());

        var result  = Assert.IsType<ObjectResult>(response);
        Assert.Equal(201, result.StatusCode);
        var payload = Assert.IsType<DriverResponse>(result.Value);
        Assert.Equal(1,              payload.Id);
        Assert.Equal("Carlos Pérez", payload.Name);
        Assert.Equal("ABC-123",      payload.Plates);
        Assert.Equal("/uploads/drivers/abc.jpg", payload.PhotoUrl);
    }

    [Fact]
    public async Task Create_Returns409_WhenPlatesAlreadyRegistered()
    {
        var service = new Mock<IDriverService>();
        service.Setup(s => s.CreateAsync(It.IsAny<CreateDriverRequest>()))
               .ThrowsAsync(new ConflictException("The license plates are already registered."));
        var controller = new DriversController(service.Object);

        var response = await controller.Create(new CreateDriverRequest());

        var result = Assert.IsType<ConflictObjectResult>(response);
        Assert.Equal(409, result.StatusCode);
    }

    [Fact]
    public async Task Create_Returns400_WhenPhotoFormatIsInvalid()
    {
        var service = new Mock<IDriverService>();
        service.Setup(s => s.CreateAsync(It.IsAny<CreateDriverRequest>()))
               .ThrowsAsync(new DomainException("Only JPG or PNG images are allowed."));
        var controller = new DriversController(service.Object);

        var response = await controller.Create(new CreateDriverRequest());

        var result = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task Create_Returns400_WhenPhotoExceedsMaxSize()
    {
        var service = new Mock<IDriverService>();
        service.Setup(s => s.CreateAsync(It.IsAny<CreateDriverRequest>()))
               .ThrowsAsync(new DomainException("Photo must not exceed 5 MB."));
        var controller = new DriversController(service.Object);

        var response = await controller.Create(new CreateDriverRequest());

        var result = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task Create_Returns500_WhenUnexpectedExceptionIsThrown()
    {
        var service = new Mock<IDriverService>();
        service.Setup(s => s.CreateAsync(It.IsAny<CreateDriverRequest>()))
               .ThrowsAsync(new Exception("Unexpected error"));
        var controller = new DriversController(service.Object);

        var response = await controller.Create(new CreateDriverRequest());

        var result = Assert.IsType<ObjectResult>(response);
        Assert.Equal(500, result.StatusCode);
    }
}
