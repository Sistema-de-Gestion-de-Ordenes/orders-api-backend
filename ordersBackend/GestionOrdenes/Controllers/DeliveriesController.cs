using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Deliveries;
using OrderManagement.Services.Interfaces;

namespace OrderManagement.Controllers;

[ApiController]
[Route("[controller]")]
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

    [HttpPost("{orderId}/evidence")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadEvidence(int orderId, [FromForm] UploadEvidenceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid request data."));

        var result = await _deliveryService.UploadEvidenceAsync(orderId, dto);
        return StatusCode(201, ApiResponse<DeliveryDto>.SuccessResult(result, "Evidence uploaded."));
    }
}
