using CountryTripsApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace CountryTripsApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class TripsController(IDbService dbService) : ControllerBase
{

    [HttpGet]
    //GET => /api/trips
    public async Task<IActionResult> GetTrips()
    {
        return Ok(await dbService.GetTripsAsync());
    }
    
}