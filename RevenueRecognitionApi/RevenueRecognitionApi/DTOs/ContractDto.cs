using System.ComponentModel.DataAnnotations;

namespace RevenueRecognitionApi.DTOs;

public class ContractGetDto
{
    public int Id { get; set; }
    
    public int ClientId { get; set; }
    
    public int SoftwareProductId { get; set; }
    
    public string SoftwareVersion { get; set; } = null!;
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }

    public int SupportExtensionYears { get; set; }
    
    public decimal Price { get; set; }
    
    public bool IsSigned { get; set; }
    
    public bool IsActive { get; set; }
}

public class ContractCreateDto
{
    [Required]
    public int ClientId { get; set; }

    [Required]
    public int SoftwareProductId { get; set; }

    [Required, MaxLength(50)] 
    public string SoftwareVersion { get; set; } = null!;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public int SupportExtensionYears { get; set; }
}

public class ContractPaymentGetDto
{
    public int Id { get; set; }
    
    public int ContractId { get; set; }

    public decimal Amount { get; set; }
    
    public DateTime PaymentDate { get; set; }
}

public class ContractPaymentCreateDto
{
    [Required]
    public int ContractId { get; set; }
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public DateTime PaymentDate { get; set; }
}