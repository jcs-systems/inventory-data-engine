namespace Inventory.Domain;

public class Stock
{
    public int StockID { get; set; }
    public int ProductID { get; set; }
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }
}