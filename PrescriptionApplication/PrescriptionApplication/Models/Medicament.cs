using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrescriptionApplication.Models;

[Table("Medicament")]
public class Medicament
{
    [Key]
    public int IdMedicament { get; set; }

    [MaxLength(100)] 
    public string Name { get; set; } = null!;

    [MaxLength(100)]
    public string Description { get; set; } = null!;
    
    [MaxLength(100)]
    public string Type { get; set; } = null!;
    
    public virtual ICollection<PrescriptionMedicament> PrescriptionMedicament { get; set; } = null!;
}