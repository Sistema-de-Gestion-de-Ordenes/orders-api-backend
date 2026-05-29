namespace OrderManagement.Models.Entities;

public class Delivery
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int DriverId { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Client? Client { get; set; }
    public virtual Driver? Driver { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
