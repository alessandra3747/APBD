using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RevenueRecognitionApi.Models;

[Table("CompanyClient ")]
public class CompanyClient : Client
{
    [Required, MaxLength(200)]
    public string CompanyName { get; set; } = null!;
    
    [Required, MaxLength(10)]
    public string Krs { get; set; } = null!;
}