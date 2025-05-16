using System.ComponentModel.DataAnnotations;

namespace s30395_kol2.DTOs;

public class StudentCreateDto
{
    [MaxLength(50)]
    public required string FirstName { get; set; }
    
    [MaxLength(100)]
    public required string LastName { get; set; }
    
    [MaxLength(200)]
    public required string Email { get; set; }
    
    public List<int>? GroupAssignments { get; set; }
}