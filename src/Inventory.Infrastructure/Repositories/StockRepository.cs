using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using Inventory.Application.Interfaces;
using Inventory.Domain;

namespace Inventory.Infrastructure.Repositories;

public class StockRepository : IStockRepository
{
    private readonly string _connectionString;

    public StockRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Stock?> GetByProductIdAsync(int productId)
    {
        const string query = @"
            SELECT StockID, ProductID, Quantity, LastUpdated 
            FROM dbo.Stocks 
            WHERE ProductID = @ProductId;";

        using IDbConnection db = new SqlConnection(_connectionString);
        
        return await db.QueryFirstOrDefaultAsync<Stock>(query, new { ProductId = productId });
    }

    public async Task<IEnumerable<Stock>> GetAllAsync()
    {
        const string query = @"
            SELECT StockID, ProductID, Quantity, LastUpdated 
            FROM dbo.Stocks;";

        using IDbConnection db = new SqlConnection(_connectionString);
        
        return await db.QueryAsync<Stock>(query);
    }
}