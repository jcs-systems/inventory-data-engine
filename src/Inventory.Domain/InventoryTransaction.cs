namespace Inventory.Domain;

public class InventoryTransaction
{
    public long TransactionID { get; set; }
    public int ProductID { get; set; }
    public byte TransactionTypeID { get; set; }
    public int Quantity { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? ReferenceDocument { get; set; }
    public string? Notes { get; set; }
}