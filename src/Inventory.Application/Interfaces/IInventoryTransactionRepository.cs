using Inventory.Application.DTOs;

namespace Inventory.Application.Interfaces;

public interface IInventoryTransactionRepository
{
    Task<int> CreateAsync(TransactionCreateDto transaction);
    Task<IEnumerable<TransactionReportDto>> GetReportAsync(TransactionFilterDto filter);
}