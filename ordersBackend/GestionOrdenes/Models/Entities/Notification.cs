namespace OrderManagement.Models.Entities;

public class Notification
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int? DeliveryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Client? Client { get; set; }
    public virtual Delivery? Delivery { get; set; }
}
