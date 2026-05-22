namespace OrderManagement.Models.Entities;

public class Driver
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Vehicle { get; set; } = string.Empty;
    public bool Available { get; set; }
    public DateTime CreatedAt { get; set; }
}
