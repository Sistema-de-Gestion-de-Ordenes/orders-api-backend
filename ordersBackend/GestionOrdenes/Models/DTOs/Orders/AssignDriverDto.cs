using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models.DTOs.Orders;

public class AssignDriverDto
{
    [Required]
    public int DriverId { get; set; }
}
