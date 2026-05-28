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
            SELECT id, customer_id, delivery_id, title, message, is_read, created_at
            FROM notifications
            WHERE customer_id = @userId
            ORDER BY created_at DESC";
        return await conn.QueryAsync<Notification>(sql, new { userId });
    }

    public async Task<Notification?> GetByIdAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = "SELECT id, customer_id, delivery_id, title, message, is_read, created_at FROM notifications WHERE id = @id";
        return await conn.QueryFirstOrDefaultAsync<Notification>(sql, new { id });
    }

    public async Task<int> InsertAsync(Notification notification)
    {
        using var conn = CreateConnection();
        const string sql = @"
            INSERT INTO notifications (customer_id, delivery_id, title, message, is_read, created_at)
            VALUES (@CustomerId, @DeliveryId, @Title, @Message, @IsRead, @CreatedAt);
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
            "SELECT fcm_token FROM customers WHERE id = @userId",
            new { userId });
    }

    public async Task UpdateFcmTokenAsync(int userId, string token)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE customers SET fcm_token = @token WHERE id = @userId",
            new { userId, token });
    }
}
