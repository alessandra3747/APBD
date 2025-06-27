using System.ComponentModel.DataAnnotations;

namespace PrescriptionApplication.DTOs;

public class PrescriptionMedicamentCreateDto
{
    [Required]
    public int IdMedicament { get; set; }
    
    [Required]
    public int Dose { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Description { get; set; } = null!;
}