using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Deliveries;
using OrderManagement.Services.Interfaces;

namespace OrderManagement.Controllers;

[ApiController]
[Route("deliveries")]
[Authorize]
public class DeliveriesController : ControllerBase
{
    private readonly IDeliveryService _deliveryService;

    public DeliveriesController(IDeliveryService deliveryService)
    {
        _deliveryService = deliveryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _deliveryService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<DeliveryDto>>.SuccessResult(result));
    }

    /// <summary>
    /// Returns the complete detail of a delivery by its identifier.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<DeliveryDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(string id)
    {
        if (!int.TryParse(id, out var deliveryId) || deliveryId <= 0)
            return BadRequest(ApiResponse<object>.Fail("Invalid delivery id format."));

        var result = await _deliveryService.GetDetailByIdAsync(deliveryId);
        return Ok(ApiResponse<DeliveryDetailDto>.SuccessResult(result));
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,Driver")]
    [ProducesResponseType(typeof(ApiResponse<DeliveryStatusResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateDeliveryStatusDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("The field status is required."));

        if (!int.TryParse(id, out var deliveryId) || deliveryId <= 0)
            return BadRequest(ApiResponse<object>.Fail("Invalid delivery id format."));

        var result = await _deliveryService.UpdateStatusAsync(deliveryId, dto.Status);
        return Ok(ApiResponse<DeliveryStatusResponseDto>.SuccessResult(result));
    }
}
