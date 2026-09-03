namespace Inventory.Application.DTOs;

public class TransactionReportDto
{
    public int TransactionID { get; set; }
    public int ProductID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int TransactionTypeID { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? ReferenceDocument { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}