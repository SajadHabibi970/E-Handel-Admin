using EHandelApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EHandelApp.Data;

// Static class used for database seeding
public static class DbSeeder
{
    // Seeds database with sample data if empty run after mig
    public static async Task SeedAsync()
    {
        // Create new database context
        using var db = new ShopContext();

        // Apply migration before seeding
        await db.Database.MigrateAsync();

        // Add Category
        if (!await db.Categories.AnyAsync())
        {
            var toolsCategory = new Category { CategoryName = "Tools", CategoryDescription = "All Tools" };
            db.Categories.Add(toolsCategory);
            await db.SaveChangesAsync();
            Console.WriteLine($"Categories seeded to DB");
        }

        // Add product
        if (!await db.Products.AnyAsync())
        {
            var category = await db.Categories.FirstOrDefaultAsync();
            if (category != null)
            {
                db.Products.AddRange(
                    new Product { ProductName = "Hammer", ProductDescription = "Rubber mallet", ProductPrice = 250, StockQuantity = 10, CategoryId = category.CategoryId},
                    new Product { ProductName = "Saw", ProductDescription = "Chainsaw", ProductPrice = 500, StockQuantity = 20, CategoryId = category.CategoryId}
                );
                await db.SaveChangesAsync();
                Console.WriteLine($"Products seeded to DB");
            }
        }

        // Add customer
        if (!await db.Customers.AnyAsync())
        {
            db.Customers.AddRange(
                new Customer { FirstName = "John", LastName = "Doa", Email = "johndoe@hotmail.com", Address = "Stockholm"},
                new Customer { FirstName = "Jane", LastName = "Doe", Email = "janedoe@hotmail.com", Address = "Malmö"}
                );
            await db.SaveChangesAsync();
            Console.WriteLine("Cusomers seeded to DB");
        }

        // Add order
        if (!await db.Orders.AnyAsync())
        {
            var customer = await db.Customers.FirstOrDefaultAsync();
            if (customer != null)
            {
                db.Orders.AddRange(
                    new Order { OrderDate = DateTime.Today.AddDays(-1), Status = "Complete", TotalAmount = 500, CustomerId = customer.CustomerId},
                    new Order { OrderDate = DateTime.Today, Status = "Pending", TotalAmount = 1000, CustomerId = customer.CustomerId}
                );
                await db.SaveChangesAsync();
                Console.WriteLine("Orders seeded to DB");
            }
        }

        // Add OrderRow
        if (!await db.OrderRows.AnyAsync())
        {
            var order = await db.Orders.FirstOrDefaultAsync();
            var product = await db.Products.FirstOrDefaultAsync();
            if (order != null && product != null)
            {
                db.OrderRows.AddRange(
                    new OrderRow { Quantity = 2, UnitPrice = product.ProductPrice, OrderId = order.OrderId, ProductId = product.ProductId},
                    new OrderRow { Quantity = 1, UnitPrice = product.ProductPrice, OrderId = order.OrderId, ProductId = product.ProductId}
                );
                await db.SaveChangesAsync();
                Console.WriteLine("OrderRows seeded to DB");
            }
        }
    }
}