using System.ComponentModel.DataAnnotations;

namespace CountryTripsApplication.Models.DTOs;

public class ClientCreateDTO
{
    [Required]
    [Length(1,50)]
    public string FirstName { get; set; }
    
    [Required]
    [Length(1,50)]
    public string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    [MaxLength(60)]
    public string Email { get; set; }
    
    [Required]
    [Phone]
    [MaxLength(20)]
    public string Telephone { get; set; }
    
    [Required]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "PESEL must be exactly 11 digits.")]
    public string Pesel { get; set; }
}