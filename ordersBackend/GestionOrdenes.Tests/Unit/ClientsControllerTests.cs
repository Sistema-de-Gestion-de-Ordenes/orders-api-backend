using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderManagement.Common;
using OrderManagement.Controllers;
using OrderManagement.Models.DTOs.Clients;
using OrderManagement.Services;

namespace GestionOrdenes.Tests.Unit;

public class ClientsControllerTests
{
    // ── Create ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_Returns201_WithClientResponse_WhenServiceSucceeds()
    {
        var expected = new ClientResponse
        {
            Id = 1, Name = "Maria Lopez", Email = "maria@test.com",
            Phone = "88001234", PhotoUrl = "/uploads/clients/abc.jpg"
        };
        var service = new Mock<IClientService>();
        service.Setup(s => s.CreateAsync(It.IsAny<CreateClientRequest>())).ReturnsAsync(expected);
        var controller = new ClientsController(service.Object);

        var response = await controller.Create(new CreateClientRequest());

        var result  = Assert.IsType<ObjectResult>(response);
        Assert.Equal(201, result.StatusCode);
        var payload = Assert.IsType<ClientResponse>(result.Value);
        Assert.Equal(1,                payload.Id);
        Assert.Equal("Maria Lopez",    payload.Name);
        Assert.Equal("maria@test.com", payload.Email);
        Assert.Equal("/uploads/clients/abc.jpg", payload.PhotoUrl);
    }

    [Fact]
    public async Task Create_Returns409_WhenEmailAlreadyRegistered()
    {
        var service = new Mock<IClientService>();
        service.Setup(s => s.CreateAsync(It.IsAny<CreateClientRequest>()))
               .ThrowsAsync(new ConflictException("The email is already in use."));
        var controller = new ClientsController(service.Object);

        var response = await controller.Create(new CreateClientRequest());

        var result = Assert.IsType<ConflictObjectResult>(response);
        Assert.Equal(409, result.StatusCode);
    }

    [Fact]
    public async Task Create_Returns400_WhenPhotoFormatIsInvalid()
    {
        var service = new Mock<IClientService>();
        service.Setup(s => s.CreateAsync(It.IsAny<CreateClientRequest>()))
               .ThrowsAsync(new DomainException("Only JPG or PNG images are allowed."));
        var controller = new ClientsController(service.Object);

        var response = await controller.Create(new CreateClientRequest());

        var result = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task Create_Returns400_WhenPhotoExceedsMaxSize()
    {
        var service = new Mock<IClientService>();
        service.Setup(s => s.CreateAsync(It.IsAny<CreateClientRequest>()))
               .ThrowsAsync(new DomainException("Photo must not exceed 5 MB."));
        var controller = new ClientsController(service.Object);

        var response = await controller.Create(new CreateClientRequest());

        var result = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task Create_Returns500_WhenUnexpectedExceptionIsThrown()
    {
        var service = new Mock<IClientService>();
        service.Setup(s => s.CreateAsync(It.IsAny<CreateClientRequest>()))
               .ThrowsAsync(new Exception("Unexpected error"));
        var controller = new ClientsController(service.Object);

        var response = await controller.Create(new CreateClientRequest());

        var result = Assert.IsType<ObjectResult>(response);
        Assert.Equal(500, result.StatusCode);
    }
}
