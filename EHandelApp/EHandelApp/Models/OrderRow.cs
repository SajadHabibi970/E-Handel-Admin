using System.ComponentModel.DataAnnotations;

namespace EHandelApp.Models;

// Modell class for OrderRow
public class OrderRow
{
    public int OrderRowId { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    [Required]
    public int Quantity { get; set; }
    [Required]
    public decimal UnitPrice { get; set; }
    
    public Order? Order { get; set; }
    public Product? Product { get; set; }
}