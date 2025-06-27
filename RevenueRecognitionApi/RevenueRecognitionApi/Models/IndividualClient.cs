using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RevenueRecognitionApi.Models;

[Table("IndividualClient")]
public class IndividualClient : Client
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = null!;
    
    [Required, MaxLength(100)]
    public string LastName { get; set; } = null!;
    
    [Required, StringLength(11)]
    public string Pesel { get; set; } = null!;
}