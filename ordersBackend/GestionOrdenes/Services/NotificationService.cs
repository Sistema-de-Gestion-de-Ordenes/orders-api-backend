using FirebaseAdmin;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Notifications;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories;

namespace OrderManagement.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepo;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(INotificationRepository notificationRepo, ILogger<NotificationService> logger)
    {
        _notificationRepo = notificationRepo;
        _logger           = logger;
    }

    public async Task<IEnumerable<NotificationResponse>> GetByClientIdAsync(int clientId, bool? read)
    {
        var notifications = await _notificationRepo.GetByClientIdAsync(clientId, read);
        return notifications.Select(n => new NotificationResponse
        {
            Id         = n.Id,
            Title      = n.Title,
            Message    = n.Message,
            Read       = n.IsRead,
            DeliveryId = n.DeliveryId,
            CreatedAt  = n.CreatedAt
        });
    }

    public async Task MarkAsReadAsync(int id)
    {
        var notification = await _notificationRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Notification with id {id} not found.");

        notification.IsRead = true;
        await _notificationRepo.SaveAsync();
    }

    public async Task SendAsync(int clientId, int? deliveryId, string title, string message)
    {
        var notification = new Notification
        {
            ClientId   = clientId,
            DeliveryId = deliveryId,
            Title      = title,
            Message    = message,
            IsRead     = false,
            CreatedAt  = DateTime.UtcNow
        };

        await _notificationRepo.CreateAsync(notification);

        try
        {
            var fcmToken = await _notificationRepo.GetFcmTokenAsync(clientId);
            if (!string.IsNullOrEmpty(fcmToken))
                await SendFcmPushAsync(fcmToken, title, message);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "FCM push failed for client {ClientId}", clientId);
        }
    }

    public async Task UpdateFcmTokenAsync(int clientId, string token)
    {
        await _notificationRepo.UpdateFcmTokenAsync(clientId, token);
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
}
