using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GestionOrdenes.Tests.Integration;

[Trait("Category", "Integration")]
public class DriverEndpointAuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DriverEndpointAuthTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDrivers_ReturnsUnauthorized_WithoutToken()
    {
        var response = await _client.GetAsync("/drivers");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateDriver_ReturnsUnauthorized_WithoutToken()
    {
        var content = new MultipartFormDataContent();
        content.Add(new StringContent("Carlos Pérez"), "name");
        content.Add(new StringContent("Moto Honda"),   "vehicle");
        content.Add(new StringContent("ABC-123"),       "plates");
        content.Add(new StringContent("88001234"),      "phone");

        var response = await _client.PostAsync("/drivers", content);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
