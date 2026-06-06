using Microsoft.AspNetCore.Mvc.Testing;

namespace GestionOrdenes.Tests.Integration;

[CollectionDefinition("Integration")]
public class IntegrationTestCollection : ICollectionFixture<WebApplicationFactory<Program>> { }
