namespace OrderManagement.Models.DTOs.Deliveries;

public class DeliveryResponse
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int DriverId { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
