using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Auth;
using OrderManagement.Repositories;

namespace OrderManagement.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepo;
    private readonly IConfiguration  _config;

    public AuthService(IAuthRepository authRepo, IConfiguration config)
    {
        _authRepo = authRepo;
        _config   = config;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new DomainException("Email and password are required.");

        var client = await _authRepo.GetByEmailAsync(dto.Email)
            ?? throw new DomainException("Invalid credentials.", 401);

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, client.PasswordHash))
            throw new DomainException("Invalid credentials.", 401);

        var jwtSettings = _config.GetSection("JwtSettings");
        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var expiration  = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("JwtSettings:ExpirationMinutes"));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
            new Claim(ClaimTypes.Email,          client.Email),
            new Claim(ClaimTypes.Name,           client.Name),
            new Claim(ClaimTypes.Role,           "client")
        };

        var token = new JwtSecurityToken(
            issuer:             jwtSettings["Issuer"],
            audience:           jwtSettings["Audience"],
            claims:             claims,
            expires:            expiration,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new LoginResponse { Token = new JwtSecurityTokenHandler().WriteToken(token) };
    }
}
