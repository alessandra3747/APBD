using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevenueRecognitionApi.DTOs;
using RevenueRecognitionApi.Exceptions;
using RevenueRecognitionApi.Services;

namespace RevenueRecognitionApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ContractsController(IDbService service) : ControllerBase
{
    
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetContractById(int id)
    {
        try
        {
            var contract = await service.GetContractByIdAsync(id);
            return Ok(contract);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> AddContract([FromBody] ContractCreateDto contractData)
    {
        try
        {
            var contract = await service.AddContractAsync(contractData);
            return CreatedAtAction(nameof(GetContractById), new { id = contract.Id }, contract);
        }
        catch (RecordAlreadyExistsException e)
        {
            return Conflict(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return Conflict(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteContractById(int id)
    {
        try
        {
            await service.DeleteContractByIdAsync(id);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return Conflict(e.Message);
        }
    }

    
    [HttpGet("payment/{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetContractPaymentById(int id)
    {
        try
        {
            var payment = await service.GetContractPaymentByIdAsync(id);
            return Ok(payment);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPost("payment")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> AddContractPayment([FromBody] ContractPaymentCreateDto contractPaymentData)
    {
        try
        {
            var payment = await service.AddContractPaymentAsync(contractPaymentData);
            return CreatedAtAction(nameof(GetContractPaymentById), new { id = payment.Id }, payment);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return Conflict(e.Message);
        }
    }
}