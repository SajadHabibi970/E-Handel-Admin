using System.ComponentModel.DataAnnotations;

namespace EHandelApp.Models;

// Modell class for Product
public class Product
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    [Required, MaxLength(100)]
    public string ProductName { get; set; } = null!;
    [MaxLength(250)]
    public string ProductDescription { get; set; } = null!;
    [Required]
    public decimal ProductPrice { get; set; }
    [Required]
    public int StockQuantity { get; set; }
    
    public Category? Category { get; set; }
    public List<OrderRow> OrderRows { get; set; } = new();
}