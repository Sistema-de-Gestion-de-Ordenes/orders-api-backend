using OrderManagement.Common;
using OrderManagement.Models.DTOs.Deliveries;
using OrderManagement.Models.Entities;
using OrderManagement.Models.Enums;
using OrderManagement.Repositories.Interfaces;
using OrderManagement.Services.Interfaces;

namespace OrderManagement.Services;

public class DeliveryService : IDeliveryService
{
    private readonly IDeliveryRepository _deliveryRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly IConfiguration _config;

    public DeliveryService(
        IDeliveryRepository deliveryRepo,
        IOrderRepository orderRepo,
        IConfiguration config)
    {
        _deliveryRepo = deliveryRepo;
        _orderRepo    = orderRepo;
        _config       = config;
    }

    public async Task<IEnumerable<DeliveryDto>> GetAllAsync()
    {
        var deliveries = await _deliveryRepo.GetAllAsync();
        return deliveries.Select(ToDto);
    }

    public async Task<DeliveryDto> UploadEvidenceAsync(int orderId, UploadEvidenceDto dto)
    {
        var order = await _orderRepo.GetByIdAsync(orderId)
            ?? throw new NotFoundException($"Order with id {orderId} not found.");

        if (order.Status == OrderStatus.Delivered)
            throw new DomainException("This order has already been marked as delivered.");

        if (order.Status == OrderStatus.Cancelled)
            throw new DomainException("Cannot register a delivery for a cancelled order.");

        // TODO: Upload image to Cloudinary (pending issue)
        // var cloudinary = BuildCloudinaryClient();
        // var uploadResult = await UploadToCloudinary(cloudinary, dto.Image);
        // var evidenceUrl = uploadResult.SecureUrl.ToString();

        var evidenceUrl = $"https://placeholder.cloudinary.com/{Guid.NewGuid()}.jpg";

        var delivery = new Delivery
        {
            OrderId     = orderId,
            EvidenceUrl = evidenceUrl,
            Notes       = dto.Notes,
            DeliveredAt = DateTime.UtcNow
        };

        delivery.Id = await _deliveryRepo.InsertAsync(delivery);
        await _orderRepo.ChangeStatusAsync(orderId, OrderStatus.Delivered);

        return ToDto(delivery);
    }

    // TODO: Implement Cloudinary integration (pending issue)
    // private Cloudinary BuildCloudinaryClient()
    // {
    //     var account = new Account(
    //         _config["Cloudinary:CloudName"],
    //         _config["Cloudinary:ApiKey"],
    //         _config["Cloudinary:ApiSecret"]);
    //     return new Cloudinary(account);
    // }

    // private static async Task<ImageUploadResult> UploadToCloudinary(Cloudinary cloudinary, IFormFile file)
    // {
    //     using var stream = file.OpenReadStream();
    //     var uploadParams = new ImageUploadParams
    //     {
    //         File   = new FileDescription(file.FileName, stream),
    //         Folder = "deliveries"
    //     };
    //     return await cloudinary.UploadAsync(uploadParams);
    // }

    private static DeliveryDto ToDto(Delivery d) => new()
    {
        Id          = d.Id,
        OrderId     = d.OrderId,
        OrderNumber = string.Empty,
        EvidenceUrl = d.EvidenceUrl,
        Notes       = d.Notes,
        DeliveredAt = d.DeliveredAt
    };
}
