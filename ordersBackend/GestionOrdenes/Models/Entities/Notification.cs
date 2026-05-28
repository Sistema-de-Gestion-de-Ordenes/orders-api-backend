namespace OrderManagement.Models.Entities;

public class Notification
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int? DeliveryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
