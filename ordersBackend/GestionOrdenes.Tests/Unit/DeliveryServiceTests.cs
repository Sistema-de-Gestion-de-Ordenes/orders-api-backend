using Moq;
using Microsoft.Extensions.Configuration;
using OrderManagement.Common;
using OrderManagement.Repositories.Interfaces;
using OrderManagement.Repositories.Models;
using OrderManagement.Services;

namespace GestionOrdenes.Tests.Unit;

public class DeliveryServiceTests
{
    [Fact]
    public async Task GetDetailByIdAsync_ReturnsMappedDto_WhenDeliveryExists()
    {
        var deliveryRepo = new Mock<IDeliveryRepository>();
        var orderRepo = new Mock<IOrderRepository>();
        var config = new Mock<IConfiguration>();

        deliveryRepo
            .Setup(r => r.GetDetailByIdAsync(5))
            .ReturnsAsync(new DeliveryDetailRecord
            {
                Id = 5,
                Status = "pending",
                Origin = "Warehouse A",
                Destination = "Customer Home",
                CustomerName = "John Doe",
                CustomerEmail = "john@example.com",
                CustomerRegisteredSince = new DateTime(2026, 5, 1),
                DriverName = "Jane Driver",
                DriverPhone = "555-1000",
                DriverPhotoUrl = "https://cdn.example.com/driver.jpg",
                DriverVerified = true
            });

        var service = new DeliveryService(deliveryRepo.Object, orderRepo.Object, config.Object);
        var result = await service.GetDetailByIdAsync(5);

        Assert.Equal(5, result.Id);
        Assert.Equal("pending", result.Status);
        Assert.Equal("John Doe", result.Client.Name);
        Assert.Equal("john@example.com", result.Client.Email);
        Assert.Equal("Jane Driver", result.DeliveryPerson.Name);
        Assert.True(result.DeliveryPerson.Verified);
    }

    [Fact]
    public async Task GetDetailByIdAsync_ThrowsNotFound_WhenDeliveryDoesNotExist()
    {
        var deliveryRepo = new Mock<IDeliveryRepository>();
        var orderRepo = new Mock<IOrderRepository>();
        var config = new Mock<IConfiguration>();

        deliveryRepo
            .Setup(r => r.GetDetailByIdAsync(404))
            .ReturnsAsync((DeliveryDetailRecord?)null);

        var service = new DeliveryService(deliveryRepo.Object, orderRepo.Object, config.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetDetailByIdAsync(404));
    }
}
