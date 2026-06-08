using Microsoft.AspNetCore.Mvc;
using Inventory.Application.Interfaces;
using Inventory.Domain;

namespace Inventory.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StocksController : ControllerBase
{
    private readonly IStockRepository _stockRepository;

    public StocksController(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Stock>>> GetAll()
    {
        var stocks = await _stockRepository.GetAllAsync();
        return Ok(stocks);
    }

    [HttpGet("{productId:int}")]
    public async Task<ActionResult<Stock>> GetByProductId(int productId)
    {
        var stock = await _stockRepository.GetByProductIdAsync(productId);
        
        if (stock == null)
        {
            return NotFound(new { message = $"No se encontraron existencias para el ProductID {productId}." });
        }

        return Ok(stock);
    }
}