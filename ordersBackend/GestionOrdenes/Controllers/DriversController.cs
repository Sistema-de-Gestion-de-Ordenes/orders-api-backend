using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Drivers;
using OrderManagement.Services;

namespace OrderManagement.Controllers;

[ApiController]
[Route("drivers")]
[Authorize]
public class DriversController : ControllerBase
{
    private readonly IDriverService _driverService;
    public DriversController(IDriverService driverService) => _driverService = driverService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _driverService.GetAllAsync();
            return Ok(result);
        }
        catch (Exception) { return StatusCode(500, new { error = "Internal server error" }); }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateDriverRequest dto)
    {
        try
        {
            var result = await _driverService.CreateAsync(dto);
            return StatusCode(201, result);
        }
        catch (ConflictException ex)  { return Conflict(new { error = ex.Message }); }
        catch (DomainException ex)    { return BadRequest(new { error = ex.Message }); }
        catch (Exception)             { return StatusCode(500, new { error = "Internal server error" }); }
    }
}
