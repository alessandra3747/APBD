using CountryTripsApplication.Exceptions;
using CountryTripsApplication.Models.DTOs;
using CountryTripsApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace CountryTripsApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientsController(IDbService dbService) : ControllerBase
{
    [HttpGet]
    [Route("{id}/trips")]
    //GET => /api/clients/{id}/trips
    public async Task<IActionResult> GetClientsTrips([FromRoute] int id)
    {
        try
        {
            return Ok(await dbService.GetClientsTripsByIdAsync(id));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }


    [HttpPost]
    //POST => /api/clients
    public async Task<IActionResult> CreateClient([FromBody] ClientCreateDTO body)
    {
        var client = await dbService.CreateClientAsync(body);
        
        return Created($"clients/{client.Id}", client);
    }

    [HttpPut]
    [Route("{id}/trips/{tripId}")]
    //PUT => /api/clients/{id}/trips/{tripId}
    public async Task<IActionResult> RegisterClientToTrip([FromRoute] int id, [FromRoute] int tripId)
    {
        try
        {
            await dbService.RegisterClientToTripAsync(id, tripId);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (ClientLimitExceeded)
        {
            return BadRequest($"Client limit to a trip with id {tripId} exceeded");
        }
    }

    [HttpDelete]
    [Route("{id}/trips/{tripId}")]
    //DELETE => /api/clients/{id}/trips/{tripId}
    public async Task<IActionResult> DeleteClientFromTrip([FromRoute] int id, [FromRoute] int tripId)
    {
        try
        {
            await dbService.DeleteClientFromTripAsync(id, tripId);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
    
}