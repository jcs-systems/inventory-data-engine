using Microsoft.AspNetCore.Mvc;
using Inventory.Application.Interfaces;
using Inventory.Domain;

namespace Inventory.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        try
        {
            if (product == null)
            {
                return BadRequest("Product data is required.");
            }

            int newProductId = await _productRepository.CreateProductAsync(product);

            return CreatedAtAction(nameof(CreateProduct), new { id = newProductId }, new { ProductID = newProductId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}