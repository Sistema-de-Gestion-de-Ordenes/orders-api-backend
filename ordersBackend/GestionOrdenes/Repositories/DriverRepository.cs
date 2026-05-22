using Dapper;
using MySqlConnector;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories.Interfaces;

namespace OrderManagement.Repositories;

public class DriverRepository : IDriverRepository
{
    private readonly string _connectionString;

    public DriverRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private MySqlConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Driver>> GetAllAsync(string? search)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT d.id, d.user_id, u.name, u.email, d.phone, d.vehicle, d.available, d.created_at
            FROM drivers d
            INNER JOIN users u ON u.id = d.user_id
            WHERE (@search IS NULL OR u.name LIKE CONCAT('%', @search, '%'))";
        return await conn.QueryAsync<Driver>(sql, new { search });
    }

    public async Task<Driver?> GetByIdAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT d.id, d.user_id, u.name, u.email, d.phone, d.vehicle, d.available, d.created_at
            FROM drivers d
            INNER JOIN users u ON u.id = d.user_id
            WHERE d.id = @id";
        return await conn.QueryFirstOrDefaultAsync<Driver>(sql, new { id });
    }

    public async Task<Driver?> GetByEmailAsync(string email)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT d.id, d.user_id, u.name, u.email, d.phone, d.vehicle, d.available, d.created_at
            FROM drivers d
            INNER JOIN users u ON u.id = d.user_id
            WHERE u.email = @email";
        return await conn.QueryFirstOrDefaultAsync<Driver>(sql, new { email });
    }

    public async Task<int> InsertAsync(Driver driver)
    {
        using var conn = CreateConnection();
        const string sql = @"
            INSERT INTO drivers (user_id, phone, vehicle, available, created_at)
            VALUES (@UserId, @Phone, @Vehicle, @Available, @CreatedAt);
            SELECT LAST_INSERT_ID();";
        return await conn.ExecuteScalarAsync<int>(sql, driver);
    }

    public async Task UpdateAsync(Driver driver)
    {
        using var conn = CreateConnection();
        const string sql = @"
            UPDATE drivers SET phone = @Phone, vehicle = @Vehicle WHERE id = @Id;
            UPDATE users   SET name  = @Name                       WHERE id = @UserId;";
        await conn.ExecuteAsync(sql, driver);
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = @"
            DELETE d, u FROM drivers d
            INNER JOIN users u ON u.id = d.user_id
            WHERE d.id = @id";
        await conn.ExecuteAsync(sql, new { id });
    }

    public async Task<bool> HasOrdersInProgressAsync(int driverId)
    {
        using var conn = CreateConnection();
        const string sql = "SELECT COUNT(*) FROM orders WHERE driver_id = @driverId AND status = 'InProgress'";
        var count = await conn.ExecuteScalarAsync<int>(sql, new { driverId });
        return count > 0;
    }

    public async Task SetAvailableAsync(int id, bool available)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE drivers SET available = @available WHERE id = @id",
            new { id, available });
    }
}
