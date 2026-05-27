using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderManagement.Controllers;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Deliveries;
using OrderManagement.Services.Interfaces;

namespace GestionOrdenes.Tests.Unit;

public class DeliveriesControllerTests
{
    [Fact]
    public async Task GetById_ReturnsBadRequest_WhenIdFormatIsInvalid()
    {
        var service = new Mock<IDeliveryService>();
        var controller = new DeliveriesController(service.Object);

        var response = await controller.GetById("abc");

        var badRequest = Assert.IsType<BadRequestObjectResult>(response);
        var payload = Assert.IsType<ApiResponse<object>>(badRequest.Value);
        Assert.False(payload.Success);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenIdIsValid()
    {
        var service = new Mock<IDeliveryService>();
        service.Setup(s => s.GetDetailByIdAsync(2)).ReturnsAsync(new DeliveryDetailDto
        {
            Id = 2,
            Status = "delivered",
            Origin = "Store",
            Destination = "Apartment",
            Client = new DeliveryClientDto { Name = "Alice", Email = "alice@mail.com", RegisteredSince = DateTime.UtcNow },
            DeliveryPerson = new DeliveryDriverDto { Name = "Bob", Phone = "555-2000", PhotoUrl = "", Verified = true }
        });

        var controller = new DeliveriesController(service.Object);

        var response = await controller.GetById("2");

        var ok = Assert.IsType<OkObjectResult>(response);
        var payload = Assert.IsType<ApiResponse<DeliveryDetailDto>>(ok.Value);
        Assert.True(payload.Success);
        Assert.Equal(2, payload.Data!.Id);
    }
}
