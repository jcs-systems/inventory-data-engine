using Microsoft.AspNetCore.Mvc;
using Inventory.Application.Interfaces;
using Inventory.Application.DTOs;

namespace Inventory.WebApi.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly IInventoryTransactionRepository _transactionRepository;

    public TransactionsController(IInventoryTransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionCreateDto dto)
    {
        if (dto == null)
        {
            return BadRequest("Los datos de la transacción son nulos.");
        }

        try
        {
            int transactionId = await _transactionRepository.CreateAsync(dto);
            
            return CreatedAtAction(nameof(CreateTransaction), new { id = transactionId }, new { id = transactionId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno al procesar el Kardex: {ex.Message}");
        }
    }

    [HttpGet("report")]
    public async Task<IActionResult> GetReport([FromQuery] TransactionFilterDto filter)
    {
        try
        {
            var report = await _transactionRepository.GetReportAsync(filter);
            return Ok(report);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno al generar el reporte: {ex.Message}");
        }
    }
}