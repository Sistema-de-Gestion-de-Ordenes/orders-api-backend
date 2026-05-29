using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models.DTOs.Deliveries;

public class UpdateDeliveryRequest
{
    [Required] public string Origin { get; set; } = string.Empty;
    [Required] public string Destination { get; set; } = string.Empty;
    [Required] public int DriverId { get; set; }
}
