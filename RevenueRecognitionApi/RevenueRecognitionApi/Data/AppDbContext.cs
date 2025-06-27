using Microsoft.EntityFrameworkCore;
using RevenueRecognitionApi.Models;

namespace RevenueRecognitionApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<IndividualClient> IndividualClients { get; set; }
    public DbSet<CompanyClient> CompanyClients { get; set; }
    public DbSet<SoftwareProduct> SoftwareProducts { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractPayment> ContractPayments { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { Id = 1, Name = "Admin" },
            new UserRole { Id = 2, Name = "User" }
        );
        
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "admin", PasswordHash = "adminhash", RoleId = 1 },
            new User { Id = 2, Username = "jan", PasswordHash = "userhash", RoleId = 2 }
        );

        modelBuilder.Entity<IndividualClient>().HasData(
            new IndividualClient
            {
                Id = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Address = "Sezamkowa",
                Email = "jan@klient.com",
                Phone = "123456789",
                Pesel = "12345678901",
                IsDeleted = false
            }
        );
        
        modelBuilder.Entity<CompanyClient>().HasData(
            new CompanyClient
            {
                Id = 1,
                CompanyName = "Test Corp",
                Address = "Business Street",
                Email = "contact@testcorp.com",
                Phone = "123456700",
                Krs = "KRS001"
            }
        );
        
        modelBuilder.Entity<SoftwareProduct>().HasData(
            new SoftwareProduct
            {
                Id = 1,
                Name = "ApbdPro",
                Description = "Apbd master software",
                Version = "1.0",
                Category = "DataBase"
            }
        );
        
        modelBuilder.Entity<Discount>().HasData(
            new Discount
            {
                Id = 1,
                Name = "Summer Sale",
                Percentage = 10,
                Start = new DateTime(2025, 6, 1),
                End = new DateTime(2025, 6, 30),
                SoftwareProductId = 1
            }
        );
        
        modelBuilder.Entity<Contract>().HasData(
            new Contract
            {
                Id = 1,
                ClientId = 1,
                SoftwareProductId = 1,
                SoftwareVersion = "1.0",
                StartDate = new DateTime(2025, 7, 1),
                EndDate = new DateTime(2025, 7, 10),
                Price = 9500,
                SupportExtensionYears = 0,
                IsSigned = false,
                IsActive = true
            }
        );
        
        modelBuilder.Entity<ContractPayment>().HasData(
            new ContractPayment
            {
                Id = 1,
                ContractId = 1,
                Amount = 9500,
                PaymentDate = new DateTime(2025, 7, 2)
            }
        );
    }
    
}