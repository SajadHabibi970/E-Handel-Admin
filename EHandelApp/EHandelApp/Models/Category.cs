using System.ComponentModel.DataAnnotations;

namespace EHandelApp.Models;

// Modell class for Category
public class Category
{
    public int CategoryId { get; set; }
    [Required, MaxLength(100)]
    public string CategoryName { get; set; } = null!;
    [MaxLength(250)]
    public string CategoryDescription { get; set; } = null!;
    
    public List<Product> Products { get; set; } = new();
}