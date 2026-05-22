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

    public async Task SendNotificationAsync(int userId, int? orderId, string title, string message)
    {
        var notification = new Notification
        {
            UserId    = userId,
            OrderId   = orderId,
            Title     = title,
            Message   = message,
            IsRead    = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepo.InsertAsync(notification);

        // TODO: Send push notification via FCM (pending issue)
        // var fcmToken = await _notificationRepo.GetFcmTokenAsync(userId);
        // if (!string.IsNullOrEmpty(fcmToken))
        //     await SendFcmPushAsync(fcmToken, title, message);

        _logger.LogInformation("Notification saved for user {UserId}: {Title}", userId, title);
    }

    public async Task SaveFcmTokenAsync(int userId, string token)
    {
        await _notificationRepo.UpdateFcmTokenAsync(userId, token);
    }

    // TODO: Implement FCM push delivery (pending issue)
    // private async Task SendFcmPushAsync(string fcmToken, string title, string message)
    // {
    //     // Use FirebaseAdmin SDK or HTTP call to FCM v1 API
    // }

    private static NotificationDto ToDto(Notification n) => new()
    {
        Id        = n.Id,
        UserId    = n.UserId,
        OrderId   = n.OrderId,
        Title     = n.Title,
        Message   = n.Message,
        IsRead    = n.IsRead,
        CreatedAt = n.CreatedAt
    };
}
