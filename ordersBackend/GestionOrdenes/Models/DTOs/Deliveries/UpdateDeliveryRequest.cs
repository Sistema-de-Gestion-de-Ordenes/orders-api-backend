using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models.DTOs.Deliveries;

public class UpdateDeliveryRequest
{
    [Required]
    [MinLength(5,  ErrorMessage = "Origin must be at least 5 characters.")]
    [MaxLength(200, ErrorMessage = "Origin cannot exceed 200 characters.")]
    [RegularExpression(@"^(?=.*[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ]).+$", ErrorMessage = "Origin must contain at least one letter.")]
    public string Origin { get; set; } = string.Empty;

    [Required]
    [MinLength(5,  ErrorMessage = "Destination must be at least 5 characters.")]
    [MaxLength(200, ErrorMessage = "Destination cannot exceed 200 characters.")]
    [RegularExpression(@"^(?=.*[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ]).+$", ErrorMessage = "Destination must contain at least one letter.")]
    public string Destination { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "DriverId must be a positive number.")]
    public int DriverId { get; set; }
}
