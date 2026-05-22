using Dapper;
using MySqlConnector;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories.Interfaces;

namespace OrderManagement.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly string _connectionString;

    public CustomerRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private MySqlConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Customer>> GetAllAsync(string? search)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT c.id, c.user_id, u.name, u.email, c.phone, c.address, c.created_at
            FROM customers c
            INNER JOIN users u ON u.id = c.user_id
            WHERE (@search IS NULL
                   OR u.name  LIKE CONCAT('%', @search, '%')
                   OR u.email LIKE CONCAT('%', @search, '%'))";
        return await conn.QueryAsync<Customer>(sql, new { search });
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT c.id, c.user_id, u.name, u.email, c.phone, c.address, c.created_at
            FROM customers c
            INNER JOIN users u ON u.id = c.user_id
            WHERE c.id = @id";
        return await conn.QueryFirstOrDefaultAsync<Customer>(sql, new { id });
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT c.id, c.user_id, u.name, u.email, c.phone, c.address, c.created_at
            FROM customers c
            INNER JOIN users u ON u.id = c.user_id
            WHERE u.email = @email";
        return await conn.QueryFirstOrDefaultAsync<Customer>(sql, new { email });
    }

    public async Task<int> InsertAsync(Customer customer)
    {
        using var conn = CreateConnection();
        const string sql = @"
            INSERT INTO customers (user_id, phone, address, created_at)
            VALUES (@UserId, @Phone, @Address, @CreatedAt);
            SELECT LAST_INSERT_ID();";
        return await conn.ExecuteScalarAsync<int>(sql, customer);
    }

    public async Task UpdateAsync(Customer customer)
    {
        using var conn = CreateConnection();
        const string sql = @"
            UPDATE customers SET phone = @Phone, address = @Address WHERE id = @Id;
            UPDATE users    SET name  = @Name                         WHERE id = @UserId;";
        await conn.ExecuteAsync(sql, customer);
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = @"
            DELETE c, u FROM customers c
            INNER JOIN users u ON u.id = c.user_id
            WHERE c.id = @id";
        await conn.ExecuteAsync(sql, new { id });
    }

    public async Task<bool> HasActiveOrdersAsync(int customerId)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT COUNT(*) FROM orders
            WHERE customer_id = @customerId
              AND status NOT IN ('Delivered', 'Cancelled')";
        var count = await conn.ExecuteScalarAsync<int>(sql, new { customerId });
        return count > 0;
    }
}
