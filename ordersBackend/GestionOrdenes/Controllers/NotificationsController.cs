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
        catch (Exception) { return StatusCode(500, new { error = "Internal server error" }); }
    }

    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        try
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok(new { message = "Notification marked as read" });
        }
        catch (NotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception)            { return StatusCode(500, new { error = "Internal server error" }); }
    }

    [HttpPost("fcm-token")]
    public async Task<IActionResult> SaveFcmToken([FromBody] FcmTokenRequest dto)
    {
        try
        {
            var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _notificationService.UpdateFcmTokenAsync(clientId, dto.FcmToken);
            return Ok(new { message = "FCM token updated successfully" });
        }
        catch (Exception) { return StatusCode(500, new { error = "Internal server error" }); }
    }
}
