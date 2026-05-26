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
    public async Task GetDeliveryById_ReturnsUnauthorized_WhenTokenIsMissing()
    {
        var response = await _client.GetAsync("/deliveries/1");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
