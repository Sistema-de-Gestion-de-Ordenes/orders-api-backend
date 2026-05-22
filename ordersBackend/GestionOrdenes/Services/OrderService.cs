using OrderManagement.Common;
using OrderManagement.Models.DTOs.Orders;
using OrderManagement.Models.Entities;
using OrderManagement.Models.Enums;
using OrderManagement.Repositories.Interfaces;
using OrderManagement.Services.Interfaces;

namespace OrderManagement.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly IDriverRepository _driverRepo;
    private readonly INotificationService _notificationService;

    public OrderService(
        IOrderRepository orderRepo,
        ICustomerRepository customerRepo,
        IDriverRepository driverRepo,
        INotificationService notificationService)
    {
        _orderRepo           = orderRepo;
        _customerRepo        = customerRepo;
        _driverRepo          = driverRepo;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync(string? search)
    {
        var orders = await _orderRepo.GetAllAsync(search);
        return orders.Select(ToDto);
    }

    public async Task<OrderDto> GetByIdAsync(int id)
    {
        var order = await _orderRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Order with id {id} not found.");
        return ToDto(order);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
    {
        _ = await _customerRepo.GetByIdAsync(dto.CustomerId)
            ?? throw new NotFoundException($"Customer with id {dto.CustomerId} not found.");

        var order = new Order
        {
            OrderNumber     = await _orderRepo.GenerateOrderNumberAsync(),
            CustomerId      = dto.CustomerId,
            Description     = dto.Description,
            DeliveryAddress = dto.DeliveryAddress,
            Total           = dto.Total,
            Status          = OrderStatus.Pending,
            CreatedAt       = DateTime.UtcNow
        };

        order.Id = await _orderRepo.InsertAsync(order);
        return ToDto(order);
    }

    public async Task<OrderDto> UpdateAsync(int id, UpdateOrderDto dto)
    {
        var order = await _orderRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Order with id {id} not found.");

        if (order.Status is OrderStatus.Delivered or OrderStatus.Cancelled)
            throw new DomainException("Cannot modify an order that has been delivered or cancelled.");

        order.Description     = dto.Description;
        order.DeliveryAddress = dto.DeliveryAddress;
        order.Total           = dto.Total;
        order.UpdatedAt       = DateTime.UtcNow;

        await _orderRepo.UpdateAsync(order);
        return ToDto(order);
    }

    public async Task DeleteAsync(int id)
    {
        var order = await _orderRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Order with id {id} not found.");

        if (order.Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be deleted.");

        await _orderRepo.DeleteAsync(id);
    }

    public async Task<OrderDto> ChangeStatusAsync(int id, ChangeStatusDto dto)
    {
        var order = await _orderRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Order with id {id} not found.");

        if (!Enum.TryParse<OrderStatus>(dto.Status, ignoreCase: true, out var newStatus))
            throw new DomainException($"'{dto.Status}' is not a valid order status.");

        ValidateStatusTransition(order.Status, newStatus);

        await _orderRepo.ChangeStatusAsync(id, newStatus);
        order.Status    = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        await _notificationService.SendNotificationAsync(
            order.CustomerId,
            id,
            "Order status updated",
            $"Your order {order.OrderNumber} is now: {newStatus}");

        return ToDto(order);
    }

    public async Task<OrderDto> AssignDriverAsync(int id, AssignDriverDto dto)
    {
        var order = await _orderRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Order with id {id} not found.");

        var driver = await _driverRepo.GetByIdAsync(dto.DriverId)
            ?? throw new NotFoundException($"Driver with id {dto.DriverId} not found.");

        if (!driver.Available)
            throw new DomainException("The selected driver is not available.");

        await _driverRepo.SetAvailableAsync(driver.Id, false);
        await _orderRepo.AssignDriverAsync(id, driver.Id);
        order.DriverId  = driver.Id;
        order.UpdatedAt = DateTime.UtcNow;

        await _notificationService.SendNotificationAsync(
            order.CustomerId,
            id,
            "Driver assigned",
            $"Your order {order.OrderNumber} has been assigned to {driver.Name}.");

        return ToDto(order);
    }

    private static void ValidateStatusTransition(OrderStatus current, OrderStatus next)
    {
        var allowed = new Dictionary<OrderStatus, OrderStatus[]>
        {
            [OrderStatus.Pending]    = [OrderStatus.InProgress],
            [OrderStatus.InProgress] = [OrderStatus.Delivered, OrderStatus.Cancelled]
        };

        if (!allowed.TryGetValue(current, out var validNext) || !validNext.Contains(next))
            throw new DomainException($"Transition from '{current}' to '{next}' is not allowed.");
    }

    private static OrderDto ToDto(Order o) => new()
    {
        Id              = o.Id,
        OrderNumber     = o.OrderNumber,
        CustomerId      = o.CustomerId,
        CustomerName    = string.Empty,
        DriverId        = o.DriverId,
        Description     = o.Description,
        DeliveryAddress = o.DeliveryAddress,
        Total           = o.Total,
        Status          = o.Status.ToString(),
        CreatedAt       = o.CreatedAt,
        UpdatedAt       = o.UpdatedAt
    };
}
