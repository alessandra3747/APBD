using Microsoft.AspNetCore.Mvc;
using PrescriptionApplication.Exceptions;
using PrescriptionApplication.Services;

namespace PrescriptionApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientController(IDbService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientDetails([FromRoute] int id)
    {
        try
        {
            var patient = await service.GetPatientDetailsAsync(id);
            return Ok(patient);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}