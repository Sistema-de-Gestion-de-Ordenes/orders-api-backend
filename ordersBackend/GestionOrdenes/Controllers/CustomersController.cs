using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Customers;
using OrderManagement.Services.Interfaces;

namespace OrderManagement.Controllers;

[ApiController]
[Route("clients")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var result = await _customerService.GetAllAsync(search);
        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResult(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _customerService.GetByIdAsync(id);
        return Ok(ApiResponse<CustomerDto>.SuccessResult(result));
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromForm] CreateCustomerDto dto)
    {
        var result = await _customerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<CustomerDto>.SuccessResult(result, "Client created."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dto)
    {
        var result = await _customerService.UpdateAsync(id, dto);
        return Ok(ApiResponse<CustomerDto>.SuccessResult(result, "Client updated."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _customerService.DeleteAsync(id);
        return Ok(ApiResponse<object>.SuccessResult(null, "Client deleted."));
    }
}
