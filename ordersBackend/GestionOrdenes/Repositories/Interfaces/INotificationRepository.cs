using OrderManagement.Models.Entities;

namespace OrderManagement.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);
    Task<Notification?> GetByIdAsync(int id);
    Task<int> InsertAsync(Notification notification);
    Task MarkAsReadAsync(int id);
    Task<string?> GetFcmTokenAsync(int userId);
    Task UpdateFcmTokenAsync(int userId, string token);
}
