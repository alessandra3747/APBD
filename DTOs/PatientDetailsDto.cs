using System.ComponentModel.DataAnnotations;

namespace PrescriptionApplication.DTOs;

public class PatientDetailsDto
{
    public int IdPatient { get; set; }
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public DateTime Birthdate { get; set; }
    
    public List<PrescriptionResponseDto> Prescriptions { get; set; } = null!;
}