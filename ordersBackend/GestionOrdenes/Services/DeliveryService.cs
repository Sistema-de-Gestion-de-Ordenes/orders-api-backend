using OrderManagement.Common;
using OrderManagement.Models.DTOs.Deliveries;
using OrderManagement.Models.Entities;
using OrderManagement.Models.Enums;
using OrderManagement.Repositories.Interfaces;
using OrderManagement.Services.Interfaces;

namespace OrderManagement.Services;

public class DeliveryService : IDeliveryService
{
    private static readonly Dictionary<string, HashSet<string>> ValidTransitions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["pending"]    = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "in_transit" },
        ["in_transit"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "delivered", "cancelled" }
    };

    private readonly IDeliveryRepository _deliveryRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly IConfiguration _config;
    private readonly INotificationService _notificationService;
    private readonly ILogger<DeliveryService> _logger;

    public DeliveryService(
        IDeliveryRepository deliveryRepo,
        IOrderRepository orderRepo,
        IConfiguration config,
        INotificationService notificationService,
        ILogger<DeliveryService> logger)
    {
        _deliveryRepo        = deliveryRepo;
        _orderRepo           = orderRepo;
        _config              = config;
        _notificationService = notificationService;
        _logger              = logger;
    }

    public async Task<IEnumerable<DeliveryDto>> GetAllAsync()
    {
        var deliveries = await _deliveryRepo.GetAllAsync();
        return deliveries.Select(ToDto);
    }

    public async Task<DeliveryDetailDto> GetDetailByIdAsync(int id)
    {
        var detail = await _deliveryRepo.GetDetailByIdAsync(id)
            ?? throw new NotFoundException($"Delivery with id {id} was not found.");

        return new DeliveryDetailDto
        {
            Id = detail.Id,
            Status = detail.Status,
            Origin = detail.Origin,
            Destination = detail.Destination,
            Client = new DeliveryClientDto
            {
                Name = detail.CustomerName,
                Email = detail.CustomerEmail,
                RegisteredSince = detail.CustomerRegisteredSince
            },
            DeliveryPerson = new DeliveryDriverDto
            {
                Name = detail.DriverName,
                Phone = detail.DriverPhone,
                PhotoUrl = detail.DriverPhotoUrl,
                Verified = detail.DriverVerified
            }
        };
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

    public async Task<DeliveryStatusResponseDto> UpdateStatusAsync(int id, string newStatus)
    {
        var normalizedStatus = newStatus.ToLowerInvariant();

        var detail = await _deliveryRepo.GetDetailByIdAsync(id)
            ?? throw new NotFoundException($"The delivery with id {id} does not exist.");

        if (!ValidTransitions.TryGetValue(detail.Status, out var allowed) || !allowed.Contains(normalizedStatus))
            throw new DomainException($"Cannot transition from {detail.Status} to {normalizedStatus}.");

        await _deliveryRepo.UpdateStatusAsync(id, normalizedStatus);

        try
        {
            await _notificationService.SendNotificationAsync(
                detail.CustomerUserId,
                id,
                "Delivery status updated",
                $"Delivery #{id} status has changed to: {normalizedStatus}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification for delivery {DeliveryId}", id);
        }

        return new DeliveryStatusResponseDto { Id = id, Status = normalizedStatus };
    }

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
