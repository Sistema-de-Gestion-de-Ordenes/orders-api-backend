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
            throw new DomainException("El correo y la contraseña son obligatorios.");

        var client = await _authRepo.GetByEmailAsync(dto.Email);
        if (client is not null)
        {
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, client.PasswordHash))
                throw new DomainException("Credenciales inválidas.", 401);

            return new LoginResponse { Token = GenerateToken(client.Id, client.Email, client.Name, "client") };
        }

        var user = await _authRepo.GetUserByEmailAsync(dto.Email);
        if (user is not null)
        {
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new DomainException("Credenciales inválidas.", 401);

            return new LoginResponse { Token = GenerateToken(user.Id, user.Email, user.Name, user.Role) };
        }

        throw new DomainException("Credenciales inválidas.", 401);
    }

    private string GenerateToken(int id, string email, string name, string role)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var expiration  = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("JwtSettings:ExpirationMinutes"));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Email,          email),
            new Claim(ClaimTypes.Name,           name),
            new Claim(ClaimTypes.Role,           role)
        };

        var token = new JwtSecurityToken(
            issuer:             jwtSettings["Issuer"],
            audience:           jwtSettings["Audience"],
            claims:             claims,
            expires:            expiration,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
