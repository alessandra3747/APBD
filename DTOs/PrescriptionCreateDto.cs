using System.ComponentModel.DataAnnotations;
using PrescriptionApplication.Models;

namespace PrescriptionApplication.DTOs;

public class PrescriptionCreateDto
{
    [Required]
    public PatientDto Patient { get; set; } = null!;
    
    [Required]
    [MinLength(1)]
    [MaxLength(10)]
    public List<PrescriptionMedicamentCreateDto> Medicaments { get; set; } = null!;
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    public DateTime DueDate { get; set; }
    
    public int IdDoctor { get; set; }
    
}