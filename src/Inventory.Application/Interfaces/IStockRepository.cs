using Inventory.Domain;

namespace Inventory.Application.Interfaces;

public interface IStockRepository
{
    Task<Stock?> GetByProductIdAsync(int productId);
    
    Task<IEnumerable<Stock>> GetAllAsync();
}