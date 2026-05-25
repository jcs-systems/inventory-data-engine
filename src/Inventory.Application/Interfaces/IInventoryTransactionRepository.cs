using Inventory.Domain;

namespace Inventory.Application.Interfaces;

public interface IInventoryTransactionRepository
{
    Task<long> RegisterTransactionAsync(InventoryTransaction transaction);
}