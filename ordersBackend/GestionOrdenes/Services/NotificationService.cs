using FirebaseAdmin;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Notifications;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories.Interfaces;
using OrderManagement.Services.Interfaces;

namespace OrderManagement.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepo;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepo,
        ILogger<NotificationService> logger)
    {
        _notificationRepo = notificationRepo;
        _logger           = logger;
    }

    public async Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId)
    {
        var notifications = await _notificationRepo.GetByUserIdAsync(userId);
        return notifications.Select(ToDto);
    }

    public async Task MarkAsReadAsync(int id)
    {
        _ = await _notificationRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Notification with id {id} not found.");
        await _notificationRepo.MarkAsReadAsync(id);
    }

    public async Task SendNotificationAsync(int customerId, int? deliveryId, string title, string message)
    {
        var notification = new Notification
        {
            CustomerId = customerId,
            DeliveryId = deliveryId,
            Title      = title,
            Message    = message,
            IsRead     = false,
            CreatedAt  = DateTime.UtcNow
        };

        await _notificationRepo.InsertAsync(notification);

        try
        {
            var fcmToken = await _notificationRepo.GetFcmTokenAsync(customerId);
            if (!string.IsNullOrEmpty(fcmToken))
                await SendFcmPushAsync(fcmToken, title, message);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "FCM push failed for customer {CustomerId}", customerId);
        }

        _logger.LogInformation("Notification saved for customer {CustomerId}: {Title}", customerId, title);
    }

    public async Task SaveFcmTokenAsync(int userId, string token)
    {
        await _notificationRepo.UpdateFcmTokenAsync(userId, token);
    }

    private static async Task SendFcmPushAsync(string fcmToken, string title, string body)
    {
        if (FirebaseApp.DefaultInstance is null)
            return;

        var msg = new FirebaseAdmin.Messaging.Message
        {
            Token        = fcmToken,
            Notification = new FirebaseAdmin.Messaging.Notification { Title = title, Body = body }
        };
        await FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance.SendAsync(msg);
    }

    private static NotificationDto ToDto(Notification n) => new()
    {
        Id         = n.Id,
        CustomerId = n.CustomerId,
        DeliveryId = n.DeliveryId,
        Title     = n.Title,
        Message   = n.Message,
        IsRead    = n.IsRead,
        CreatedAt = n.CreatedAt
    };
}
