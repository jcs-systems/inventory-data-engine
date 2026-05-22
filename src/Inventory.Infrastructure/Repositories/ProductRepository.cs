using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using Inventory.Application.Interfaces;
using Inventory.Domain;

namespace Inventory.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int> CreateProductAsync(Product product)
    {
        using var connection = new SqlConnection(_connectionString);

        var parameters = new DynamicParameters();
        parameters.Add("@SKU", product.SKU, DbType.String, ParameterDirection.Input, 50);
        parameters.Add("@ProductName", product.ProductName, DbType.String, ParameterDirection.Input, 150);
        parameters.Add("@Description", product.Description, DbType.String, ParameterDirection.Input, -1);
        parameters.Add("@CategoryID", product.CategoryID, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@Cost", product.Cost, DbType.Decimal, ParameterDirection.Input);
        parameters.Add("@UnitPrice", product.UnitPrice, DbType.Decimal, ParameterDirection.Input);
        parameters.Add("@MinimumStock", product.MinimumStock, DbType.Int32, ParameterDirection.Input);

        return await connection.ExecuteScalarAsync<int>(
            "dbo.sp_Products_Create",
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }
}