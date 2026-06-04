using Microsoft.EntityFrameworkCore;
using OrderManagement.Models.Entities;
using OrderManagement.Persistence;

namespace OrderManagement.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly OrderManagementDbContext _db;
    public NotificationRepository(OrderManagementDbContext db) => _db = db;

    public async Task<IEnumerable<Notification>> GetByClientIdAsync(int clientId, bool? read)
    {
        var query = _db.Notifications.Where(n => n.ClientId == clientId);
        if (read.HasValue)
            query = query.Where(n => n.IsRead == read.Value);
        return await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
    }

    public async Task<Notification?> GetByIdAsync(int id)
        => await _db.Notifications.FirstOrDefaultAsync(n => n.Id == id);

    public async Task<Notification> CreateAsync(Notification notification)
    {
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync();
        return notification;
    }

    public async Task SaveAsync() => await _db.SaveChangesAsync();

    public async Task<string?> GetFcmTokenAsync(int clientId)
        => await _db.Clients
            .Where(c => c.Id == clientId)
            .Select(c => c.FcmToken)
            .FirstOrDefaultAsync();

    public async Task UpdateFcmTokenAsync(int clientId, string token)
    {
        var client = await _db.Clients.FindAsync(clientId);
        if (client is not null)
        {
            client.FcmToken = token;
            await _db.SaveChangesAsync();
        }
    }

    public async Task UpdateUserFcmTokenAsync(int userId, string token)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is not null)
        {
            user.FcmToken = token;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<string>> GetAllAdminFcmTokensAsync()
        => await _db.Users
            .Where(u => u.Role == "admin" && u.FcmToken != null && u.FcmToken != string.Empty)
            .Select(u => u.FcmToken!)
            .ToListAsync();
}
