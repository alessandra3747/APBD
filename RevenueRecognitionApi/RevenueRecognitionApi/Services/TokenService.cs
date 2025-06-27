using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RevenueRecognitionApi.Data;
using RevenueRecognitionApi.Models;

namespace RevenueRecognitionApi.Services;


public interface ITokenService
{
    public Task<string> CreateAccessTokenAsync(User user);
    public string CreateRefreshToken();
}


public class TokenService(IConfiguration configuration, AppDbContext data) : ITokenService
{
    public async Task<string> CreateAccessTokenAsync(User user)
    {
        var userRole = await data.UserRoles.FirstOrDefaultAsync(e => e.Id == user.RoleId);
        
        if (userRole == null)
        {
            throw new UnauthorizedAccessException("Role not found");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, userRole.Name)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SymmetricSecurityKey"]!));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = configuration["JWT:Issuer"],
            Audience = configuration["JWT:Audience"],
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = signingCredentials,
            Expires = DateTime.UtcNow.AddMinutes(30)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string CreateRefreshToken()
    {
        var randomNumber = new byte[96];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}