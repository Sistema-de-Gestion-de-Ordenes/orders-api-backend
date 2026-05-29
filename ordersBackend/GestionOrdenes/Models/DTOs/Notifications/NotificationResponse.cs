namespace OrderManagement.Models.DTOs.Notifications;

public class NotificationResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Read { get; set; }
    public int? DeliveryId { get; set; }
    public DateTime CreatedAt { get; set; }
}
