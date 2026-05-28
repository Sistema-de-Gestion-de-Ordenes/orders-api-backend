using OrderManagement.Models.DTOs.Notifications;

namespace OrderManagement.Services.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId);
    Task MarkAsReadAsync(int id);
    Task SendNotificationAsync(int customerId, int? deliveryId, string title, string message);
    Task SaveFcmTokenAsync(int userId, string token);
}
