using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Auth;
using OrderManagement.Models.Entities;
using OrderManagement.Services.Interfaces;

namespace OrderManagement.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;

    public AuthService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        using var conn = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));

        var user = await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT id, name, email, password_hash, role FROM users WHERE email = @email",
            new { email = dto.Email });

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new DomainException("Invalid credentials.", 401);

        var expiration = DateTime.UtcNow.AddMinutes(
            _config.GetValue<int>("JwtSettings:ExpirationMinutes"));

        return new LoginResponseDto
        {
            Token      = GenerateToken(user, expiration),
            Expiration = expiration,
            Name       = user.Name,
            Email      = user.Email,
            Role       = user.Role.ToString()
        };
    }

    public async Task LogoutAsync(int userId)
    {
        using var conn = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
        await conn.ExecuteAsync("UPDATE users SET fcm_token = NULL WHERE id = @userId", new { userId });
    }

    private string GenerateToken(User user, DateTime expiration)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email,          user.Email),
            new Claim(ClaimTypes.Name,           user.Name),
            new Claim(ClaimTypes.Role,           user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer:            jwtSettings["Issuer"],
            audience:          jwtSettings["Audience"],
            claims:            claims,
            expires:           expiration,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
