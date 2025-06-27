using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RevenueRecognitionApi.Data;
using RevenueRecognitionApi.DTOs;
using RevenueRecognitionApi.Exceptions;
using RevenueRecognitionApi.Models;
using RevenueRecognitionApi.Services;

namespace RevenueRecognitionApi.Tests;


public class FakeExchangeService : IExchangeService
{
    public Task<decimal> GetExchangeRateAsync(string from, string to) => Task.FromResult(1m);
}


public class DbServiceTests
{
    private AppDbContext CreateDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        
        return new AppDbContext(options);
    }

    
    // INDIVIDUAL CLIENT TESTS
    
    [Fact]
    public async Task AddIndividualClientAsync_AddsClient()
    {
        await using var data = CreateDb(nameof(AddIndividualClientAsync_AddsClient));
        
        var service = new DbService(data, new FakeExchangeService());
        
        var individualClientCreateDto = new IndividualClientCreateDto
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Address = "Sezamkowa",
            Email = "jan@test.com",
            Phone = "123456789",
            Pesel = "12345678901"
        };

        var result = await service.AddIndividualClientAsync(individualClientCreateDto);
        
        Assert.NotNull(result);
        Assert.Equal("Jan", result.FirstName);
        Assert.Equal("Kowalski", result.LastName);
        Assert.Equal("12345678901", result.Pesel);

        var dbClient = await data.IndividualClients.FirstOrDefaultAsync(c => c.Pesel == "12345678901");
        
        Assert.NotNull(dbClient);
    }

    
    [Fact]
    public async Task AddIndividualClientAsync_DuplicatePesel_Throws()
    {
        await using var data = CreateDb(nameof(AddIndividualClientAsync_DuplicatePesel_Throws));
        
        var service = new DbService(data, new FakeExchangeService());
        
        await data.IndividualClients.AddAsync(new IndividualClient
        {
            Pesel = "12345678901",
            FirstName = "Jan",
            LastName = "Nowak",
            Address = "Pustelnicza",
            Email = "jan2@test.com",
            Phone = "00000078901"
        });
        
        await data.SaveChangesAsync();
        
        var individualClientCreateDto = new IndividualClientCreateDto
        {
            Pesel = "12345678901",
            FirstName = "Jan",
            LastName = "Kowalski",
            Address = "Sezamkowa",
            Email = "jan@test.com",
            Phone = "123456789",
        };

        await Assert.ThrowsAsync<RecordAlreadyExistsException>(() => service.AddIndividualClientAsync(individualClientCreateDto));
    }

    
    [Fact]
    public async Task GetIndividualClientByIdAsync_ReturnsClient()
    {
        await using var data = CreateDb(nameof(GetIndividualClientByIdAsync_ReturnsClient));
        
        var client = new IndividualClient
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Address = "Sezamkowa",
            Email = "jan@test.com",
            Phone = "123456789",
            Pesel = "12345678901"
        };
        
        await data.IndividualClients.AddAsync(client);
        
        await data.SaveChangesAsync();

        var service = new DbService(data, new FakeExchangeService());
        
        var result = await service.GetIndividualClientByIdAsync(client.Id);

        Assert.NotNull(result);
        Assert.Equal("Jan", result.FirstName);
        Assert.Equal("Kowalski", result.LastName);
    }

    
    [Fact]
    public async Task GetIndividualClientByIdAsync_NotFound_Throws()
    {
        await using var data = CreateDb(nameof(GetIndividualClientByIdAsync_NotFound_Throws));
        
        var service = new DbService(data, new FakeExchangeService());

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetIndividualClientByIdAsync(1000));
    }
    

    [Fact]
    public async Task UpdateIndividualClientAsync_UpdatesClient()
    {
        await using var data = CreateDb(nameof(UpdateIndividualClientAsync_UpdatesClient));
        
        var client = new IndividualClient
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Address = "Sezamkowa",
            Email = "jan@test.com",
            Phone = "123456789",
            Pesel = "12345678901"
        };
        
        await data.IndividualClients.AddAsync(client);
        
        await data.SaveChangesAsync();

        var service = new DbService(data, new FakeExchangeService());
        
        var updateClientDto = new IndividualClientUpdateDto
        {
            FirstName = "New",
            LastName = "Name",
            Address = "New Address",
            Email = "new@test.com",
            Phone = "123456789"
        };

        await service.UpdateIndividualClientAsync(client.Id, updateClientDto);

        var updated = await data.IndividualClients.FindAsync(client.Id);
        
        Debug.Assert(updated != null, nameof(updated) + " != null");
        Assert.Equal("New", updated.FirstName);
        Assert.Equal("New Address", updated.Address);
    }

    
    [Fact]
    public async Task UpdateIndividualClientAsync_NotFound_Throws()
    {
        await using var data = CreateDb(nameof(UpdateIndividualClientAsync_NotFound_Throws));
        
        var service = new DbService(data, new FakeExchangeService());
        
        var updateClientDto = new IndividualClientUpdateDto
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Address = "Sezamkowa",
            Email = "jan@test.com",
            Phone = "123456789"
        };

        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateIndividualClientAsync(999, updateClientDto));
    }

    
    [Fact]
    public async Task DeleteIndividualClientAsync_DeletesClient()
    {
        await using var data = CreateDb(nameof(DeleteIndividualClientAsync_DeletesClient));
        
        var client = new IndividualClient
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Address = "Sezamkowa",
            Email = "jan@test.com",
            Phone = "123456789",
            Pesel = "12345678901"
        };
        
        await data.IndividualClients.AddAsync(client);
        
        await data.SaveChangesAsync();

        var service = new DbService(data, new FakeExchangeService());
        
        await service.DeleteIndividualClientAsync(client.Id);

        var deleted = await data.IndividualClients.FindAsync(client.Id);
        
        Debug.Assert(deleted != null, nameof(deleted) + " != null");
        Assert.True(deleted.IsDeleted);
        Assert.Equal("REMOVED", deleted.FirstName);
        Assert.Equal("REMOVED", deleted.Email);
    }

    
    [Fact]
    public async Task DeleteIndividualClientAsync_NotFound_Throws()
    {
        await using var data = CreateDb(nameof(DeleteIndividualClientAsync_NotFound_Throws));
        
        var service = new DbService(data, new FakeExchangeService());

        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteIndividualClientAsync(1234));
    }

    
    // COMPANY CLIENT TESTS

    [Fact]
    public async Task AddCompanyClientAsync_AddsClient()
    {
        await using var data = CreateDb(nameof(AddCompanyClientAsync_AddsClient));
        
        var service = new DbService(data, new FakeExchangeService());
        
        var companyClientCreateDto = new CompanyClientCreateDto
        {
            CompanyName = "Family Company",
            Address = "Sezamkowa",
            Email = "company@test.com",
            Phone = "123456789",
            Krs = "KRS123"
        };

        var result = await service.AddCompanyClientAsync(companyClientCreateDto);

        Assert.NotNull(result);
        Assert.Equal("Family Company", result.CompanyName);
        Assert.Equal("KRS123", result.Krs);

        var dbClient = await data.CompanyClients.FirstOrDefaultAsync(c => c.Krs == "KRS123");
        
        Assert.NotNull(dbClient);
    }

    [Fact]
    public async Task AddCompanyClientAsync_DuplicateKrs_Throws()
    {
        await using var data = CreateDb(nameof(AddCompanyClientAsync_DuplicateKrs_Throws));
        
        await data.CompanyClients.AddAsync(new CompanyClient
        {
            CompanyName = "Family Company",
            Krs = "KRS123",
            Address = "Sezamkowa",
            Email = "company@test.com",
            Phone = "123456789"
        });
        
        await data.SaveChangesAsync();

        var companyClientCreateDto = new CompanyClientCreateDto
        {
            CompanyName = "Other Company",
            Krs = "KRS123",
            Address = "Other Address",
            Email = "other@test.com",
            Phone = "111111119",
        };
        
        var service = new DbService(data, new FakeExchangeService());
        
        await Assert.ThrowsAsync<RecordAlreadyExistsException>(() => service.AddCompanyClientAsync(companyClientCreateDto));
    }

    
    [Fact]
    public async Task GetCompanyClientByIdAsync_ReturnsClient()
    {
        await using var data = CreateDb(nameof(GetCompanyClientByIdAsync_ReturnsClient));
        
        var client = new CompanyClient
        {
            CompanyName = "Family Company",
            Address = "Sezamkowa",
            Email = "company@test.com",
            Phone = "123456789",
            Krs = "KRS123"
        };
        
        await data.CompanyClients.AddAsync(client);
        
        await data.SaveChangesAsync();

        var service = new DbService(data, new FakeExchangeService());
        
        var result = await service.GetCompanyClientByIdAsync(client.Id);

        Assert.NotNull(result);
        Assert.Equal("Family Company", result.CompanyName);
    }

    
    [Fact]
    public async Task GetCompanyClientByIdAsync_NotFound_Throws()
    {
        await using var data = CreateDb(nameof(GetCompanyClientByIdAsync_NotFound_Throws));
        
        var service = new DbService(data, new FakeExchangeService());

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetCompanyClientByIdAsync(1000));
    }

    
    [Fact]
    public async Task UpdateCompanyClientAsync_UpdatesClient()
    {
        await using var data = CreateDb(nameof(UpdateCompanyClientAsync_UpdatesClient));
        
        var client = new CompanyClient
        {
            CompanyName = "Family Company",
            Address = "Sezamkowa",
            Email = "company@test.com",
            Phone = "123456789",
            Krs = "KRS123"
        };
        
        await data.CompanyClients.AddAsync(client);
        
        await data.SaveChangesAsync();

        var service = new DbService(data, new FakeExchangeService());
        
        var updateDto = new CompanyClientUpdateDto
        {
            CompanyName = "New Company",
            Address = "New Address",
            Email = "new@test.com",
            Phone = "000000009"
        };

        await service.UpdateCompanyClientAsync(client.Id, updateDto);

        var updated = await data.CompanyClients.FindAsync(client.Id);
        
        Debug.Assert(updated != null, nameof(updated) + " != null");
        Assert.Equal("New Company", updated.CompanyName);
        Assert.Equal("New Address", updated.Address);
    }

    
    [Fact]
    public async Task UpdateCompanyClientAsync_NotFound_Throws()
    {
        await using var data = CreateDb(nameof(UpdateCompanyClientAsync_NotFound_Throws));
        
        var service = new DbService(data, new FakeExchangeService());
        
        var updateDto = new CompanyClientUpdateDto
        {
            CompanyName = "Family Company",
            Address = "Sezamkowa",
            Email = "company@test.com",
            Phone = "123456789",
        };

        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateCompanyClientAsync(1234, updateDto));
    }
}