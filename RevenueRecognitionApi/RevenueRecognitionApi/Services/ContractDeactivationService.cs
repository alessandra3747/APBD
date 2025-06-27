using Microsoft.EntityFrameworkCore;
using RevenueRecognitionApi.Data;

namespace RevenueRecognitionApi.Services;

public class ContractDeactivationService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var data = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                var now = DateTime.UtcNow;

                var contractsToExpire = await data.Contracts
                    .Include(c => c.Payments)
                    .Where(
                        c => c.IsActive && 
                             !c.IsSigned && 
                             c.EndDate < now
                             )
                    .ToListAsync(stoppingToken);

                foreach (var contract in contractsToExpire)
                {
                    contract.IsActive = false;
                    
                    foreach (var payment in contract.Payments)
                    {
                        payment.IsRefunded = true;
                    }
                }

                if (contractsToExpire.Count > 0)
                {
                    await data.SaveChangesAsync(stoppingToken);
                }
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}