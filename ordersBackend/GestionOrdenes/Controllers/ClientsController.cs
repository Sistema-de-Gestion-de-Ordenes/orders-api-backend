using Microsoft.AspNetCore.Mvc;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Clients;
using OrderManagement.Services;

namespace OrderManagement.Controllers;

[ApiController]
[Route("clients")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;
    public ClientsController(IClientService clientService) => _clientService = clientService;

    [HttpPost]
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
}
