namespace Inventory.Application.DTOs;

public class TransactionCreateDto
{
    public int ProductID { get; set; }
    public byte TransactionTypeID { get; set; }
    public int Quantity { get; set; }
    public string? ReferenceDocument { get; set; }
    public string? Notes { get; set; }
}