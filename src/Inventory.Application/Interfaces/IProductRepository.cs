using Inventory.Domain;

namespace Inventory.Application.Interfaces;

public interface IProductRepository
{
    Task<int> CreateProductAsync(Product product);
}