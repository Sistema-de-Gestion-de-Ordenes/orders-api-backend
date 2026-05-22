using Dapper;
using MySqlConnector;
using OrderManagement.Models.Entities;
using OrderManagement.Models.Enums;
using OrderManagement.Repositories.Interfaces;

namespace OrderManagement.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly string _connectionString;

    public OrderRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private MySqlConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Order>> GetAllAsync(string? search)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT id, order_number, customer_id, driver_id, description,
                   delivery_address, total, status, created_at, updated_at
            FROM orders
            WHERE (@search IS NULL
                   OR order_number LIKE CONCAT('%', @search, '%')
                   OR description  LIKE CONCAT('%', @search, '%'))
            ORDER BY created_at DESC";
        return await conn.QueryAsync<Order>(sql, new { search });
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT id, order_number, customer_id, driver_id, description,
                   delivery_address, total, status, created_at, updated_at
            FROM orders WHERE id = @id";
        return await conn.QueryFirstOrDefaultAsync<Order>(sql, new { id });
    }

    public async Task<int> InsertAsync(Order order)
    {
        using var conn = CreateConnection();
        const string sql = @"
            INSERT INTO orders
                (order_number, customer_id, driver_id, description, delivery_address, total, status, created_at)
            VALUES
                (@OrderNumber, @CustomerId, @DriverId, @Description, @DeliveryAddress, @Total, @Status, @CreatedAt);
            SELECT LAST_INSERT_ID();";
        return await conn.ExecuteScalarAsync<int>(sql, new
        {
            order.OrderNumber,
            order.CustomerId,
            order.DriverId,
            order.Description,
            order.DeliveryAddress,
            order.Total,
            Status = order.Status.ToString(),
            order.CreatedAt
        });
    }

    public async Task UpdateAsync(Order order)
    {
        using var conn = CreateConnection();
        const string sql = @"
            UPDATE orders
            SET description      = @Description,
                delivery_address = @DeliveryAddress,
                total            = @Total,
                updated_at       = @UpdatedAt
            WHERE id = @Id";
        await conn.ExecuteAsync(sql, order);
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync("DELETE FROM orders WHERE id = @id", new { id });
    }

    public async Task ChangeStatusAsync(int id, OrderStatus status)
    {
        using var conn = CreateConnection();
        const string sql = "UPDATE orders SET status = @status, updated_at = @now WHERE id = @id";
        await conn.ExecuteAsync(sql, new { id, status = status.ToString(), now = DateTime.UtcNow });
    }

    public async Task AssignDriverAsync(int id, int driverId)
    {
        using var conn = CreateConnection();
        const string sql = "UPDATE orders SET driver_id = @driverId, updated_at = @now WHERE id = @id";
        await conn.ExecuteAsync(sql, new { id, driverId, now = DateTime.UtcNow });
    }

    public async Task<string> GenerateOrderNumberAsync()
    {
        using var conn = CreateConnection();
        var count = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM orders");
        return $"ORD-{(count + 1):D3}";
    }
}
