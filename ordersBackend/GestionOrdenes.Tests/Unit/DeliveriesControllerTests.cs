using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderManagement.Common;
using OrderManagement.Controllers;
using OrderManagement.Models.DTOs.Deliveries;
using OrderManagement.Services;

namespace GestionOrdenes.Tests.Unit;

public class DeliveriesControllerTests
{
    private static DeliveriesController CreateController(IDeliveryService service, string userName = "testUser")
    {
        var controller = new DeliveriesController(service);
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, userName)], "test"));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        return controller;
    }

    // --- GetById ---

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenDeliveryDoesNotExist()
    {
        var service = new Mock<IDeliveryService>();
        service.Setup(s => s.GetByIdAsync(99))
               .ThrowsAsync(new NotFoundException("The delivery with id 99 does not exist."));
        var controller = CreateController(service.Object);

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
        var controller = CreateController(service.Object);

        var response = await controller.GetById(2);

        var ok      = Assert.IsType<OkObjectResult>(response);
        var payload = Assert.IsType<DeliveryDetailResponse>(ok.Value);
        Assert.Equal(2, payload.Id);
    }

    // --- Update ---

    [Fact]
    public async Task Update_ReturnsOk_WhenDeliveryIsUpdated()
    {
        var service = new Mock<IDeliveryService>();
        service.Setup(s => s.UpdateAsync(1, It.IsAny<UpdateDeliveryRequest>()))
               .ReturnsAsync(new DeliveryResponse
               {
                   Id = 1, ClientId = 1, DriverId = 2,
                   Origin = "New Origin", Destination = "New Destination", Status = "pending"
               });
        var controller = CreateController(service.Object);

        var response = await controller.Update(1, new UpdateDeliveryRequest
        {
            Origin = "New Origin", Destination = "New Destination", DriverId = 2
        });

        var ok      = Assert.IsType<OkObjectResult>(response);
        var payload = Assert.IsType<DeliveryResponse>(ok.Value);
        Assert.Equal(1, payload.Id);
        Assert.Equal(2, payload.DriverId);
        Assert.Equal("New Origin", payload.Origin);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenDeliveryDoesNotExist()
    {
        var service = new Mock<IDeliveryService>();
        service.Setup(s => s.UpdateAsync(99, It.IsAny<UpdateDeliveryRequest>()))
               .ThrowsAsync(new NotFoundException("The delivery with id 99 does not exist."));
        var controller = CreateController(service.Object);

        var response = await controller.Update(99, new UpdateDeliveryRequest
        {
            Origin = "A", Destination = "B", DriverId = 1
        });

        Assert.IsType<NotFoundObjectResult>(response);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenDriverDoesNotExist()
    {
        var service = new Mock<IDeliveryService>();
        service.Setup(s => s.UpdateAsync(1, It.IsAny<UpdateDeliveryRequest>()))
               .ThrowsAsync(new NotFoundException("The driver with id 999 does not exist."));
        var controller = CreateController(service.Object);

        var response = await controller.Update(1, new UpdateDeliveryRequest
        {
            Origin = "A", Destination = "B", DriverId = 999
        });

        Assert.IsType<NotFoundObjectResult>(response);
    }

    // --- UpdateStatus ---

    [Fact]
    public async Task UpdateStatus_ReturnsOk_WhenTransitionIsValid()
    {
        var service = new Mock<IDeliveryService>();
        service.Setup(s => s.UpdateStatusAsync(1, It.IsAny<UpdateStatusRequest>(), It.IsAny<string>()))
               .ReturnsAsync(new DeliveryStatusResponse { Id = 1, Status = "in_transit" });
        var controller = CreateController(service.Object, "driver1");

        var response = await controller.UpdateStatus(1, new UpdateStatusRequest { Status = "in_transit" });

        var ok      = Assert.IsType<OkObjectResult>(response);
        var payload = Assert.IsType<DeliveryStatusResponse>(ok.Value);
        Assert.Equal(1, payload.Id);
        Assert.Equal("in_transit", payload.Status);
    }

    [Fact]
    public async Task UpdateStatus_ReturnsBadRequest_WhenTransitionIsInvalid()
    {
        var service = new Mock<IDeliveryService>();
        service.Setup(s => s.UpdateStatusAsync(1, It.IsAny<UpdateStatusRequest>(), It.IsAny<string>()))
               .ThrowsAsync(new DomainException("Cannot transition from delivered to pending."));
        var controller = CreateController(service.Object);

        var response = await controller.UpdateStatus(1, new UpdateStatusRequest { Status = "pending" });

        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public async Task UpdateStatus_ReturnsNotFound_WhenDeliveryDoesNotExist()
    {
        var service = new Mock<IDeliveryService>();
        service.Setup(s => s.UpdateStatusAsync(99, It.IsAny<UpdateStatusRequest>(), It.IsAny<string>()))
               .ThrowsAsync(new NotFoundException("The delivery with id 99 does not exist."));
        var controller = CreateController(service.Object);

        var response = await controller.UpdateStatus(99, new UpdateStatusRequest { Status = "in_transit" });

        Assert.IsType<NotFoundObjectResult>(response);
    }

    [Fact]
    public async Task UpdateStatus_Returns500_WhenUnexpectedExceptionOccurs()
    {
        var service = new Mock<IDeliveryService>();
        service.Setup(s => s.UpdateStatusAsync(1, It.IsAny<UpdateStatusRequest>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception("Unexpected error"));
        var controller = CreateController(service.Object);

        var response = await controller.UpdateStatus(1, new UpdateStatusRequest { Status = "in_transit" });

        var result = Assert.IsType<ObjectResult>(response);
        Assert.Equal(500, result.StatusCode);
    }
}
