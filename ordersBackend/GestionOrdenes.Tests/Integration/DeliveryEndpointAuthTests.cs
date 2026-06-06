using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GestionOrdenes.Tests.Integration;

[Trait("Category", "Integration")]
[Collection("Integration")]
public class DeliveryEndpointAuthTests
{
    private readonly HttpClient _client;

    public DeliveryEndpointAuthTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDeliveries_ReturnsUnauthorized_WithoutToken()
    {
        var response = await _client.GetAsync("/deliveries");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetDeliveryById_ReturnsUnauthorized_WithoutToken()
    {
        var response = await _client.GetAsync("/deliveries/99999");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDelivery_ReturnsUnauthorized_WithoutToken()
    {
        var content  = new StringContent(
            """{"origin":"Warehouse A","destination":"Customer Home","driverId":1}""",
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _client.PutAsync("/deliveries/1", content);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
