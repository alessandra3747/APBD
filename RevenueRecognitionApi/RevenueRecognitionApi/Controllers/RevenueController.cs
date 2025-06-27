using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevenueRecognitionApi.DTOs;
using RevenueRecognitionApi.Exceptions;
using RevenueRecognitionApi.Services;

namespace RevenueRecognitionApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class RevenueController(IDbService service) : ControllerBase
{
    
    [HttpPost("current")]
    public async Task<IActionResult> GetCurrent([FromBody] RevenueRequestDto revenueData)
    {
        try
        {
            var result = await service.GetCurrentRevenueAsync(revenueData);
            return Ok(result);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    
    [HttpPost("forecast")]
    public async Task<IActionResult> GetForecast([FromBody] RevenueRequestDto revenueData)
    {
        try
        {
            var result = await service.GetForecastRevenueAsync(revenueData);
            return Ok(result);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
}