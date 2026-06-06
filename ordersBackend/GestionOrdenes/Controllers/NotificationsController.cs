using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Notifications;
using OrderManagement.Services;

namespace OrderManagement.Controllers;

[ApiController]
[Route("notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    public NotificationsController(INotificationService notificationService) => _notificationService = notificationService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? read)
    {
        try
        {
            var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _notificationService.GetByClientIdAsync(clientId, read);
            return Ok(result);
        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor" }); }
    }

    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        try
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok(new { message = "Notificación marcada como leída" });
        }
        catch (NotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception)            { return StatusCode(500, new { error = "Error interno del servidor" }); }
    }

    [HttpPost("fcm-token")]
    public async Task<IActionResult> SaveFcmToken([FromBody] FcmTokenRequest dto)
    {
        try
        {
            var id   = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role == "admin")
                await _notificationService.UpdateUserFcmTokenAsync(id, dto.FcmToken);
            else
                await _notificationService.UpdateFcmTokenAsync(id, dto.FcmToken);

            return Ok(new { message = "Token FCM actualizado correctamente" });
        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor" }); }
    }
}
