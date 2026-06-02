namespace OrderManagement.Models.DTOs.Deliveries;

public class DeliveryDetailResponse
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public ClientDetailDto Client { get; set; } = new();
    public DriverDetailDto Driver { get; set; } = new();
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
}

public class ClientDetailDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RegisteredSince { get; set; } = string.Empty;
}

public class DriverDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public bool Verified { get; set; }
}
