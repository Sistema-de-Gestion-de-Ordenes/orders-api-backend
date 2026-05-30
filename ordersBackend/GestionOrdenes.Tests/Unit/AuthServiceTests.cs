using Microsoft.Extensions.Configuration;
using Moq;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Auth;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories;
using OrderManagement.Services;

namespace GestionOrdenes.Tests.Unit;

public class AuthServiceTests
{
    private static IConfiguration BuildJwtConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"]         = "super-secret-key-that-is-at-least-32-chars!!",
                ["JwtSettings:Issuer"]            = "TestIssuer",
                ["JwtSettings:Audience"]          = "TestAudience",
                ["JwtSettings:ExpirationMinutes"] = "60"
            })
            .Build();

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsLoginResponse()
    {
        var hash   = BCrypt.Net.BCrypt.HashPassword("secret123");
        var client = new Client { Id = 1, Name = "Ana", Email = "ana@test.com", Phone = "88881111", PasswordHash = hash };

        var repo = new Mock<IAuthRepository>();
        repo.Setup(r => r.GetByEmailAsync("ana@test.com")).ReturnsAsync(client);

        var result = await new AuthService(repo.Object, BuildJwtConfig())
            .LoginAsync(new LoginRequest { Email = "ana@test.com", Password = "secret123" });

        Assert.False(string.IsNullOrEmpty(result.Token));
    }

    [Fact]
    public async Task LoginAsync_EmailNotFound_ThrowsDomainException401()
    {
        var repo = new Mock<IAuthRepository>();
        repo.Setup(r => r.GetByEmailAsync("notfound@test.com")).ReturnsAsync((Client?)null);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            new AuthService(repo.Object, BuildJwtConfig())
                .LoginAsync(new LoginRequest { Email = "notfound@test.com", Password = "anypassword" }));

        Assert.Equal(401, ex.StatusCode);
        Assert.Equal("Invalid credentials.", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ThrowsDomainException401()
    {
        var hash   = BCrypt.Net.BCrypt.HashPassword("correct-password");
        var client = new Client { Id = 2, Name = "Bob", Email = "bob@test.com", Phone = "77772222", PasswordHash = hash };

        var repo = new Mock<IAuthRepository>();
        repo.Setup(r => r.GetByEmailAsync("bob@test.com")).ReturnsAsync(client);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            new AuthService(repo.Object, BuildJwtConfig())
                .LoginAsync(new LoginRequest { Email = "bob@test.com", Password = "wrong-password" }));

        Assert.Equal(401, ex.StatusCode);
        Assert.Equal("Invalid credentials.", ex.Message);
    }
}
