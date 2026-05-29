using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models.DTOs.Notifications;

public class FcmTokenRequest
{
    [Required] public string FcmToken { get; set; } = string.Empty;
}
