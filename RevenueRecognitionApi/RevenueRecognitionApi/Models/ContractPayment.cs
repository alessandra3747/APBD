using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RevenueRecognitionApi.Models;

[Table("ContractPayment")]
public class ContractPayment
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int ContractId { get; set; }

    [ForeignKey("ContractId")] 
    public virtual Contract Contract { get; set; } = null!;
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public DateTime PaymentDate { get; set; }
    
    public bool IsRefunded { get; set; } = false;
}