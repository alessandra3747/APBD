namespace PrescriptionApplication.DTOs;

public class MedicamentDto
{
    public int IdMedicament { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public int Dose { get; set; }
    
    public string Details { get; set; } = null!;
    
}