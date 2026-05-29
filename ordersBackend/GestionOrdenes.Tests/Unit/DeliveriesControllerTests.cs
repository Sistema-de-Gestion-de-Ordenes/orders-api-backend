using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderManagement.Common;
using OrderManagement.Controllers;
using OrderManagement.Models.DTOs.Deliveries;
using OrderManagement.Services;

namespace GestionOrdenes.Tests.Unit;

public class DeliveriesControllerTests
{
    [Fact]
    public async Task GetById_ReturnsNotFound_WhenDeliveryDoesNotExist()
    {
        var service = new Mock<IDeliveryService>();
        service.Setup(s => s.GetByIdAsync(99))
               .ThrowsAsync(new NotFoundException("The delivery with id 99 does not exist."));
        var controller = new DeliveriesController(service.Object);

        var response = await controller.GetById(99);

        Assert.IsType<NotFoundObjectResult>(response);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenDeliveryExists()
    {
        var service = new Mock<IDeliveryService>();
        service.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(new DeliveryDetailResponse
        {
            Id          = 2,
            Status      = "pending",
            Origin      = "Store",
            Destination = "Home",
            Client      = new ClientDetailDto { Name = "Alice", Email = "alice@mail.com", RegisteredSince = "01/01/2026" },
            Driver      = new DriverDetailDto { Name = "Bob", Phone = "555-2000", PhotoUrl = null, Verified = true }
        });
        var controller = new DeliveriesController(service.Object);

        var response = await controller.GetById(2);

        var ok      = Assert.IsType<OkObjectResult>(response);
        var payload = Assert.IsType<DeliveryDetailResponse>(ok.Value);
        Assert.Equal(2, payload.Id);
    }

    [Fact]
    public async Task UpdateStatus_ReturnsBadRequest_WhenTransitionIsInvalid()
    {
        var service = new Mock<IDeliveryService>();
        service.Setup(s => s.UpdateStatusAsync(1, It.IsAny<UpdateStatusRequest>()))
               .ThrowsAsync(new DomainException("Cannot transition from delivered to pending."));
        var controller = new DeliveriesController(service.Object);

        var response = await controller.UpdateStatus(1, new UpdateStatusRequest { Status = "pending" });

        Assert.IsType<BadRequestObjectResult>(response);
    }
}
