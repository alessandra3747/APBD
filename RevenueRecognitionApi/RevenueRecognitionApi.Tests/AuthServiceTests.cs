using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RevenueRecognitionApi.Data;
using RevenueRecognitionApi.DTOs;
using RevenueRecognitionApi.Exceptions;
using RevenueRecognitionApi.Models;
using RevenueRecognitionApi.Services;

namespace RevenueRecognitionApi.Tests;


public class FakeTokenService : ITokenService
{
    public Task<string> CreateAccessTokenAsync(User user) => Task.FromResult("fake-access-token");
    public string CreateRefreshToken() => "fake-refresh-token";
}


public class AuthServiceTests
{
    private AppDbContext CreateDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        
        return new AppDbContext(options);
    }

    
    [Fact]
    public async Task RegisterUserAsync_ExistingUser_Throws()
    {
       await using var data = CreateDb(nameof(RegisterUserAsync_ExistingUser_Throws));
        
        await data.UserRoles.AddAsync(new UserRole
        {
            Id = 1, 
            Name = "User"
        });
        
        await data.Users.AddAsync(new User
        {
            Username = "jan", 
            PasswordHash = "any", 
            RoleId = 1
        });
        
        await data.SaveChangesAsync();

        var service = new AuthService(data, new FakeTokenService());
        
        var registerDto = new RegisterDto
        {
            Username = "jan", 
            Password = "Password123"
        };

        await Assert.ThrowsAsync<RecordAlreadyExistsException>(() => service.RegisterUserAsync(registerDto));
    }

    
    [Fact]
    public async Task LoginUserAsync_ValidCredentials_Success()
    {
        await using var data = CreateDb(nameof(LoginUserAsync_ValidCredentials_Success));
        
        await data.UserRoles.AddAsync(new UserRole
        {
            Id = 1, 
            Name = "User"
        });
        
        var user = new User
        {
            Username = "jan",
            PasswordHash = new PasswordHasher<User>().HashPassword(new User(), "Password123"),
            RoleId = 1
        };
        
        await data.Users.AddAsync(user);
        
        await data.SaveChangesAsync();

        var service = new AuthService(data, new FakeTokenService());
        
        var loginDto = new LoginDto 
        { 
            Username = "jan", 
            Password = "Password123" 
        };

        var result = await service.LoginUserAsync(loginDto);

        Assert.NotNull(result);
        Assert.Equal("fake-access-token", result.AccessToken);
        Assert.Equal("fake-refresh-token", result.RefreshToken);

        var refreshTokenToken = await data.RefreshTokens.FirstOrDefaultAsync(t => t.UserId == user.Id);
        
        Assert.NotNull(refreshTokenToken);
        Assert.Equal("fake-refresh-token", refreshTokenToken.Token);
        Assert.False(refreshTokenToken.IsRevoked);
    }

    
    [Fact]
    public async Task LoginUserAsync_WrongPassword_Throws()
    {
        await using var data = CreateDb(nameof(LoginUserAsync_WrongPassword_Throws));
        
        await data.UserRoles.AddAsync(new UserRole
        {
            Id = 1, 
            Name = "User"
        });
        
        var user = new User
        {
            Username = "jan",
            PasswordHash = new PasswordHasher<User>().HashPassword(new User(), "Password123"),
            RoleId = 1
        };
        
        await data.Users.AddAsync(user);
        
        await data.SaveChangesAsync();

        var service = new AuthService(data, new FakeTokenService());
        
        var loginDto = new LoginDto
        {
            Username = "jan", 
            Password = "WrongPassword"
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginUserAsync(loginDto));
    }
    

    [Fact]
    public async Task SignOutAsync_InvalidToken_Throws()
    {
        await using var data = CreateDb(nameof(SignOutAsync_InvalidToken_Throws));
        
        var service = new AuthService(data, new FakeTokenService());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.SignOutAsync("fake-refresh-token"));
    }

}