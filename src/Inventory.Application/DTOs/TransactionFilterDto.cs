namespace Inventory.Application.DTOs;

public class TransactionFilterDto
{
    public int? ProductID { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}