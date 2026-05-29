using OrderManagement.Models.DTOs.Notifications;

namespace OrderManagement.Services;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponse>> GetByClientIdAsync(int clientId, bool? read);
    Task MarkAsReadAsync(int id);
    Task SendAsync(int clientId, int? deliveryId, string title, string message);
    Task UpdateFcmTokenAsync(int clientId, string token);
}
