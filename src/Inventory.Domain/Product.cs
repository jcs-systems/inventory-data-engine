namespace Inventory.Domain;

public class Product
{
    public int ProductID { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryID { get; set; }
    public decimal Cost { get; set; }
    public decimal UnitPrice { get; set; }
    public int MinimumStock { get; set; }
    public DateTime CreatedAt { get; set; }
}