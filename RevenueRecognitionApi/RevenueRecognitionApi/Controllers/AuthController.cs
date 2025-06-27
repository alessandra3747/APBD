using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevenueRecognitionApi.DTOs;
using RevenueRecognitionApi.Exceptions;
using RevenueRecognitionApi.Services;

namespace RevenueRecognitionApi.Controllers;


[ApiController]
[Route("[controller]")]
public class AuthController(IDbService db, IAuthService authService) : ControllerBase
{
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerData)
    {
        try
        {
            var result = await authService.RegisterUserAsync(registerData);
            return Ok(result);
        }
        catch (RecordAlreadyExistsException e)
        {
            return Conflict(e.Message);
        }
    }
    

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginData)
    {
        try
        {
            var result = await authService.LoginUserAsync(loginData);
            return Ok(result);
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(e.Message);
        }
    }

    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        try
        {
            var result = await authService.RefreshTokenAsync(refreshToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(e.Message);
        }
    }
    
    [Authorize]
    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOut([FromBody] string refreshToken)
    {
        await authService.SignOutAsync(refreshToken);
        return NoContent();
    }
    
    [Authorize]
    [HttpPost("sign-out-all")]
    public async Task<IActionResult> SignOutAll()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        
        await authService.SignOutAllAsync(userId);
        
        return NoContent();
    }
    
}