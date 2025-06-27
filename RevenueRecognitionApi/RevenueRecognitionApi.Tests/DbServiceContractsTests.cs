using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RevenueRecognitionApi.Data;
using RevenueRecognitionApi.DTOs;
using RevenueRecognitionApi.Exceptions;
using RevenueRecognitionApi.Models;
using RevenueRecognitionApi.Services;

namespace RevenueRecognitionApi.Tests;


public class DbServiceContractsTests
{
    
    // CONTRACTS
    private AppDbContext CreateData(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        
        return new AppDbContext(options);
    }

   
    [Fact]
    public async Task AddContractAsync_Success()
    {
        await using var data = CreateData(nameof(AddContractAsync_Success));
        
        await data.IndividualClients.AddAsync(new IndividualClient
        {
            FirstName = "Jan", 
            LastName = "Kowalski", 
            Address = "Sezamkowa", 
            Email = "jan@test.com", 
            Phone = "123456789", 
            Pesel = "12345678901"
        });
        
        await data.SoftwareProducts.AddAsync(new SoftwareProduct
        {
            Name = "Test", 
            Description = "Test description", 
            Version = "1.0", 
            Category = "Test category",
        });
        
        await data.SaveChangesAsync();

        var client = await data.IndividualClients.FirstAsync();
        var product = await data.SoftwareProducts.FirstAsync();

        var service = new DbService(data, new FakeExchangeService());
        
        var contractCreateDto = new ContractCreateDto
        {
            ClientId = client.Id,
            SoftwareProductId = product.Id,
            SoftwareVersion = "1.0",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5),
            SupportExtensionYears = 1
        };
        
        var contract = await service.AddContractAsync(contractCreateDto);
        
        Assert.NotNull(contract);
        Assert.Equal(client.Id, contract.ClientId);
        Assert.Equal(product.Id, contract.SoftwareProductId);
        Assert.Equal(11000, contract.Price); // 10000 base + 1000 support
    }

    [Fact]
    public async Task AddContractAsync_DurationTooShort_Throws()
    {
        await using var data = CreateData(nameof(AddContractAsync_DurationTooShort_Throws));
        
        await data.IndividualClients.AddAsync(new IndividualClient
        {
            FirstName = "Jan", 
            LastName = "Kowalski", 
            Address = "Sezamkowa", 
            Email = "jan@test.com", 
            Phone = "123456789", 
            Pesel = "12345678901"
        });
        
        await data.SoftwareProducts.AddAsync(new SoftwareProduct
        {
            Name = "Test", 
            Description = "Test description", 
            Version = "1.0", 
            Category = "Test category",
        });
        
        await data.SaveChangesAsync();

        var client = await data.IndividualClients.FirstAsync();
        var product = await data.SoftwareProducts.FirstAsync();

        var service = new DbService(data, new FakeExchangeService());
        
        var contractCreateDto = new ContractCreateDto
        {
            ClientId = client.Id,
            SoftwareProductId = product.Id,
            SoftwareVersion = "1.0",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(2),
            SupportExtensionYears = 1
        };
        
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddContractAsync(contractCreateDto));
    }

    [Fact]
    public async Task AddContractAsync_ClientHasActiveContract_Throws()
    {
        await using var data = CreateData(nameof(AddContractAsync_ClientHasActiveContract_Throws));
        
        await data.IndividualClients.AddAsync(new IndividualClient
        {
            FirstName = "Jan", 
            LastName = "Kowalski", 
            Address = "Sezamkowa", 
            Email = "jan@test.com", 
            Phone = "123456789", 
            Pesel = "12345678901"
        });
        
        await data.SoftwareProducts.AddAsync(new SoftwareProduct
        {
            Name = "Test", 
            Description = "Test description", 
            Version = "1.0", 
            Category = "Test category",
        });
        
        await data.SaveChangesAsync();

        var client = await data.IndividualClients.FirstAsync();
        var product = await data.SoftwareProducts.FirstAsync();

        await data.Contracts.AddAsync(new Contract
        {
            ClientId = client.Id,
            SoftwareProductId = product.Id,
            SoftwareVersion = "1.0",
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(10),
            IsActive = true
        });
        
        await data.SaveChangesAsync();

        var service = new DbService(data, new FakeExchangeService());
        
        var contractCreateDto = new ContractCreateDto
        {
            ClientId = client.Id,
            SoftwareProductId = product.Id,
            SoftwareVersion = "1.0",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5),
            SupportExtensionYears = 1
        };
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddContractAsync(contractCreateDto));
    }

    [Fact]
    public async Task GetContractByIdAsync_ReturnsContract()
    {
        await using var data = CreateData(nameof(GetContractByIdAsync_ReturnsContract));
        
        await data.Contracts.AddAsync(new Contract
        {
            ClientId = 1,
            SoftwareProductId = 2,
            SoftwareVersion = "1.0",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5),
            IsActive = true
        });
        
        await data.SaveChangesAsync();

        var contract = await data.Contracts.FirstAsync();

        var service = new DbService(data, new FakeExchangeService());
        
        var result = await service.GetContractByIdAsync(contract.Id);

        Assert.NotNull(result);
        Assert.Equal(contract.SoftwareProductId, result.SoftwareProductId);
    }

    [Fact]
    public async Task GetContractByIdAsync_NotFound_Throws()
    {
        await using var data = CreateData(nameof(GetContractByIdAsync_NotFound_Throws));
        
        var service = new DbService(data, new FakeExchangeService());
       
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetContractByIdAsync(1000));
    }

    [Fact]
    public async Task DeleteContractByIdAsync_DeletesIfNotSigned()
    {
        await using var data = CreateData(nameof(DeleteContractByIdAsync_DeletesIfNotSigned));
        
        await data.Contracts.AddAsync(new Contract
        {
            ClientId = 1,
            SoftwareProductId = 2,
            SoftwareVersion = "1.0",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5),
            IsActive = true,
            IsSigned = false
        });
        
        await data.SaveChangesAsync();

        var contract = await data.Contracts.FirstAsync();

        var service = new DbService(data, new FakeExchangeService());
        
        await service.DeleteContractByIdAsync(contract.Id);

        var updated = await data.Contracts.FindAsync(contract.Id);
        
        Debug.Assert(updated != null, nameof(updated) + " != null");
        Assert.False(updated.IsActive);
    }

    [Fact]
    public async Task DeleteContractByIdAsync_Signed_Throws()
    {
        await using var data = CreateData(nameof(DeleteContractByIdAsync_Signed_Throws));
        
        await data.Contracts.AddAsync(new Contract
        {
            ClientId = 1,
            SoftwareProductId = 2,
            SoftwareVersion = "1.0",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5),
            IsActive = true,
            IsSigned = true
        });
        
        await data.SaveChangesAsync();

        var contract = await data.Contracts.FirstAsync();

        var service = new DbService(data, new FakeExchangeService());
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteContractByIdAsync(contract.Id));
    }
    
    // CONTRACT PAYMENTS
    
    [Fact]
    public async Task AddContractPaymentAsync_Overpay_Throws()
    {
        await using var data = CreateData(nameof(AddContractPaymentAsync_Overpay_Throws));
        
        await data.Contracts.AddAsync(new Contract
        {
            ClientId = 1,
            SoftwareProductId = 1,
            SoftwareVersion = "1.0",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            Price = 1000,
            IsActive = true,
            IsSigned = false
        });
        
        await data.SaveChangesAsync();

        var contract = await data.Contracts.FirstAsync();

        var service = new DbService(data, new FakeExchangeService());
        
        var paymentDto = new ContractPaymentCreateDto
        {
            ContractId = contract.Id,
            Amount = 1500,
            PaymentDate = DateTime.UtcNow.AddDays(1)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddContractPaymentAsync(paymentDto));
    }

    [Fact]
    public async Task AddContractPaymentAsync_OnInactiveContract_Throws()
    {
        await using var data = CreateData(nameof(AddContractPaymentAsync_OnInactiveContract_Throws));
        
        await data.Contracts.AddAsync(new Contract
        {
            ClientId = 1,
            SoftwareProductId = 1,
            SoftwareVersion = "1.0",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            Price = 1000,
            IsActive = false,
            IsSigned = false
        });
        
        await data.SaveChangesAsync();

        var contract = await data.Contracts.FirstAsync();

        var service = new DbService(data, new FakeExchangeService());
       
        var paymentDto = new ContractPaymentCreateDto
        {
            ContractId = contract.Id,
            Amount = 200,
            PaymentDate = DateTime.UtcNow.AddDays(1)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddContractPaymentAsync(paymentDto));
    }

    [Fact]
    public async Task GetContractPaymentByIdAsync_ReturnsPayment()
    {
        await using var data = CreateData(nameof(GetContractPaymentByIdAsync_ReturnsPayment));
       
        await data.ContractPayments.AddAsync(new ContractPayment
        {
            ContractId = 1,
            Amount = 123,
            PaymentDate = DateTime.UtcNow
        });
        
        await data.SaveChangesAsync();

        var payment = await data.ContractPayments.FirstAsync();

        var service = new DbService(data, new FakeExchangeService());
        
        var result = await service.GetContractPaymentByIdAsync(payment.Id);

        Assert.NotNull(result);
        Assert.Equal(payment.Amount, result.Amount);
    }

    [Fact]
    public async Task GetContractPaymentByIdAsync_NotFound_Throws()
    {
        await using var data = CreateData(nameof(GetContractPaymentByIdAsync_NotFound_Throws));
        
        var service = new DbService(data, new FakeExchangeService());

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetContractPaymentByIdAsync(32199));
    }
}