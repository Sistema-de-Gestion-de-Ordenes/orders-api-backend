using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models.DTOs.Notifications;

public class FcmTokenDto
{
    [Required]
    public string Token { get; set; } = string.Empty;
}
