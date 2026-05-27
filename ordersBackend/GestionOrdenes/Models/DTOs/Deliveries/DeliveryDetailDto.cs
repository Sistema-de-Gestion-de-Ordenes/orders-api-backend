namespace OrderManagement.Models.DTOs.Deliveries;

public class DeliveryDetailDto
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DeliveryClientDto Client { get; set; } = new();
    public DeliveryDriverDto DeliveryPerson { get; set; } = new();
}

public class DeliveryClientDto
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime RegisteredSince { get; set; }
}

public class DeliveryDriverDto
{
    public string Name { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string PhotoUrl { get; set; } = string.Empty;

    public bool Verified { get; set; }
}
