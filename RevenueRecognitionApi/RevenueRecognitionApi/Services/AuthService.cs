using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RevenueRecognitionApi.Data;
using RevenueRecognitionApi.DTOs;
using RevenueRecognitionApi.Exceptions;
using RevenueRecognitionApi.Models;

namespace RevenueRecognitionApi.Services;


public interface IAuthService
{
    public Task<AuthResponseDto> RegisterUserAsync(RegisterDto registerDto);
    public Task<AuthResponseDto> LoginUserAsync(LoginDto loginDto);
    public Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    public Task SignOutAsync(string refreshToken);
    public Task SignOutAllAsync(int userId);
}


public class AuthService(AppDbContext data, ITokenService tokenService) : IAuthService
{
    
    public async Task<AuthResponseDto> RegisterUserAsync(RegisterDto registerDto)
    {
        if (await data.Users.AnyAsync(u => u.Username == registerDto.Username))
        {
            throw new RecordAlreadyExistsException("User already exists");
        }
        
        var transaction = await data.Database.BeginTransactionAsync();

        try
        {
            var passwordHash = new PasswordHasher<User>().HashPassword(new User(), registerDto.Password);

            var user = new User
            {
                Username = registerDto.Username,
                RoleId = (await data.UserRoles.FirstAsync(r => r.Name == "User")).Id,
                PasswordHash = passwordHash
            };

            await data.Users.AddAsync(user);
            await data.SaveChangesAsync();

            var accessToken = await tokenService.CreateAccessTokenAsync(user);
            var refreshToken = tokenService.CreateRefreshToken();

            await data.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            });

            await data.SaveChangesAsync();
            
            await transaction.CommitAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    
    public async Task<AuthResponseDto> LoginUserAsync(LoginDto loginDto)
    {
        var user = await data.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username);
        
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var passwordVerificationResult  = new PasswordHasher<User>().VerifyHashedPassword(
            user, 
            user.PasswordHash, 
            loginDto.Password
            );
        
        if (passwordVerificationResult != PasswordVerificationResult.Success)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var accessToken = await tokenService.CreateAccessTokenAsync(user);
        var refreshToken = tokenService.CreateRefreshToken();

        var userRefreshToken  = await data.RefreshTokens
            .FirstOrDefaultAsync(t => t.UserId == user.Id && !t.IsRevoked);
        
        if (userRefreshToken != null)
        {
            userRefreshToken.Token = refreshToken;
            userRefreshToken.ExpiresAt = DateTime.UtcNow.AddHours(2);
        }
        else
        {
            await data.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            });
        }
        
        await data.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    
    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        await using var transaction = await data.Database.BeginTransactionAsync();
        
        try
        {
            var rt = await data.RefreshTokens
                .Include(t => t.User)
                .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(
                    t => t.Token == refreshToken &&
                         !t.IsRevoked &&
                         t.ExpiresAt > DateTime.UtcNow
                    );

            if (rt == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            rt.IsRevoked = true;
            await data.SaveChangesAsync();

            var newAccessToken = await tokenService.CreateAccessTokenAsync(rt.User);
            var newRefreshToken = tokenService.CreateRefreshToken();

            await data.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = rt.User.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            });
            
            await data.SaveChangesAsync();

            await transaction.CommitAsync();

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task SignOutAsync(string refreshToken)
    {
        var token = await data.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked);
        
        if (token == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        token.IsRevoked = true;
        
        await data.SaveChangesAsync();
    }
    
    public async Task SignOutAllAsync(int userId)
    {
        var tokens = data.RefreshTokens.Where(t => t.UserId == userId && !t.IsRevoked);
        
        await tokens.ForEachAsync(t => t.IsRevoked = true);
        
        await data.SaveChangesAsync();
    }
    
}