using Microsoft.AspNetCore.Mvc;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Auth;
using OrderManagement.Services;

namespace OrderManagement.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }
        catch (DomainException ex) when (ex.StatusCode == 401) { return Unauthorized(new { error = ex.Message }); }
        catch (DomainException ex) { return BadRequest(new { error = ex.Message }); }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor" }); }
    }
}
