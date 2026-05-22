namespace OrderManagement.Models.DTOs.Drivers;

public class DriverDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Vehicle { get; set; } = string.Empty;
    public bool Available { get; set; }
    public DateTime CreatedAt { get; set; }
}
