using Dapper;
using MySqlConnector;
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

    private MySqlConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Delivery>> GetAllAsync()
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT id, order_id, evidence_url, notes, delivered_at
            FROM deliveries
            ORDER BY delivered_at DESC";
        return await conn.QueryAsync<Delivery>(sql);
    }

    public async Task<Delivery?> GetByOrderIdAsync(int orderId)
    {
        using var conn = CreateConnection();
        const string sql = "SELECT id, order_id, evidence_url, notes, delivered_at FROM deliveries WHERE order_id = @orderId";
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
            INSERT INTO deliveries (order_id, evidence_url, notes, delivered_at)
            VALUES (@OrderId, @EvidenceUrl, @Notes, @DeliveredAt);
            SELECT LAST_INSERT_ID();";
        return await conn.ExecuteScalarAsync<int>(sql, delivery);
    }
}
