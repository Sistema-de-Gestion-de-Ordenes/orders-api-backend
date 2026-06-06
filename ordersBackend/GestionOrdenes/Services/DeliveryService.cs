using OrderManagement.Common;
using OrderManagement.Models.DTOs.Deliveries;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories;

namespace OrderManagement.Services;

public class DeliveryService : IDeliveryService
{
    private static readonly Dictionary<string, HashSet<string>> ValidTransitions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["pending"]    = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "in_transit" },
        ["in_transit"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "delivered", "cancelled" }
    };

    private readonly IDeliveryRepository _deliveryRepo;
    private readonly IClientRepository _clientRepo;
    private readonly IDriverRepository _driverRepo;
    private readonly INotificationService _notificationService;
    private readonly ILogger<DeliveryService> _logger;

    public DeliveryService(
        IDeliveryRepository deliveryRepo,
        IClientRepository clientRepo,
        IDriverRepository driverRepo,
        INotificationService notificationService,
        ILogger<DeliveryService> logger)
    {
        _deliveryRepo        = deliveryRepo;
        _clientRepo          = clientRepo;
        _driverRepo          = driverRepo;
        _notificationService = notificationService;
        _logger              = logger;
    }

    public async Task<IEnumerable<DeliverySummaryResponse>> GetAllAsync()
    {
        var deliveries = await _deliveryRepo.GetAllAsync();
        return deliveries.Select(d => new DeliverySummaryResponse
        {
            Id             = d.Id,
            Client         = d.Client?.Name ?? string.Empty,
            ClientPhotoUrl = d.Client?.PhotoUrl,
            Driver         = d.Driver?.Name ?? string.Empty,
            DriverPhotoUrl = d.Driver?.PhotoUrl,
            Origin         = d.Origin,
            Destination    = d.Destination,
            Status         = d.Status
        });
    }

    public async Task<DeliveryDetailResponse> GetByIdAsync(int id)
    {
        var d = await _deliveryRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"The delivery with id {id} does not exist.");

        return new DeliveryDetailResponse
        {
            Id     = d.Id,
            Status = d.Status,
            Origin = d.Origin,
            Destination = d.Destination,
            Client = new ClientDetailDto
            {
                Name            = d.Client?.Name ?? string.Empty,
                Email           = d.Client?.Email ?? string.Empty,
                RegisteredSince = d.Client?.CreatedAt.ToString("dd/MM/yyyy") ?? string.Empty,
                PhotoUrl        = d.Client?.PhotoUrl
            },
            Driver = new DriverDetailDto
            {
                Id       = d.DriverId,
                Name     = d.Driver?.Name ?? string.Empty,
                Phone    = d.Driver?.Phone ?? string.Empty,
                PhotoUrl = d.Driver?.PhotoUrl,
                Verified = d.Driver?.IsVerified ?? false
            }
        };
    }

    public async Task<DeliveryResponse> CreateAsync(CreateDeliveryRequest dto)
    {
        _ = await _clientRepo.GetByIdAsync(dto.ClientId)
            ?? throw new NotFoundException($"The client with id {dto.ClientId} does not exist.");

        _ = await _driverRepo.GetByIdAsync(dto.DriverId)
            ?? throw new NotFoundException($"The driver with id {dto.DriverId} does not exist.");

        var delivery = new Delivery
        {
            ClientId    = dto.ClientId,
            DriverId    = dto.DriverId,
            Origin      = dto.Origin,
            Destination = dto.Destination,
            Status      = "pending",
            CreatedAt   = DateTime.UtcNow,
            UpdatedAt   = DateTime.UtcNow
        };

        var created = await _deliveryRepo.CreateAsync(delivery);
        return ToResponse(created);
    }

    public async Task<DeliveryResponse> UpdateAsync(int id, UpdateDeliveryRequest dto)
    {
        var delivery = await _deliveryRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"The delivery with id {id} does not exist.");

        _ = await _driverRepo.GetByIdAsync(dto.DriverId)
            ?? throw new NotFoundException($"The driver with id {dto.DriverId} does not exist.");

        delivery.Origin      = dto.Origin;
        delivery.Destination = dto.Destination;
        delivery.DriverId    = dto.DriverId;

        var updated = await _deliveryRepo.UpdateAsync(delivery);
        return ToResponse(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var delivery = await _deliveryRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"The delivery with id {id} does not exist.");

        await _deliveryRepo.DeleteAsync(delivery);
    }

    public async Task<DeliveryStatusResponse> UpdateStatusAsync(int id, UpdateStatusRequest dto, string performedBy)
    {
        var normalizedStatus = dto.Status.ToLowerInvariant();

        var delivery = await _deliveryRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"The delivery with id {id} does not exist.");

        if (!ValidTransitions.TryGetValue(delivery.Status, out var allowed) || !allowed.Contains(normalizedStatus))
            throw new DomainException($"Cannot transition from {delivery.Status} to {normalizedStatus}.");

        var previousStatus = delivery.Status;
        delivery.Status = normalizedStatus;
        await _deliveryRepo.UpdateAsync(delivery);

        _logger.LogInformation(
            "Delivery {DeliveryId} status changed from {PreviousStatus} to {NewStatus} by {PerformedBy}",
            id, previousStatus, normalizedStatus, performedBy);

        try
        {
            await _notificationService.SendAsync(
                delivery.ClientId,
                id,
                "Delivery status updated",
                $"Delivery #{id} status has changed to: {normalizedStatus}.");

            _logger.LogInformation(
                "Notification sent for delivery {DeliveryId} to client {ClientId}",
                id, delivery.ClientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send FCM notification for delivery {DeliveryId}", id);
        }

        return new DeliveryStatusResponse { Id = id, Status = normalizedStatus };
    }

    private static DeliveryResponse ToResponse(Delivery d) => new()
    {
        Id          = d.Id,
        ClientId    = d.ClientId,
        DriverId    = d.DriverId,
        Origin      = d.Origin,
        Destination = d.Destination,
        Status      = d.Status
    };
}
