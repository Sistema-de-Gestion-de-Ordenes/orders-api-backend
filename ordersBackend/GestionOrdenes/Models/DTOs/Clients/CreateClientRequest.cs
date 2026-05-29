using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models.DTOs.Clients;

public class CreateClientRequest
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Email { get; set; } = string.Empty;
    [Required] public string Phone { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public IFormFile Photo { get; set; } = null!;
}
