using EHandelApp.Data;
using EHandelApp.Models;
using Microsoft.EntityFrameworkCore;

// Run database seeding
await DbSeeder.SeedAsync();

while (true)
{
    // Menu för user interaction
    Console.WriteLine("\n Commands; 1: List Categories | 2: Add Category | 3: Edit Category | 4: Remove Category | 5: Exit");
    Console.Write("> ");
    
    var line = Console.ReadLine() ??  string.Empty;

    // Skip empty lines
    if (string.IsNullOrEmpty(line))
    {
        continue;
    }

    // Exit program
    if (line.Equals("5", StringComparison.OrdinalIgnoreCase))
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