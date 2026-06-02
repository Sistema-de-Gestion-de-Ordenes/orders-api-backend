namespace OrderManagement.Models.DTOs.Drivers;

public class DriverResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Vehicle { get; set; } = string.Empty;
    public string Plates { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
}
