using Dapper;
using MySqlConnector;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories.Interfaces;

namespace OrderManagement.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly string _connectionString;

    public NotificationRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private MySqlConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT id, user_id, order_id, title, message, is_read, created_at
            FROM notifications
            WHERE user_id = @userId
            ORDER BY created_at DESC";
        return await conn.QueryAsync<Notification>(sql, new { userId });
    }

    public async Task<Notification?> GetByIdAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = "SELECT id, user_id, order_id, title, message, is_read, created_at FROM notifications WHERE id = @id";
        return await conn.QueryFirstOrDefaultAsync<Notification>(sql, new { id });
    }

    public async Task<int> InsertAsync(Notification notification)
    {
        using var conn = CreateConnection();
        const string sql = @"
            INSERT INTO notifications (user_id, order_id, title, message, is_read, created_at)
            VALUES (@UserId, @OrderId, @Title, @Message, @IsRead, @CreatedAt);
            SELECT LAST_INSERT_ID();";
        return await conn.ExecuteScalarAsync<int>(sql, notification);
    }

    public async Task MarkAsReadAsync(int id)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync("UPDATE notifications SET is_read = TRUE WHERE id = @id", new { id });
    }

    public async Task<string?> GetFcmTokenAsync(int userId)
    {
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<string?>(
            "SELECT fcm_token FROM users WHERE id = @userId",
            new { userId });
    }

    public async Task UpdateFcmTokenAsync(int userId, string token)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE users SET fcm_token = @token WHERE id = @userId",
            new { userId, token });
    }
}
