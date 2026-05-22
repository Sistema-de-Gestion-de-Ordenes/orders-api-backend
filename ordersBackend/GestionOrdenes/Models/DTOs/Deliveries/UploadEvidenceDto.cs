using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models.DTOs.Deliveries;

public class UploadEvidenceDto
{
    [Required]
    public IFormFile Image { get; set; } = null!;

    public string? Notes { get; set; }
}
