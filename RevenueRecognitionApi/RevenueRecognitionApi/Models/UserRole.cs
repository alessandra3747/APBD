using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RevenueRecognitionApi.Models;

[Table("UserRole")]
public class UserRole
{
    [Key]
    public int Id { get; set; }
    
    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;
    
    public virtual ICollection<User> Users { get; set; } = null!;
}