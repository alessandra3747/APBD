using System.ComponentModel.DataAnnotations;

namespace RevenueRecognitionApi.DTOs;

public class IndividualClientGetDto
{
    public int Id { get; set; }
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public string Address { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string Phone { get; set; } = null!;
    
    public string Pesel { get; set; } = null!;
}

public class IndividualClientCreateDto
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required, MaxLength(100)] 
    public string LastName { get; set; } = null!;
        
    [Required, MaxLength(200)]
    public string Address { get; set; } = null!;
    
    [Required, MaxLength(100)]
    public string Email { get; set; } = null!;
    
    [Required, MaxLength(50)]
    public string Phone { get; set; } = null!;
    
    [Required, StringLength(11)]
    public string Pesel { get; set; } = null!;
}

public class IndividualClientUpdateDto
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = null!;
    
    [Required, MaxLength(100)]
    public string LastName { get; set; } = null!;
    
    [Required, MaxLength(200)]
    public string Address { get; set; } = null!;
    
    [Required, MaxLength(100), EmailAddress]
    public string Email { get; set; } = null!;
    
    [Required, MaxLength(50), Phone]
    public string Phone { get; set; } = null!;
}


public class CompanyClientGetDto
{
    public int Id { get; set; }
    
    public string CompanyName { get; set; } = null!;
    
    public string Address { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string Phone { get; set; } = null!;
    
    public string Krs { get; set; } = null!;
}

public class CompanyClientCreateDto
{
    [Required, MaxLength(200)]
    public string CompanyName { get; set; } = null!;
    
    [Required, MaxLength(200)]
    public string Address { get; set; } = null!;
    
    [Required, MaxLength(100), EmailAddress]
    public string Email { get; set; } = null!;
    
    [Required, MaxLength(50), Phone]
    public string Phone { get; set; } = null!;
    
    [Required, MaxLength(10)]
    public string Krs { get; set; } = null!;
}

public class CompanyClientUpdateDto
{
    [Required, MaxLength(200)]
    public string CompanyName { get; set; } = null!;
    
    [Required, MaxLength(200)]
    public string Address { get; set; } = null!;
    
    [Required, MaxLength(100)]
    public string Email { get; set; } = null!;
    
    [Required, MaxLength(50)]
    public string Phone { get; set; } = null!;
}

public class ClientsListDto
{
    public List<IndividualClientGetDto> Individuals { get; set; } = null!;
    public List<CompanyClientGetDto> Companies { get; set; } = null!;
}