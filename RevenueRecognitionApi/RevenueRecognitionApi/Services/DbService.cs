using Microsoft.EntityFrameworkCore;
using RevenueRecognitionApi.Data;
using RevenueRecognitionApi.DTOs;
using RevenueRecognitionApi.Exceptions;
using RevenueRecognitionApi.Models;

namespace RevenueRecognitionApi.Services;

public interface IDbService
{
    // ALL CLIENTS
    public Task<ClientsListDto> GetAllClientsAsync();
    
    // INDIVIDUAL CLIENTS
    public Task<IndividualClientGetDto> GetIndividualClientByIdAsync(int id);
    public Task<IndividualClientGetDto> AddIndividualClientAsync(IndividualClientCreateDto clientData);
    public Task UpdateIndividualClientAsync(int id, IndividualClientUpdateDto clientData);
    public Task DeleteIndividualClientAsync(int id);

    // COMPANY CLIENTS
    
    public Task<CompanyClientGetDto> GetCompanyClientByIdAsync(int id);
    public Task<CompanyClientGetDto> AddCompanyClientAsync(CompanyClientCreateDto clientData);
    public Task UpdateCompanyClientAsync(int id, CompanyClientUpdateDto clientData);

    // SOFTWARE PRODUCTS AND DISCOUNTS
    public Task<SoftwareProductGetDto> GetSoftwareProductByIdAsync(int id);
    public Task<SoftwareProductGetDto> AddSoftwareProductAsync(SoftwareProductCreateDto softwareProductData);
    public Task<DiscountGetDto> GetDiscountByIdAsync(int id);
    public Task<DiscountGetDto> AddDiscountAsync(DiscountCreateDto discountData);

    // CONTRACTS AND PAYMENTS
    public Task<ContractGetDto> GetContractByIdAsync(int id);
    public Task<ContractGetDto> AddContractAsync(ContractCreateDto contractData);
    public Task DeleteContractByIdAsync(int id);
    public Task<ContractPaymentGetDto> GetContractPaymentByIdAsync(int id);
    public Task<ContractPaymentGetDto> AddContractPaymentAsync(ContractPaymentCreateDto contractPaymentData);

    // REVENUE
    public Task<RevenueResponseDto> GetCurrentRevenueAsync(RevenueRequestDto revenueData);
    public Task<RevenueResponseDto> GetForecastRevenueAsync(RevenueRequestDto dto);
}


public class DbService(AppDbContext data, IExchangeService exchangeService) : IDbService
{
    
    // CLIENTS
    public async Task<ClientsListDto> GetAllClientsAsync()
    {
        var individuals = await data.IndividualClients
            .Where(c => !c.IsDeleted)
            .Select(c => new IndividualClientGetDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Address = c.Address,
                Email = c.Email,
                Phone = c.Phone,
                Pesel = c.Pesel
            }).ToListAsync();

        var companies = await data.CompanyClients
            .Select(c => new CompanyClientGetDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                Address = c.Address,
                Email = c.Email,
                Phone = c.Phone,
                Krs = c.Krs
            }).ToListAsync();

        return new ClientsListDto
        {
            Individuals = individuals,
            Companies = companies
        };
    }
    

   // INDIVIDUAL CLIENTS
   public async Task<IndividualClientGetDto> GetIndividualClientByIdAsync(int id)
   {
       
       var client = await data.IndividualClients
           .Where(c => c.Id == id && !c.IsDeleted)
           .Select(c => new IndividualClientGetDto
           {
               Id = c.Id,
               FirstName = c.FirstName,
               LastName = c.LastName,
               Address = c.Address,
               Email = c.Email,
               Phone = c.Phone,
               Pesel = c.Pesel
           }).FirstOrDefaultAsync();
       
       return client ?? throw new NotFoundException($"Client with id {id} not found");
   }

    public async Task<IndividualClientGetDto> AddIndividualClientAsync(IndividualClientCreateDto clientData)
    {
            
        if (await data.IndividualClients.AnyAsync(c => c.Pesel == clientData.Pesel))
        {
            throw new RecordAlreadyExistsException($"Client with PESEL {clientData.Pesel} already exists.");
        }
        
        var client = new IndividualClient
        {
            FirstName = clientData.FirstName,
            LastName = clientData.LastName,
            Address = clientData.Address,
            Email = clientData.Email,
            Phone = clientData.Phone,
            Pesel = clientData.Pesel
        };
        data.IndividualClients.Add(client);
        await data.SaveChangesAsync();
        
        return new IndividualClientGetDto
        {
            Id = client.Id,
            FirstName = clientData.FirstName,
            LastName = clientData.LastName,
            Address = clientData.Address,
            Email = clientData.Email,
            Phone = clientData.Phone,
            Pesel = clientData.Pesel
        };
    }

    public async Task UpdateIndividualClientAsync(int id, IndividualClientUpdateDto clientData)
    {
        var client = await data.IndividualClients.FirstOrDefaultAsync(c => c.Id == id);
            
        if (client == null || client.IsDeleted)
        {
            throw new NotFoundException($"Client with id {id} not found or is deleted");
        }

        client.FirstName = clientData.FirstName;
        client.LastName = clientData.LastName;
        client.Address = clientData.Address;
        client.Email = clientData.Email;
        client.Phone = clientData.Phone;
            
        await data.SaveChangesAsync();
    }

        
    public async Task DeleteIndividualClientAsync(int id)
    {
        var client = await data.IndividualClients.FirstOrDefaultAsync(c => c.Id == id);
        
        if (client == null || client.IsDeleted)
        {
            throw new NotFoundException($"Client with id {id} not found or is deleted");
        }
        
        client.IsDeleted = true;
        client.FirstName = client.LastName = client.Address = client.Email = client.Phone = "REMOVED";
        
        await data.SaveChangesAsync();
    }

    // COMPANY CLIENTS
    public async Task<CompanyClientGetDto> GetCompanyClientByIdAsync(int id)
    {
        
        var client = await data.CompanyClients
            .Where(c => c.Id == id)
            .Select(c => new CompanyClientGetDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                Address = c.Address,
                Email = c.Email,
                Phone = c.Phone,
                Krs = c.Krs
            }).FirstOrDefaultAsync();
        
        return client ?? throw new NotFoundException($"Client with id {id} not found");
    }

    public async Task<CompanyClientGetDto> AddCompanyClientAsync(CompanyClientCreateDto clientData)
    {
        if (await data.CompanyClients.AnyAsync(c => c.Krs == clientData.Krs))
        {
            throw new RecordAlreadyExistsException($"Client with KRS {clientData.Krs} already exists.");
        }
        
        var client = new CompanyClient
        {
            CompanyName = clientData.CompanyName,
            Address = clientData.Address,
            Email = clientData.Email,
            Phone = clientData.Phone,
            Krs = clientData.Krs
        };
        
        data.CompanyClients.Add(client);
        await data.SaveChangesAsync();
        
        return new CompanyClientGetDto
        {
            Id = client.Id,
            CompanyName = client.CompanyName,
            Address = client.Address,
            Email = clientData.Email,
            Phone = clientData.Phone,
            Krs = clientData.Krs
        };
    }
    
    public async Task UpdateCompanyClientAsync(int id, CompanyClientUpdateDto clientData)
    {
        var client = await data.CompanyClients.FirstOrDefaultAsync(c => c.Id == id);
        
        if (client == null)
        {
            throw new NotFoundException($"Client with id {id} not found.");
        }
        
        client.CompanyName = clientData.CompanyName;
        client.Address = clientData.Address;
        client.Email = clientData.Email;
        client.Phone = clientData.Phone;
        
        await data.SaveChangesAsync();
    }

    // SOFTWARE PRODUCTS
    public async Task<SoftwareProductGetDto> GetSoftwareProductByIdAsync(int id)
    {
        var product = await data.SoftwareProducts
            .Where(p => p.Id == id)
            .Select(p => new SoftwareProductGetDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Version = p.Version,
                Category = p.Category
            }).FirstOrDefaultAsync();
        
        return product ?? throw new NotFoundException($"Software product with id {id} not found");
    }

    public async Task<SoftwareProductGetDto> AddSoftwareProductAsync(SoftwareProductCreateDto softwareProductData)
    {
        if (await data.SoftwareProducts.AnyAsync(
                s => s.Name == softwareProductData.Name && 
                     s.Description == softwareProductData.Description && 
                     s.Version == softwareProductData.Version &&
                     s.Category == softwareProductData.Category)
            )
        {
            throw new RecordAlreadyExistsException($"Software product {softwareProductData.Name} already exists.");
        }
        
        var product = new SoftwareProduct
        {
            Name = softwareProductData.Name,
            Description = softwareProductData.Description,
            Version = softwareProductData.Version,
            Category = softwareProductData.Category
        };
            
        data.SoftwareProducts.Add(product);
        await data.SaveChangesAsync();
            
        return new SoftwareProductGetDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Version = product.Version,
            Category = product.Category
        };
    }

    public async Task<DiscountGetDto> GetDiscountByIdAsync(int id)
    {
        var discount = await data.Discounts
            .Where(d => d.Id == id)
            .Select(d => new DiscountGetDto
            {
                Id = d.Id,
                Name = d.Name,
                Percentage = d.Percentage,
                Start = d.Start,
                End = d.End,
                SoftwareProductId = d.SoftwareProductId
            }).FirstOrDefaultAsync();
        
        return discount ?? throw new NotFoundException($"Discount with id {id} not found");
    }
    
    public async Task<DiscountGetDto> AddDiscountAsync(DiscountCreateDto discountData)
    {
        if (await data.Discounts.AnyAsync(
                d => d.Name == discountData.Name &&
                     d.Percentage == discountData.Percentage &&
                     d.Start == discountData.Start &&
                     d.End == discountData.End &&
                     d.SoftwareProductId == discountData.SoftwareProductId
            ))
        {
            throw new RecordAlreadyExistsException($"Discount {discountData.Name} already exists.");
        }
        
        var discount = new Discount
        {
            Name = discountData.Name,
            Percentage = discountData.Percentage,
            Start = discountData.Start,
            End = discountData.End,
            SoftwareProductId = discountData.SoftwareProductId
        };
        
        data.Discounts.Add(discount);
        await data.SaveChangesAsync();
        
        return new DiscountGetDto
        {
            Id = discount.Id,
            Name = discount.Name,
            Percentage = discount.Percentage,
            Start = discount.Start,
            End = discount.End,
            SoftwareProductId = discountData.SoftwareProductId
        };
    }

    // CONTRACTS

    public async Task<ContractGetDto> GetContractByIdAsync(int id)
    {
        var contract = await data.Contracts
            .Where(c => c.Id == id)
            .Select(c => new ContractGetDto
            {
                ClientId = c.Id,
                SoftwareProductId = c.SoftwareProductId,
                SoftwareVersion = c.SoftwareVersion,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                SupportExtensionYears = c.SupportExtensionYears
                
            }).FirstOrDefaultAsync();
        
        return contract ?? throw new NotFoundException($"Contract with id {id} not found");
    }

    public async Task<ContractGetDto> AddContractAsync(ContractCreateDto contractData)
    {
        
        // Duration check
        if ((contractData.EndDate - contractData.StartDate).TotalDays < 3 || 
            (contractData.EndDate - contractData.StartDate).TotalDays > 30)
        {
            throw new ArgumentException("Contract duration must be between 3 and 30 days.");
        }
        
        // Support extension check
        if (contractData.SupportExtensionYears < 0 || contractData.SupportExtensionYears > 3)
        {
            throw new ArgumentException("Support extension can only be 0-3 years.");
        }
        
        // Client check
        var client = await data.IndividualClients.FindAsync(contractData.ClientId) ??
                     (Client?)await data.CompanyClients.FindAsync(contractData.ClientId);
            
        if (client == null)
        {
            throw new NotFoundException($"Client with id {contractData.ClientId} does not exist.");
        }

        // Software check
        var software = await data.SoftwareProducts
            .Include(s => s.Discounts)
            .FirstOrDefaultAsync(
                s => s.Id == contractData.SoftwareProductId &&
                     s.Version == contractData.SoftwareVersion
                     );
            
        if (software == null)
        {
            throw new NotFoundException($"Software product with id {contractData.SoftwareProductId} does not exist.");
        }

        // Existing active contract or subscription check
        var hasActive = await data.Contracts.AnyAsync(
            c => c.ClientId == contractData.ClientId && 
                 c.SoftwareProductId == contractData.SoftwareProductId && 
                 c.IsActive
                 );
            
        if (hasActive)
        {
            throw new InvalidOperationException($"Client with id {contractData.ClientId} already has an active contract for this product.");
        }

        // Calculate price
        decimal basePrice = 10000;
        decimal supportCost = contractData.SupportExtensionYears * 1000;
        decimal price = basePrice + supportCost;

        // Apply best discount (if any active)
        var now = DateTime.UtcNow;
        var bestDiscount = software.Discounts
            .Where(d => d.Start <= now && d.End >= now)
            .OrderByDescending(d => d.Percentage)
            .FirstOrDefault();
            
        if (bestDiscount != null)
        {
            price -= price * (bestDiscount.Percentage / 100m);
        }

        // Loyal client check
        var isLoyal = await data.Contracts.AnyAsync(c => c.ClientId == contractData.ClientId && c.IsSigned);
        
        if (isLoyal)
        {
            price -= price * 0.05m;
        }

        var contract = new Contract
        {
            ClientId = contractData.ClientId,
            SoftwareProductId = contractData.SoftwareProductId,
            SoftwareVersion = contractData.SoftwareVersion,
            StartDate = contractData.StartDate,
            EndDate = contractData.EndDate,
            Price = price,
            SupportExtensionYears = contractData.SupportExtensionYears,
            IsSigned = false,
            IsActive = true
        };
        
        data.Contracts.Add(contract);
        await data.SaveChangesAsync();
        
        return new ContractGetDto
        {
            Id = contract.Id,
            ClientId = contract.Id,
            SoftwareProductId = contract.SoftwareProductId,
            SoftwareVersion = contract.SoftwareVersion,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            Price = contract.Price,
            SupportExtensionYears = contractData.SupportExtensionYears,
            IsSigned = contract.IsSigned,
            IsActive = contract.IsActive
        };
    }
    
    public async Task DeleteContractByIdAsync(int id)
    {
        var contract = await data.Contracts.FirstOrDefaultAsync(c => c.Id == id);
        
        if (contract == null)
        {
            throw new NotFoundException($"Contract with id {id} not found.");
        }
        
        if (contract.IsSigned)
        {
            throw new InvalidOperationException("Cannot delete a signed contract.");
        }
        
        contract.IsActive = false;
        await data.SaveChangesAsync();
    }

    public async Task<ContractPaymentGetDto> GetContractPaymentByIdAsync(int id)
    {
        var contractPayment = await data.ContractPayments
            .Where(c => c.Id == id)
            .Select(c => new ContractPaymentGetDto
            {
                Id = c.Id,
                ContractId = c.ContractId,
                Amount = c.Amount,
                PaymentDate = c.PaymentDate
            }).FirstOrDefaultAsync();
        
        return contractPayment ?? throw new NotFoundException($"Contract payment with id {id} not found");
    }
        
    public async Task<ContractPaymentGetDto> AddContractPaymentAsync(ContractPaymentCreateDto contractPaymentData)
    {
        await using var transaction = await data.Database.BeginTransactionAsync();
        
        try
        {
            var contract = await data.Contracts
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.Id == contractPaymentData.ContractId);

            if (contract == null)
            {
                throw new NotFoundException($"Contract with id {contractPaymentData.ContractId} does not exist.");
            }

            if (contractPaymentData.PaymentDate < contract.StartDate ||
                contractPaymentData.PaymentDate > contract.EndDate)
            {
                throw new InvalidOperationException("Payment must be made within the contract period.");
            }

            if (!contract.IsActive)
            {
                throw new InvalidOperationException(
                    $"Contract with id {contractPaymentData.ContractId} is not active.");
            }

            var sumPaid = contract.Payments.Sum(p => p.Amount);

            if (sumPaid + contractPaymentData.Amount > contract.Price)
            {
                throw new InvalidOperationException("Total payments cannot exceed contract price.");
            }

            var contractPayment = new ContractPayment
            {
                ContractId = contractPaymentData.ContractId,
                Amount = contractPaymentData.Amount,
                PaymentDate = contractPaymentData.PaymentDate
            };
            data.ContractPayments.Add(contractPayment);
            await data.SaveChangesAsync();


            // After payment saved, check if fully paid
            contract = await data.Contracts
                .Include(c => c.Payments)
                .FirstAsync(c => c.Id == contractPaymentData.ContractId);

            if (contract.Payments.Sum(p => p.Amount) == contract.Price)
            {
                contract.IsSigned = true;
            }

            await data.SaveChangesAsync();

            await transaction.CommitAsync();

            return new ContractPaymentGetDto
            {
                Id = contractPayment.Id,
                ContractId = contractPayment.ContractId,
                Amount = contractPayment.Amount,
                PaymentDate = contractPayment.PaymentDate,
            };
        } 
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    
    // REVENUE
    public async Task<RevenueResponseDto> GetCurrentRevenueAsync(RevenueRequestDto revenueData)
    {
        // Revenue from signed contracts (fully paid)
        var contracts = data.Contracts
            .Where(c => c.IsSigned)
            .AsQueryable();
        
        if (revenueData.ProductId.HasValue)
        {
            contracts = contracts.Where(c => c.SoftwareProductId == revenueData.ProductId.Value);
        }
        
        var plnRevenue = await contracts.SumAsync(c => c.Price);
        
        if (string.IsNullOrWhiteSpace(revenueData.Currency) || revenueData.Currency == "PLN")
        {
            return new RevenueResponseDto
            {
                Amount = plnRevenue, 
                Currency = "PLN", 
                ExchangeRate = 1
            };
        }
        
        var rate = await exchangeService.GetExchangeRateAsync("PLN", revenueData.Currency);
        
        return new RevenueResponseDto
        {
            Amount = plnRevenue * rate,
            Currency = revenueData.Currency,
            ExchangeRate = rate
        };
    }

    public async Task<RevenueResponseDto> GetForecastRevenueAsync(RevenueRequestDto revenueData)
    {
        // Signed + all active unsigned contracts
        var contracts = data.Contracts
            .Where(c => c.IsSigned || c.IsActive)
            .AsQueryable();
        
        if (revenueData.ProductId.HasValue)
        {
            contracts = contracts.Where(c => c.SoftwareProductId == revenueData.ProductId.Value);
        }
        
        var plnRevenue = await contracts.SumAsync(c => c.Price);
        
        if (string.IsNullOrWhiteSpace(revenueData.Currency) || revenueData.Currency == "PLN")
        {
            return new RevenueResponseDto
            {
                Amount = plnRevenue, 
                Currency = "PLN", 
                ExchangeRate = 1
            };
        }

        var rate = await exchangeService.GetExchangeRateAsync("PLN", revenueData.Currency);
        
        return new RevenueResponseDto
        {
            Amount = plnRevenue * rate,
            Currency = revenueData.Currency,
            ExchangeRate = rate
        };
    }
    
}