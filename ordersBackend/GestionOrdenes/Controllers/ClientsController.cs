using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Clients;
using OrderManagement.Services;

namespace OrderManagement.Controllers;

[ApiController]
[Route("clients")]
[Authorize]// FIX #1 (Critical): Added class-level [Authorize] to protect all routes by default.
            // GetAll returns PII (name, email, phone, photoUrl) and must require a valid token.
            // Create keeps [AllowAnonymous] below since registration is intentionally public.
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;
    public ClientsController(IClientService clientService) => _clientService = clientService;

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromForm] CreateClientRequest dto)
    {
        try
        {
            var result = await _clientService.CreateAsync(dto);
            return StatusCode(201, result);
        }
        catch (ConflictException ex)  { return Conflict(new { error = ex.Message }); }
        catch (DomainException ex)    { return BadRequest(new { error = ex.Message }); }
        catch (Exception)             { return StatusCode(500, new { error = "Internal server error" }); }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _clientService.GetAllAsync();
            return Ok(result);
        }
        catch (Exception) { return StatusCode(500, new { error = "Internal server error" }); }
    }
}
