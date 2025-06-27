using System.ComponentModel.DataAnnotations;

namespace RevenueRecognitionApi.DTOs;

public class SoftwareProductCreateDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;
    
    [Required, MaxLength(500)]
    public string Description { get; set; } = null!
        ;
    [Required, MaxLength(50)]
    public string Version { get; set; } = null!;
    
    [Required, MaxLength(50)]
    public string Category { get; set; } = null!;
}

public class DiscountCreateDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;
    
    [Required]
    public decimal Percentage { get; set; }
    
    [Required]
    public DateTime Start { get; set; }
    
    [Required]
    public DateTime End { get; set; }
    
    [Required]
    public int SoftwareProductId { get; set; }
}

public class SoftwareProductGetDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public string Version { get; set; } = null!;
    
    public string Category { get; set; } = null!;
}