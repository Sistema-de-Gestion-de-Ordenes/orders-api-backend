using OrderManagement.Models.DTOs.Auth;

namespace OrderManagement.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    Task LogoutAsync(int userId);
}
