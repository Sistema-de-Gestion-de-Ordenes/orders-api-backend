using OrderManagement.Models.Entities;

namespace OrderManagement.Repositories;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetByClientIdAsync(int clientId, bool? read);
    Task<Notification?> GetByIdAsync(int id);
    Task<Notification> CreateAsync(Notification notification);
    Task SaveAsync();
    Task<string?> GetFcmTokenAsync(int clientId);
    Task UpdateFcmTokenAsync(int clientId, string token);
}
