namespace OrderManagement.Models.DTOs.Deliveries;

public class DeliveryHistoryResponse
{
    public int Id { get; set; }
    public string Client { get; set; } = string.Empty;
    public string? ClientPhotoUrl { get; set; }
    public string Driver { get; set; } = string.Empty;
    public string? DriverPhotoUrl { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
