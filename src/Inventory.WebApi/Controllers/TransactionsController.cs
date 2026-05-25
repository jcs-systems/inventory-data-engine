using Microsoft.AspNetCore.Mvc;
using Inventory.Application.Interfaces;
using Inventory.Domain;

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
    public async Task<IActionResult> CreateTransaction([FromBody] InventoryTransaction transaction)
    {
        if (transaction == null)
        {
            return BadRequest("Los datos de la transacción son nulos.");
        }

        try
        {
            long transactionId = await _transactionRepository.RegisterTransactionAsync(transaction);
            
            return CreatedAtAction(nameof(CreateTransaction), new { id = transactionId }, new { id = transactionId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno al procesar el Kardex: {ex.Message}");
        }
    }
}