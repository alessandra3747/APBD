using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RevenueRecognitionApi.Models;

[Table("Contract")]
public class Contract
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int ClientId { get; set; }
    
    [Required]
    public int SoftwareProductId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string SoftwareVersion { get; set; } = null!;
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    [Required]
    public int SupportExtensionYears { get; set; }
    
    public bool IsSigned { get; set; }
    
    public bool IsActive { get; set; }

    public virtual ICollection<ContractPayment> Payments { get; set; } = null!;

}