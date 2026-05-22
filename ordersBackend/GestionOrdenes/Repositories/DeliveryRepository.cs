using Dapper;
using MySqlConnector;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories.Interfaces;

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
