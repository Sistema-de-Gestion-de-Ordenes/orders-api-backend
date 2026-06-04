using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using OrderManagement.Common;
using OrderManagement.Models.DTOs.Clients;
using OrderManagement.Models.Entities;
using OrderManagement.Repositories;
using OrderManagement.Services;

namespace GestionOrdenes.Tests.Unit;

public class ClientServiceTests
{
    private static Mock<IWebHostEnvironment> MakeEnv()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "gestion_ordenes_client_tests");
        var env = new Mock<IWebHostEnvironment>();
        env.Setup(e => e.WebRootPath).Returns(tempRoot);
        env.Setup(e => e.ContentRootPath).Returns(tempRoot);
        return env;
    }

    private static Mock<IFormFile> MakePhoto(
        string contentType = "image/jpeg",
        long size          = 1024,
        string fileName    = "photo.jpg")
    {
        var photo = new Mock<IFormFile>();
        photo.Setup(p => p.ContentType).Returns(contentType);
        photo.Setup(p => p.Length).Returns(size);
        photo.Setup(p => p.FileName).Returns(fileName);
        photo.Setup(p => p.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);
        return photo;
    }

    private static ClientService CreateService(
        Mock<IClientRepository>   clientRepo,
        Mock<IWebHostEnvironment>? env = null)
        => new(clientRepo.Object, (env ?? MakeEnv()).Object);

    // ── CreateAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ThrowsDomainException_WhenMimeTypeIsInvalid()
    {
        var clientRepo = new Mock<IClientRepository>();
        var dto = new CreateClientRequest
        {
            Name = "Maria", Email = "maria@test.com", Phone = "88001234", Password = "Pass123!",
            Photo = MakePhoto(contentType: "image/gif", fileName: "photo.gif").Object
        };

        await Assert.ThrowsAsync<DomainException>(() => CreateService(clientRepo).CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsDomainException_WhenPhotoExceedsMaxSize()
    {
        var clientRepo = new Mock<IClientRepository>();
        var dto = new CreateClientRequest
        {
            Name = "Maria", Email = "maria@test.com", Phone = "88001234", Password = "Pass123!",
            Photo = MakePhoto(size: 6 * 1024 * 1024).Object
        };

        await Assert.ThrowsAsync<DomainException>(() => CreateService(clientRepo).CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsConflictException_WhenEmailAlreadyRegistered()
    {
        var clientRepo = new Mock<IClientRepository>();
        clientRepo.Setup(r => r.GetByEmailAsync("maria@test.com"))
                  .ReturnsAsync(new Client { Email = "maria@test.com" });

        var dto = new CreateClientRequest
        {
            Name = "Maria", Email = "maria@test.com", Phone = "88001234", Password = "Pass123!",
            Photo = MakePhoto().Object
        };

        await Assert.ThrowsAsync<ConflictException>(() => CreateService(clientRepo).CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ReturnsMappedClientResponse_WhenAllDataIsValid()
    {
        var clientRepo = new Mock<IClientRepository>();
        clientRepo.Setup(r => r.GetByEmailAsync("maria@test.com")).ReturnsAsync((Client?)null);
        clientRepo.Setup(r => r.CreateAsync(It.IsAny<Client>()))
                  .ReturnsAsync((Client c) => { c.Id = 1; return c; });

        var dto = new CreateClientRequest
        {
            Name     = "Maria Lopez",
            Email    = "maria@test.com",
            Phone    = "88001234",
            Password = "Pass123!",
            Photo    = MakePhoto().Object
        };

        var result = await CreateService(clientRepo).CreateAsync(dto);

        Assert.Equal(1,               result.Id);
        Assert.Equal("Maria Lopez",   result.Name);
        Assert.Equal("maria@test.com", result.Email);
        Assert.Equal("88001234",      result.Phone);
        Assert.NotNull(result.PhotoUrl);
        Assert.Contains("/uploads/clients/", result.PhotoUrl);
    }

    [Fact]
    public async Task CreateAsync_DeletesFile_WhenRepositoryThrows()
    {
        var clientRepo = new Mock<IClientRepository>();
        clientRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Client?)null);
        clientRepo.Setup(r => r.CreateAsync(It.IsAny<Client>()))
                  .ThrowsAsync(new Exception("DB error"));

        var dto = new CreateClientRequest
        {
            Name = "Maria", Email = "maria@test.com", Phone = "88001234", Password = "Pass123!",
            Photo = MakePhoto().Object
        };

        await Assert.ThrowsAsync<Exception>(() => CreateService(clientRepo).CreateAsync(dto));
    }
}
