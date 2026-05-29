using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models.DTOs.Deliveries;

public class CreateDeliveryRequest
{
    [Required] public int ClientId { get; set; }
    [Required] public int DriverId { get; set; }
    [Required] public string Origin { get; set; } = string.Empty;
    [Required] public string Destination { get; set; } = string.Empty;
}
