using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Deliveries;
using OrderManagement.Services;
namespace OrderManagement.Controllers;

[ApiController]
[Route("deliveries")]
[Authorize]
public class DeliveriesController : ControllerBase
{
    private readonly IDeliveryService _deliveryService;
    public DeliveriesController(IDeliveryService deliveryService) => _deliveryService = deliveryService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _deliveryService.GetAllAsync();
            return Ok(result);
        }
        catch (Exception) { return StatusCode(500, new { error = "Internal server error" }); }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _deliveryService.GetByIdAsync(id);
            return Ok(result);
        }
        catch (NotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception)            { return StatusCode(500, new { error = "Internal server error" }); }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeliveryRequest dto)
    {
        try
        {
            var result = await _deliveryService.CreateAsync(dto);
            return StatusCode(201, result);
        }
        catch (NotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (DomainException ex)   { return BadRequest(new { error = ex.Message }); }
        catch (Exception)            { return StatusCode(500, new { error = "Internal server error" }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDeliveryRequest dto)
    {
        try
        {
            var result = await _deliveryService.UpdateAsync(id, dto);
            return Ok(result);
        }
        catch (NotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (DomainException ex)   { return BadRequest(new { error = ex.Message }); }
        catch (Exception)            { return StatusCode(500, new { error = "Internal server error" }); }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _deliveryService.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (Exception)            { return StatusCode(500, new { error = "Internal server error" }); }
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(DeliveryStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest dto)
    {
        try
        {
            var performedBy = User.FindFirstValue(ClaimTypes.Name) ?? "unknown";
            var result = await _deliveryService.UpdateStatusAsync(id, dto, performedBy);
            return Ok(result);
        }
        catch (NotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (DomainException ex)   { return BadRequest(new { error = ex.Message }); }
        catch (Exception)            { return StatusCode(500, new { error = "Internal server error" }); }
    }
}
