using OrderManagement.Models.DTOs.Auth;

namespace OrderManagement.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest dto);
}
