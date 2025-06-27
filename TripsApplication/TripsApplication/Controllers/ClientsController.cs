using Microsoft.AspNetCore.Mvc;
using TripsApplication.Exceptions;
using TripsApplication.Services;

namespace TripsApplication.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController(IDbService service) : ControllerBase
{
    
    [HttpDelete("{clientId}")]
    public async Task<IActionResult> DeleteClient([FromRoute] int clientId)
    {
        try
        {
            await service.DeleteClientByIdAsync(clientId);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
}