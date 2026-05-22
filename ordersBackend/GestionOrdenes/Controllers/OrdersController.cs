using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Orders;
using OrderManagement.Services.Interfaces;

namespace OrderManagement.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var result = await _orderService.GetAllAsync(search);
        return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _orderService.GetByIdAsync(id);
        return Ok(ApiResponse<OrderDto>.SuccessResult(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid request data."));

        var result = await _orderService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<OrderDto>.SuccessResult(result, "Order created."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid request data."));

        var result = await _orderService.UpdateAsync(id, dto);
        return Ok(ApiResponse<OrderDto>.SuccessResult(result, "Order updated."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _orderService.DeleteAsync(id);
        return Ok(ApiResponse<object>.SuccessResult(null, "Order deleted."));
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeStatusDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid request data."));

        var result = await _orderService.ChangeStatusAsync(id, dto);
        return Ok(ApiResponse<OrderDto>.SuccessResult(result, "Order status updated."));
    }

    [HttpPatch("{id}/assign")]
    public async Task<IActionResult> AssignDriver(int id, [FromBody] AssignDriverDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid request data."));

        var result = await _orderService.AssignDriverAsync(id, dto);
        return Ok(ApiResponse<OrderDto>.SuccessResult(result, "Driver assigned."));
    }
}
