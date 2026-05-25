using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using Inventory.Application.Interfaces;
using Inventory.Domain;

namespace Inventory.Infrastructure.Repositories;

public class InventoryTransactionRepository : IInventoryTransactionRepository
{
    private readonly string _connectionString;

    public InventoryTransactionRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlConnectionString") 
            ?? throw new ArgumentNullException(nameof(configuration), "La cadena de conexión no existe.");
    }

    public async Task<long> RegisterTransactionAsync(InventoryTransaction transaction)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var parameters = new DynamicParameters();

        parameters.Add("@ProductID", transaction.ProductID, DbType.Int32);
        parameters.Add("@TransactionTypeID", transaction.TransactionTypeID, DbType.Byte);
        parameters.Add("@Quantity", transaction.Quantity, DbType.Int32);
        parameters.Add("@ReferenceDocument", transaction.ReferenceDocument, DbType.String, size: 100);
        parameters.Add("@Notes", transaction.Notes, DbType.String, size: 500);

        return await connection.ExecuteScalarAsync<long>(
            "dbo.sp_Inventory_RegisterTransaction", 
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }
}