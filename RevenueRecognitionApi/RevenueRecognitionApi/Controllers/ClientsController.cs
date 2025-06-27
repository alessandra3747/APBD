using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevenueRecognitionApi.DTOs;
using RevenueRecognitionApi.Exceptions;
using RevenueRecognitionApi.Services;

namespace RevenueRecognitionApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ClientsController(IDbService service) : ControllerBase
{
    
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetAllClients()
    {
        var result = await service.GetAllClientsAsync(); 
        return Ok(result);
    }
    
    
    [HttpGet("individual/{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetIndividualClient(int id)
    {
        try
        {
            var client = await service.GetIndividualClientByIdAsync(id);
            return Ok(client);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPost("individual")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> AddIndividual([FromBody] IndividualClientCreateDto clientData)
    {
        try
        {
            var client = await service.AddIndividualClientAsync(clientData);
            return CreatedAtAction(nameof(GetIndividualClient), new { id = client.Id }, client);
        } 
        catch (RecordAlreadyExistsException e)
        {
            return Conflict(e.Message);
        }
    }
    
    [HttpPut("individual/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateIndividual(int id, [FromBody] IndividualClientUpdateDto clientData)
    {
        try
        {
            await service.UpdateIndividualClientAsync(id, clientData);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpDelete("individual/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteIndividual(int id)
    {
        try
        {
            await service.DeleteIndividualClientAsync(id);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    
    [HttpGet("company/{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetCompanyClient(int id)
    {
        try
        {
            var client = await service.GetCompanyClientByIdAsync(id);
            return Ok(client);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPost("company")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> AddCompany([FromBody] CompanyClientCreateDto clientData)
    {
        try
        {
            var client = await service.AddCompanyClientAsync(clientData);
            return CreatedAtAction(nameof(GetCompanyClient), new { id = client.Id }, client);
        }
        catch (RecordAlreadyExistsException e)
        {
            return Conflict(e.Message);
        }
    }
    
    [HttpPut("company/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCompany(int id, [FromBody] CompanyClientUpdateDto clientData)
    {
        try
        {
            await service.UpdateCompanyClientAsync(id, clientData);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpDelete("company/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCompany(int id)
    {
        return Forbid();
    }
    
}