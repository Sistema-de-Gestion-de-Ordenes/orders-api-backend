using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models.DTOs.Drivers;

public class UpdateDriverDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Vehicle { get; set; } = string.Empty;
}
