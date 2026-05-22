using OrderManagement.Models.DTOs.Orders;

namespace OrderManagement.Services.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllAsync(string? search);
    Task<OrderDto> GetByIdAsync(int id);
    Task<OrderDto> CreateAsync(CreateOrderDto dto);
    Task<OrderDto> UpdateAsync(int id, UpdateOrderDto dto);
    Task DeleteAsync(int id);
    Task<OrderDto> ChangeStatusAsync(int id, ChangeStatusDto dto);
    Task<OrderDto> AssignDriverAsync(int id, AssignDriverDto dto);
}
