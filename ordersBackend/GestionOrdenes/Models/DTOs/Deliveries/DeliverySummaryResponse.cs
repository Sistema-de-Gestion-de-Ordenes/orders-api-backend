namespace OrderManagement.Models.DTOs.Deliveries;

public class DeliverySummaryResponse
{
    public int Id { get; set; }
    public string Client { get; set; } = string.Empty;
    public string Driver { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
