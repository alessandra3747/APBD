using System.ComponentModel.DataAnnotations;

namespace PrescriptionApplication.DTOs;

public class PatientDto
{
    public int? IdPatient { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = null!;

    [Required]
    public DateTime Birthdate { get; set; }
}