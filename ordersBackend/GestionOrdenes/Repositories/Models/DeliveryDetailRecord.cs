namespace OrderManagement.Repositories.Models;

public class DeliveryDetailRecord
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime CustomerRegisteredSince { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public string DriverPhone { get; set; } = string.Empty;
    public string DriverPhotoUrl { get; set; } = string.Empty;
    public bool DriverVerified { get; set; }
}
