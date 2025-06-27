using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RevenueRecognitionApi.Models;

[Table("Discount")]
public class Discount
{
    [Key]
    public int Id { get; set; }
    
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;
    
    [Required]
    public decimal Percentage { get; set; }
    
    [Required]
    public DateTime Start { get; set; }
    
    [Required]
    public DateTime End { get; set; }
    
    public int SoftwareProductId { get; set; }
    
    [ForeignKey("SoftwareProductId")]
    public virtual SoftwareProduct SoftwareProduct { get; set; } = null!;
}