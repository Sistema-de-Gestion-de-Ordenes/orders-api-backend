using Dapper;
using Microsoft.Data.SqlClient;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories.Interfaces;
using OrderManagement.Repositories.Models;

namespace OrderManagement.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly string _connectionString;

    public DeliveryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private SqlConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Delivery>> GetAllAsync()
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT
                d.id                              AS id,
                d.id                              AS order_id,
                CAST(d.origin AS nvarchar(max))   AS evidence_url,
                CAST(d.destination AS nvarchar(max)) AS notes,
                d.created_at                      AS delivered_at
            FROM deliveries
            ORDER BY d.created_at DESC";
        return await conn.QueryAsync<Delivery>(sql);
    }

    public async Task<Delivery?> GetByOrderIdAsync(int orderId)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT
                d.id                              AS id,
                d.id                              AS order_id,
                CAST(d.origin AS nvarchar(max))   AS evidence_url,
                CAST(d.destination AS nvarchar(max)) AS notes,
                d.created_at                      AS delivered_at
            FROM deliveries d
            WHERE d.id = @orderId";
        return await conn.QueryFirstOrDefaultAsync<Delivery>(sql, new { orderId });
    }

    public async Task<DeliveryDetailRecord?> GetDetailByIdAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT
                d.id                    AS Id,
                d.status                AS Status,
                d.origin                AS Origin,
                d.destination           AS Destination,
                c.user_id               AS CustomerUserId,
                c.name                  AS CustomerName,
                c.email                 AS CustomerEmail,
                c.created_at            AS CustomerRegisteredSince,
                dr.name                 AS DriverName,
                dr.phone                AS DriverPhone,
                COALESCE(dr.photo_url, '') AS DriverPhotoUrl,
                dr.is_verified          AS DriverVerified
            FROM deliveries d
            INNER JOIN customers c ON c.id = d.customer_id
            INNER JOIN drivers dr ON dr.id = d.driver_id
            WHERE d.id = @id";

        return await conn.QueryFirstOrDefaultAsync<DeliveryDetailRecord>(sql, new { id });
    }

    public async Task<int> InsertAsync(Delivery delivery)
    {
        using var conn = CreateConnection();
        const string sql = @"
            INSERT INTO deliveries (customer_id, driver_id, origin, destination, status, created_at, updated_at)
            VALUES (1, 1, @EvidenceUrl, @Notes, 'pending', @DeliveredAt, @DeliveredAt);
            SELECT CAST(SCOPE_IDENTITY() as int);";
        return await conn.ExecuteScalarAsync<int>(sql, delivery);
    }

    public async Task UpdateStatusAsync(int id, string newStatus)
    {
        using var conn = CreateConnection();
        const string sql = "UPDATE deliveries SET status = @newStatus, updated_at = GETUTCDATE() WHERE id = @id";
        await conn.ExecuteAsync(sql, new { id, newStatus });
    }
}
