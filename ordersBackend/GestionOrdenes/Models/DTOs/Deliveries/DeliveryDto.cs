namespace OrderManagement.Models.DTOs.Deliveries;

public class DeliveryDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string EvidenceUrl { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime DeliveredAt { get; set; }
}
