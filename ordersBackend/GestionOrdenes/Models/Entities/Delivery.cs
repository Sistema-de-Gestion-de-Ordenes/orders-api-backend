namespace OrderManagement.Models.Entities;

public class Delivery
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string EvidenceUrl { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime DeliveredAt { get; set; }
}
