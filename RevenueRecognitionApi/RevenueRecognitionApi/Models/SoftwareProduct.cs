using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RevenueRecognitionApi.Models;

[Table("SoftwareProduct")]
public class SoftwareProduct
{
    [Key]
    public int Id { get; set; }
    
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;
    
    [MaxLength(500)]
    public string Description { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Version { get; set; } = null!;
    
    [Required, MaxLength(50)]
    public string Category { get; set; } = null!;
    
    public virtual ICollection<Discount> Discounts { get; set; } = null!;
    
} 