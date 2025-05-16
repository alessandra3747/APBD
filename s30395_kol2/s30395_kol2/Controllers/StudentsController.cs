using Microsoft.AspNetCore.Mvc;
using s30395_kol2.DTOs;
using s30395_kol2.Exceptions;
using s30395_kol2.Services;

namespace s30395_kol2.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentController(IDbService service) : ControllerBase
{
    
    
    [HttpGet]
    public async Task<IActionResult> GetStudents()
    {
        return Ok(await service.GetStudentDetailsAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudent([FromBody] StudentCreateDto body)
    {
        try
        {
            var student = await service.CreateStudentAsync(body);
            return Created($"students/{student.Id}", student);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    
}