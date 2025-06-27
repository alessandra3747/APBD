using Microsoft.AspNetCore.Mvc;
using TripsApplication.DTOs;
using TripsApplication.Services;

namespace TripsApplication.Controllers;

[ApiController]
[Route("api/trips")]
public class TripsController(IDbService service): ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var trips = await service.GetTripsAsync(page, pageSize);
            return Ok(trips);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
    
    
    [HttpPost("{idTrip}/clients")]
    /*
     *  IN POSTMAN - EXAMPLE OF POST BODY JSON TO TEST:
     * {
        "firstName": "John",
        "lastName": "Doe",
        "email": "doe@wp.pl",
        "telephone": "543-323-542",
        "pesel": "91040294554",
        "idTrip": 1,
        "tripName": "Rome",
        "paymentDate": "2021-04-20"
        }
     */
    public async Task<IActionResult> AssignClientToTrip([FromBody] ClientTripDetailsDto dto)
    {
        try
        {
            await service.AssignClientToTripAsync(dto);
            return Ok($"Client {dto.FirstName} {dto.LastName} assigned to trip with id {dto.IdTrip}.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
}