using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Drivers;
using OrderManagement.Services.Interfaces;

namespace OrderManagement.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class DriversController : ControllerBase
{
    private readonly IDriverService _driverService;

    public DriversController(IDriverService driverService)
    {
        _driverService = driverService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var result = await _driverService.GetAllAsync(search);
        return Ok(ApiResponse<IEnumerable<DriverDto>>.SuccessResult(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDriverDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid request data."));

        var result = await _driverService.CreateAsync(dto);
        return StatusCode(201, ApiResponse<DriverDto>.SuccessResult(result, "Driver created."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDriverDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid request data."));

        var result = await _driverService.UpdateAsync(id, dto);
        return Ok(ApiResponse<DriverDto>.SuccessResult(result, "Driver updated."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _driverService.DeleteAsync(id);
        return Ok(ApiResponse<object>.SuccessResult(null, "Driver deleted."));
    }
}
