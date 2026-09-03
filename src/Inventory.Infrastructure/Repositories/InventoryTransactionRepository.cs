using System.Data;
using Dapper;
using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Inventory.Infrastructure.Repositories;

public class InventoryTransactionRepository : IInventoryTransactionRepository
{
    private readonly string _connectionString;
    private readonly IEmailService _emailService; // 1. Campo privado

    // 2. Inyección en el constructor
    public InventoryTransactionRepository(IConfiguration configuration, IEmailService emailService)
    {
        _connectionString = configuration.GetConnectionString("SqlConnectionString") 
            ?? throw new ArgumentNullException(nameof(configuration), "La cadena de conexión no existe.");
        _emailService = emailService;
    }

    /// <summary>
/// Registra una transacción ejecutando el Stored Procedure en SQL Server.
/// </summary>
public async Task<int> CreateAsync(TransactionCreateDto transaction)
{
    using var connection = new SqlConnection(_connectionString);

    var parameters = new DynamicParameters();
    parameters.Add("@ProductID", transaction.ProductID, DbType.Int32);
    parameters.Add("@TransactionTypeID", transaction.TransactionTypeID, DbType.Byte);
    parameters.Add("@Quantity", transaction.Quantity, DbType.Int32);
    parameters.Add("@ReferenceDocument", transaction.ReferenceDocument, DbType.String, size: 100);
    parameters.Add("@Notes", transaction.Notes, DbType.String, size: 500);

    int transactionId = await connection.ExecuteScalarAsync<int>(
        "dbo.sp_Inventory_RegisterTransaction", 
        parameters,
        commandType: CommandType.StoredProcedure
    );

    // Consulta con los nombres de columna reales de tu esquema
    var stockQuery = @"
        SELECT 
            s.Quantity, 
            p.MinimumStock, 
            p.ProductName, 
            p.SKU 
        FROM dbo.Stocks s
        INNER JOIN dbo.Products p ON s.ProductID = p.ProductID
        WHERE s.ProductID = @ProductID";

    var stockInfo = await connection.QueryFirstOrDefaultAsync<dynamic>(stockQuery, new { ProductID = transaction.ProductID });

    // Evaluamos si el stock actualizado cayó al mínimo o por debajo
    if (stockInfo != null && stockInfo.Quantity <= stockInfo.MinimumStock)
    {
        string subject = $"⚠️ Alerta de Stock Mínimo: {stockInfo.ProductName}";
        string body = $@"
            <h2>Alerta de Reabastecimiento de Inventario</h2>
            <p>El producto <strong>{stockInfo.ProductName}</strong> (SKU: {stockInfo.SKU}) ha alcanzado su nivel mínimo o está por debajo del límite.</p>
            <ul>
                <li><strong>Stock Actual:</strong> {stockInfo.Quantity}</li>
                <li><strong>Stock Mínimo Permitido:</strong> {stockInfo.MinimumStock}</li>
            </ul>
            <p>Por favor, coordine la compra o reabastecimiento a la brevedad.</p>";

        await _emailService.SendEmailAsync("engineer@jcsystems.me", subject, body);
    }

    return transactionId;
}

    /// <summary>
    /// Consulta el reporte Kardex aplicando filtros dinámicos en C# mediante Dapper.
    /// </summary>
    public async Task<IEnumerable<TransactionReportDto>> GetReportAsync(TransactionFilterDto filter)
    {
        using var connection = new SqlConnection(_connectionString);

        var sql = @"
            SELECT 
                t.TransactionID,
                t.ProductID,
                p.ProductName,
                p.SKU,
                t.TransactionTypeID,
                tt.TypeName AS TypeName,
                t.Quantity,
                t.ReferenceDocument,
                t.Notes,
                t.TransactionDate AS CreatedAt
            FROM dbo.InventoryTransactions t
            INNER JOIN dbo.Products p ON t.ProductID = p.ProductID
            INNER JOIN dbo.TransactionTypes tt ON t.TransactionTypeID = tt.TransactionTypeID
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (filter.ProductID.HasValue)
        {
            sql += " AND t.ProductID = @ProductID";
            parameters.Add("ProductID", filter.ProductID.Value);
        }

        if (filter.StartDate.HasValue)
        {
            sql += " AND t.TransactionDate >= @StartDate";
            parameters.Add("StartDate", filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            sql += " AND t.TransactionDate <= @EndDate";
            parameters.Add("EndDate", filter.EndDate.Value);
        }

        sql += " ORDER BY t.TransactionDate DESC;";

        return await connection.QueryAsync<TransactionReportDto>(sql, parameters);
    }
}