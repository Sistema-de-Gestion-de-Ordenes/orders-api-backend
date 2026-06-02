using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models.DTOs.Deliveries;

public class UpdateStatusRequest
{
    [Required] public string Status { get; set; } = string.Empty;
}
