using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevenueRecognitionApi.DTOs;
using RevenueRecognitionApi.Exceptions;
using RevenueRecognitionApi.Services;

namespace RevenueRecognitionApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class SoftwareController(IDbService service) : ControllerBase
{
    
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetSoftwareById(int id)
    {
        try
        {
            var product = await service.GetSoftwareProductByIdAsync(id);
            return Ok(product);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddSoftware([FromBody] SoftwareProductCreateDto softwareProductData)
    {
        try
        {
            var product = await service.AddSoftwareProductAsync(softwareProductData);
            return CreatedAtAction(nameof(GetSoftwareById), new { id = product.Id }, product);
        }
        catch (RecordAlreadyExistsException e)
        {
            return Conflict(e.Message);
        }
    }

    
    [HttpGet("discount/{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetDiscountById(int id)
    {
        try
        {
            var discount = await service.GetDiscountByIdAsync(id);
            return Ok(discount);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPost("discount")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddDiscount([FromBody] DiscountCreateDto discountData)
    {
        try
        {
            var discount = await service.AddDiscountAsync(discountData);
            return CreatedAtAction(nameof(GetDiscountById), new { id = discount.Id }, discount);
        }
        catch (RecordAlreadyExistsException e)
        {
            return Conflict(e.Message);
        }
    }
}