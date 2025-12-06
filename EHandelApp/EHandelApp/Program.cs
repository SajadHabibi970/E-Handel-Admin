using EHandelApp.Data;
using EHandelApp.Models;
using Microsoft.EntityFrameworkCore;

// Run database seeding
await DbSeeder.SeedAsync();

while (true)
{
    // Menu för user interaction
    Console.WriteLine("\n Commands; 1: List Categories | 2: Add Category | 3: Edit Category | 4: Remove Category | " +
                      "\n5: List Product | 6: Add Product | 7: Edit Product | 8: Remove Product |" +
                      "\n9: List Customer | 10: Add Customer | 11: Edit Customer | 12: Delete Customer |" +
                      "\n13");
    Console.Write("> ");
    
    var line = Console.ReadLine() ??  string.Empty;

    // Skip empty lines
    if (string.IsNullOrEmpty(line))
    {
        continue;
    }

    // Exit program
    if (line.Equals("13", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }
    
    var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
    var cmd = parts[0];
    
    switch (cmd)
    {
        case "1":
            await ListCategoriesAsync();
            break;
        case "2":
            await AddCategoryAsync();
            break;
        case "3":
            await ListCategoriesAsync();
            Console.WriteLine("Enter the Category Id to edit");
            if (!int.TryParse(Console.ReadLine(), out var cId))
            {
                Console.WriteLine("Usage: edit <id>");
                break;
            }
            await EditCategoryAsync(cId);
            break;
        case "4":
            await ListCategoriesAsync();
            Console.WriteLine("Enter the Category Id to delete:");
            if (!int.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("Usage: edit <id>");
                break;
            }
            await DeleteCategoryAsync(id);
            break;
        case "5":
            await ListProductsAsync();
            break;
        case "6":
            await AddProductAsync();
            break;
        case "7":
            await ListProductsAsync();
            Console.WriteLine("Enter the Product Id to edit");
            if (!int.TryParse(Console.ReadLine(), out var pIdd))
            {
                Console.WriteLine("Usage: edit <id>");
                break;
            }
            await EditProductAsync(pIdd);
            break;
        case "8":
            await ListProductsAsync();
            Console.WriteLine("Enter the Product Id to delete:");
            if (!int.TryParse(Console.ReadLine(), out var pId))
            {
                Console.WriteLine("Usage: edit <id>");
            }
            await DeleteProductAsync(pId);
            break;
        case "9":
            await ListCustomersAsync();
            break;
        default:
            Console.WriteLine("Unknown command");
            break;
        
    }
}
/*
 * Retrives all categories from the database and prints them in a formatted list.
 */
static async Task ListCategoriesAsync()
{
    using var db = new ShopContext();
    var rows = await db.Categories.AsNoTracking().OrderBy(category => category.CategoryId).ToListAsync();
    Console.WriteLine("\nId | Name | Description");
    foreach (var row in rows)
    {
        Console.WriteLine($"{row.CategoryId} | {row.CategoryName}  | {row.CategoryDescription}");
    }
}
/*
 * Adds new category.
 * Prompts the user for input, validates it and inserts a new category into the database.
*/
static async Task AddCategoryAsync()
{
    Console.WriteLine("Category Name: ");
    var name = Console.ReadLine()?.Trim() ?? string.Empty;

    if (string.IsNullOrEmpty(name) || name.Length > 100)
    {
        Console.WriteLine("Name is required (max 100 characters)");
        return;
    }
    
    Console.WriteLine("Description (Optional): ");
    var desc = Console.ReadLine()?.Trim() ?? string.Empty;
    
    using var db = new ShopContext();
    db.Categories.Add(new Category { CategoryName = name, CategoryDescription = desc });

    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Category added");
    }
    catch (DbUpdateException exception)
    {
        Console.WriteLine("DB error (Maybe dublicate)!" + exception.GetBaseException().Message);
    }
}
/*
 * Edit an existing category.
 * Loads the category by id, asks the user for new values, validates input and updates only
 * the fields that were changed.
 */
static async Task EditCategoryAsync(int id)
{
    using var db = new ShopContext();
    var category = await db.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);

    if (category == null)
    {
        Console.WriteLine("Category not found");
        return;
    }
    
    Console.WriteLine(category.CategoryName);
    var name = Console.ReadLine()?.Trim() ?? string.Empty;

    if (!string.IsNullOrEmpty(name))
    {
        if (name.Length > 100)
        {
            Console.WriteLine("Name is required (max 100 characters)");
            return;
        }
        category.CategoryName = name;
    }
    
    Console.WriteLine(category.CategoryDescription);
    var desc = Console.ReadLine()?.Trim() ?? string.Empty;

    if (!string.IsNullOrEmpty(desc))
    {
        category.CategoryDescription = desc;
    }

    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Category edited");
    }
    catch (DbUpdateException exception)
    {
        Console.WriteLine("DB error:" + exception.GetBaseException().Message);
    }
}
/*
 * Deletes a category by its id.
 * Handles errors such as foreign key constraints when a category is in use by products.
 */
static async Task DeleteCategoryAsync(int id)
{
    using var db = new ShopContext();
    var category = await db.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
    if (category == null)
    {
        Console.WriteLine("Category not found");
        return;
    }
    db.Categories.Remove(category);

    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Category deleted");
    }
    catch (DbUpdateException exception)
    {
        Console.WriteLine(exception.Message);
    }
}

static async Task ListProductsAsync()
{
    using var db = new ShopContext();
    var rows = await db.Products.AsNoTracking().ToListAsync();
    Console.WriteLine("\nId | Name | Description | Price | Stock Quantity");
    foreach (var row in rows)
    {
        Console.WriteLine($"{row.ProductId} | {row.ProductName} | {row.ProductDescription} | {row.ProductPrice} kr | {row.StockQuantity} st");
    }
}

static async Task AddProductAsync()
{
    using var db = new ShopContext();
    
    var categories = await db.Categories.AsNoTracking()
        .OrderBy(c => c.CategoryId)
        .ToListAsync();

    if (!categories.Any())
    {
        Console.WriteLine("No categories exist. Add a Category first");
        return;
    }
    Console.WriteLine("\nAvailable categories: ");
    foreach (var c in categories)
    {
        Console.WriteLine($"{c.CategoryId} | {c.CategoryName}");
    }
    
    Console.WriteLine("Enter category Id: ");
    if (!int.TryParse(Console.ReadLine(), out var categoryId)
        || !categories.Any(c => c.CategoryId == categoryId))
    {
        Console.WriteLine("Invalid category id");
        return;
    }
    
    Console.WriteLine("Product Name: ");
    var pName = Console.ReadLine()?.Trim() ?? string.Empty;
    if (string.IsNullOrEmpty(pName) || pName.Length > 100)
    {
        Console.WriteLine("Product Name is required (max 100 characters)");
        return;
    }
    Console.WriteLine("Description (Optional): ");
    var pDesc = Console.ReadLine()?.Trim() ?? string.Empty;
    if (pDesc.Length > 250)
    {
        Console.WriteLine("Product Description is required (max 250 characters)");
    }
    Console.WriteLine("Price: ");
    if (!decimal.TryParse(Console.ReadLine(), out var pPrice) || pPrice <= 0)
    {
        Console.WriteLine("Price must be greater than 0");
        return;
    }
    Console.WriteLine("Stock Quantity: ");
    if (!int.TryParse(Console.ReadLine(), out var pStockQuantity) || pStockQuantity <= 0)
    {
        Console.WriteLine("Stock Quantity must be greater than 0");
        return;
    }
    
    db.Products.Add(new Product
        { 
            ProductName = pName,
            ProductDescription = pDesc, 
            ProductPrice = pPrice, 
            StockQuantity = pStockQuantity,
            CategoryId = categoryId
        });

    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Product added");
    }
    catch (DbUpdateException exception)
    {
        Console.WriteLine("DB error"+ exception.GetBaseException().Message);
    }

}

static async Task EditProductAsync(int pid)
{
    using var db = new ShopContext();
    var product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == pid);
    if (product == null)
    {
        Console.WriteLine("Product not found");
        return;
    }
    Console.WriteLine($"Name: {product.ProductName}");
    var pName = Console.ReadLine()?.Trim() ?? string.Empty;
    if (!string.IsNullOrEmpty(pName))
    {
        product.ProductName = pName;
    }
    Console.WriteLine($"Description: {product.ProductDescription}");
    var pDesc = Console.ReadLine()?.Trim() ?? string.Empty;
    if (!string.IsNullOrWhiteSpace(pDesc))
    {
        product.ProductDescription = pDesc;
    }
    Console.WriteLine($"Price: {product.ProductPrice} kr");
    var pPrice = Console.ReadLine()?.Trim() ?? string.Empty;
    if (!string.IsNullOrEmpty(pPrice))
    {
        if (decimal.TryParse(pPrice, out var pPriceDecimal) && pPriceDecimal > 0)
        {
            product.ProductPrice = pPriceDecimal;
        }

        else
        {
            Console.WriteLine("Invalid price. Keeping old price.");
        }
    }
    Console.WriteLine($"Stock Quantity: {product.StockQuantity} st");
    var pStockQuantity = Console.ReadLine()?.Trim() ?? string.Empty;
    if (!string.IsNullOrEmpty(pStockQuantity))
    {
        if (int.TryParse(pStockQuantity, out var pStockQuantityInt) && pStockQuantityInt >= 0)
        {
            product.StockQuantity = pStockQuantityInt;
        }
        else
        {
            Console.WriteLine("Invalid stock quantity.");
        }
    }
    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Product edited");
    }
    catch (DbUpdateException exception)
    {
        Console.WriteLine("DB error"+ exception.GetBaseException().Message);
    }
}

static async Task DeleteProductAsync(int pidd)
{
    using var db = new ShopContext();
    var product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == pidd);
    if (product == null)
    {
        Console.WriteLine("Product not found");
        return;
    }
    db.Products.Remove(product);
    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Product deleted");
    }
    catch (DbUpdateException exception)
    {
        Console.WriteLine("DB error"+ exception.GetBaseException().Message);
    }
}

static async Task ListCustomersAsync()
{
    using var db = new ShopContext();
    var customers = await db.Customers.AsNoTracking()
        .OrderBy(c => c.CustomerId)
        .ToListAsync();
    Console.WriteLine($"\nId | FirstName | LastName | Email | Address");
    foreach (var customer in customers)
    {
        Console.WriteLine($"{customer.CustomerId} | {customer.FirstName} | {customer.LastName} | {customer.Email} | {customer.Address}");
    }
}