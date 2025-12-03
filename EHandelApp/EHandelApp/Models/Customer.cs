using System.ComponentModel.DataAnnotations;

namespace EHandelApp.Models;

// Modell class for Customer
public class Customer
{
    public int CustomerId { get; set; }
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = null!;
    [Required, MaxLength(100)]
    public string LastName { get; set; } = null!;
    [Required, MaxLength(100)]
    public string Email { get; set; } = null!;

    [Required] 
    public string? Address { get; set; }

    public List<Order> Orders { get; set; } = new();
}