using Microsoft.EntityFrameworkCore;

namespace EHandelApp.Models;

[Keyless]
public class OrderSummery
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}