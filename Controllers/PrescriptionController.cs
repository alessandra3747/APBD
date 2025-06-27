using Microsoft.AspNetCore.Mvc;
using PrescriptionApplication.DTOs;
using PrescriptionApplication.Exceptions;
using PrescriptionApplication.Models;
using PrescriptionApplication.Services;

namespace PrescriptionApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class PrescriptionController(IDbService service) : ControllerBase
{
    
    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionCreateDto prescriptionData)
    {
        try
        {
            var prescription = await service.CreatePrescriptionAsync(prescriptionData);
            return CreatedAtAction(nameof(GetPrescriptionById), new { id = prescription.IdPrescription }, prescription);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPrescriptionById([FromRoute] int id)
    {
        try
        {
            var prescription = await service.GetPrescriptionByIdAsync(id);
            return Ok(prescription);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
}