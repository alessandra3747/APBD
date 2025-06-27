using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RevenueRecognitionApi.Models;

[Table("RefreshToken")]
public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    
    [Required, MaxLength(200)]
    public string Token { get; set; } = null!;
    
    [Required]
    public DateTime ExpiresAt { get; set; }
    
    public bool IsRevoked { get; set; }
}