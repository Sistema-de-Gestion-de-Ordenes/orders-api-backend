using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models.DTOs.Customers;

public class UpdateCustomerDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Phone]
    public string Phone { get; set; } = string.Empty;
}
