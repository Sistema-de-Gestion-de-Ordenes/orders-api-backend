namespace OrderManagement.Models.Entities;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? FcmToken { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
