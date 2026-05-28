using Dapper;
using Microsoft.Data.SqlClient;
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

    private SqlConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Customer>> GetAllAsync(string? search)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT id         AS Id,
                   name       AS Name,
                   email      AS Email,
                   phone      AS Phone,
                   photo_url  AS PhotoUrl,
                   created_at AS CreatedAt
            FROM customers
            WHERE (@search IS NULL
                   OR name  LIKE '%' + @search + '%'
                   OR email LIKE '%' + @search + '%')";
        return await conn.QueryAsync<Customer>(sql, new { search });
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT id         AS Id,
                   name       AS Name,
                   email      AS Email,
                   phone      AS Phone,
                   photo_url  AS PhotoUrl,
                   created_at AS CreatedAt
            FROM customers
            WHERE id = @id";
        return await conn.QueryFirstOrDefaultAsync<Customer>(sql, new { id });
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT id    AS Id,
                   name  AS Name,
                   email AS Email
            FROM customers
            WHERE email = @email";
        return await conn.QueryFirstOrDefaultAsync<Customer>(sql, new { email });
    }

    public async Task<int> InsertAsync(Customer customer)
    {
        using var conn = CreateConnection();
        const string sql = @"
            INSERT INTO customers (name, email, phone, password_hash, photo_url, created_at, updated_at)
            VALUES (@Name, @Email, @Phone, @PasswordHash, @PhotoUrl, @CreatedAt, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.ExecuteScalarAsync<int>(sql, customer);
    }

    public async Task UpdateAsync(Customer customer)
    {
        using var conn = CreateConnection();
        const string sql = @"
            UPDATE customers
            SET name = @Name, phone = @Phone, updated_at = GETDATE()
            WHERE id = @Id";
        await conn.ExecuteAsync(sql, customer);
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = "DELETE FROM customers WHERE id = @id";
        await conn.ExecuteAsync(sql, new { id });
    }

    public async Task<bool> HasActiveDeliveriesAsync(int customerId)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT COUNT(*) FROM deliveries
            WHERE customer_id = @customerId
              AND status NOT IN ('delivered', 'cancelled')";
        var count = await conn.ExecuteScalarAsync<int>(sql, new { customerId });
        return count > 0;
    }
}
