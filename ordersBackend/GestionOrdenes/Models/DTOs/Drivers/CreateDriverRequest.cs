using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models.DTOs.Drivers;

public class CreateDriverRequest
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Vehicle { get; set; } = string.Empty;
    [Required] public string Plates { get; set; } = string.Empty;
    [Required] public string Phone { get; set; } = string.Empty;
    [Required] public IFormFile Photo { get; set; } = null!;
}
