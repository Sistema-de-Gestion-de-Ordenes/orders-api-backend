using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models.DTOs.Drivers;

public class CreateDriverDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Vehicle { get; set; } = string.Empty;
}
