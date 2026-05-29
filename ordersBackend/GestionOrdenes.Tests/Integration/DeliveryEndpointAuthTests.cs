using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GestionOrdenes.Tests.Integration;

public class DeliveryEndpointAuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DeliveryEndpointAuthTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDeliveries_ReturnsOk_WithoutAuthentication()
    {
        var response = await _client.GetAsync("/deliveries");

        // Deliveries endpoints are public; no auth required
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDeliveryById_ReturnsNotFound_ForNonExistentId()
    {
        var response = await _client.GetAsync("/deliveries/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
