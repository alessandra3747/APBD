using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RevenueRecognitionApi.Models;

[Table("User")]
public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required, MaxLength(100)]
    public string Username { get; set; } = null!;
    
    [Required, MaxLength(200)]
    public string PasswordHash { get; set; } = null!;
    
    [Required]
    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    public virtual UserRole Role { get; set; } = null!;
    
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = null!;
}