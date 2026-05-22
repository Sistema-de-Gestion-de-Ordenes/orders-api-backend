using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models.DTOs.Orders;

public class CreateOrderDto
{
    [Required]
    public int CustomerId { get; set; }

    [Required, MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string DeliveryAddress { get; set; } = string.Empty;

    [Required, Range(0.01, double.MaxValue)]
    public decimal Total { get; set; }
}
