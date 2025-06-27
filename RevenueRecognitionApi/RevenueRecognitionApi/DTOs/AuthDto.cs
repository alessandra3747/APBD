using System.ComponentModel.DataAnnotations;

namespace RevenueRecognitionApi.DTOs;

public class RegisterDto
{
    [Required, MaxLength(100)]
    public string Username { get; set; } = null!;
    
    [Required, MaxLength(100), MinLength(8)]
    public string Password { get; set; } = null!;
}

public class LoginDto
{
    [Required, MaxLength(100)]
    public string Username { get; set; } = null!;
    
    [Required, MaxLength(100), MinLength(8)]
    public string Password { get; set; } = null!;
}

public class AuthResponseDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}