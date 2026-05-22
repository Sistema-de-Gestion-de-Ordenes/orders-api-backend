using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models.DTOs.Orders;

public class ChangeStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
