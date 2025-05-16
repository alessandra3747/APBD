namespace s30395_kol2.DTOs;

public class StudentDetailsGetDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public List<StudentGroupGetDto> Groups { get; set; }
}