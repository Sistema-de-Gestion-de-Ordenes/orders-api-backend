using Microsoft.Extensions.Logging;
using Moq;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Deliveries;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories;
using OrderManagement.Services;

namespace GestionOrdenes.Tests.Unit;

public class DeliveryServiceTests
{
    private static DeliveryService CreateService(
        Mock<IDeliveryRepository> deliveryRepo,
        Mock<IClientRepository>? clientRepo = null,
        Mock<IDriverRepository>? driverRepo = null,
        Mock<INotificationService>? notificationService = null)
        => new(
            deliveryRepo.Object,
            (clientRepo ?? new Mock<IClientRepository>()).Object,
            (driverRepo ?? new Mock<IDriverRepository>()).Object,
            (notificationService ?? new Mock<INotificationService>()).Object,
            new Mock<ILogger<DeliveryService>>().Object);

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedDto_WhenDeliveryExists()
    {
        var deliveryRepo = new Mock<IDeliveryRepository>();
        deliveryRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new Delivery
        {
            Id          = 5,
            Status      = "pending",
            Origin      = "Warehouse A",
            Destination = "Customer Home",
            ClientId    = 1,
            DriverId    = 1,
            Client = new Client
            {
                Name      = "John Doe",
                Email     = "john@example.com",
                CreatedAt = new DateTime(2026, 5, 1)
            },
            Driver = new Driver
            {
                Name       = "Jane Driver",
                Phone      = "555-1000",
                PhotoUrl   = "https://example.com/driver.jpg",
                IsVerified = true
            }
        });

        var result = await CreateService(deliveryRepo).GetByIdAsync(5);

        Assert.Equal(5, result.Id);
        Assert.Equal("pending", result.Status);
        Assert.Equal("John Doe", result.Client.Name);
        Assert.Equal("john@example.com", result.Client.Email);
        Assert.Equal("Jane Driver", result.Driver.Name);
        Assert.True(result.Driver.Verified);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFound_WhenDeliveryDoesNotExist()
    {
        var deliveryRepo = new Mock<IDeliveryRepository>();
        deliveryRepo.Setup(r => r.GetByIdAsync(404)).ReturnsAsync((Delivery?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => CreateService(deliveryRepo).GetByIdAsync(404));
    }

    [Fact]
    public async Task UpdateStatusAsync_ThrowsDomainException_WhenTransitionIsInvalid()
    {
        var deliveryRepo = new Mock<IDeliveryRepository>();
        deliveryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Delivery
        {
            Id = 1, Status = "delivered", ClientId = 1, DriverId = 1,
            Origin = "A", Destination = "B"
        });

        await Assert.ThrowsAsync<DomainException>(() =>
            CreateService(deliveryRepo).UpdateStatusAsync(1, new UpdateStatusRequest { Status = "pending" }));
    }

    [Fact]
    public async Task DeleteAsync_ExistingDelivery_CallsDeleteOnRepository()
    {
        var delivery = new Delivery { Id = 10, ClientId = 1, DriverId = 1, Origin = "A", Destination = "B" };

        var deliveryRepo = new Mock<IDeliveryRepository>();
        deliveryRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(delivery);
        deliveryRepo.Setup(r => r.DeleteAsync(delivery)).Returns(Task.CompletedTask);

        await CreateService(deliveryRepo).DeleteAsync(10);

        deliveryRepo.Verify(r => r.DeleteAsync(delivery), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeliveryNotFound_ThrowsNotFoundException()
    {
        var deliveryRepo = new Mock<IDeliveryRepository>();
        deliveryRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Delivery?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => CreateService(deliveryRepo).DeleteAsync(99));
    }

    [Fact]
    public async Task UpdateAsync_ReturnsUpdatedResponse_WhenDeliveryAndDriverExist()
    {
        var deliveryRepo = new Mock<IDeliveryRepository>();
        var driverRepo   = new Mock<IDriverRepository>();

        deliveryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Delivery
        {
            Id = 1, Status = "pending", Origin = "Warehouse A", Destination = "Customer Home",
            ClientId = 1, DriverId = 1
        });
        driverRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Driver
        {
            Id = 2, Name = "New Driver", Phone = "555-9999", Vehicle = "Truck", Plates = "ABC-123"
        });
        deliveryRepo.Setup(r => r.UpdateAsync(It.IsAny<Delivery>())).ReturnsAsync((Delivery d) => d);

        var dto    = new UpdateDeliveryRequest { Origin = "New Origin", Destination = "New Destination", DriverId = 2 };
        var result = await CreateService(deliveryRepo, driverRepo: driverRepo).UpdateAsync(1, dto);

        Assert.Equal(1, result.Id);
        Assert.Equal("New Origin", result.Origin);
        Assert.Equal("New Destination", result.Destination);
        Assert.Equal(2, result.DriverId);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFound_WhenDeliveryDoesNotExist()
    {
        var deliveryRepo = new Mock<IDeliveryRepository>();
        deliveryRepo.Setup(r => r.GetByIdAsync(404)).ReturnsAsync((Delivery?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateService(deliveryRepo).UpdateAsync(404, new UpdateDeliveryRequest
            {
                Origin = "A", Destination = "B", DriverId = 1
            }));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFound_WhenDriverDoesNotExist()
    {
        var deliveryRepo = new Mock<IDeliveryRepository>();
        var driverRepo   = new Mock<IDriverRepository>();

        deliveryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Delivery
        {
            Id = 1, Status = "pending", Origin = "A", Destination = "B",
            ClientId = 1, DriverId = 1
        });
        driverRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Driver?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateService(deliveryRepo, driverRepo: driverRepo).UpdateAsync(1, new UpdateDeliveryRequest
            {
                Origin = "A", Destination = "B", DriverId = 999
            }));
    }
}
