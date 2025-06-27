using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RevenueRecognitionApi.Models;

[Table("Client")]
public abstract class Client
{
    [Key]
    public int Id { get; set; }
    
    [Required, MaxLength(200)]
    public string Address { get; set; } = null!;
    
    [Required, MaxLength(100)]
    public string Email { get; set; } = null!;
    
    [Required, MaxLength(50)]
    public string Phone { get; set; } = null!;
    
    public bool IsDeleted { get; set; }
}