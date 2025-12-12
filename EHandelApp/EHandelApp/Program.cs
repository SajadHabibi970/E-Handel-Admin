using EHandelApp;
using EHandelApp.Data;
using EHandelApp.Models;
using Microsoft.EntityFrameworkCore;

// Run database seeding
await DbSeeder.SeedAsync();

while (true)
{
    // Menu för user interaction
    Console.WriteLine("\n Commands; 1: List Categories | 2: Add Category | 3: Edit Category | 4: Remove Category | " +
                      "\n5: List Product | 6: Add Product | 7: Edit Product | 8: Remove Product | 9: Search Product | " +
                      "\n10: List Customer | 11: Add Customer |" +
                      "\n12: List Order | 13: List OrderSummery | 14: Add Order | 15: OrderDetails|" +
                      "\n16: List OrderRow | 17: Add OrderRow |" +
                      "\n18 = Exit");
    Console.Write("> ");
    
    var line = Console.ReadLine() ??  string.Empty;

    // Skip empty lines
    if (string.IsNullOrEmpty(line))
    {
        continue;
    }

    // Exit program
    if (line.Equals("18", StringComparison.OrdinalIgnoreCase))
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
            await SearchProductsAsync();
            break;
        case "10":
            await ListCustomersAsync();
            break;
        case "11":
            await AddCustomerAsync();
            break;
        case "12":
            await ListOrdersAsync();
            break;
        case "13":
            await ListOrderSummeryViewAsync();
            break;
        case "14":
            await AddOrderAsync();
            break;
        case "15":
            await OrderDetailsAsync();
            break;
        case "16":
            await ListOrderRowAsync();
            break;
        case "17":
            await AddOrderRowAsync();
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

static async Task SearchProductsAsync()
{
    Console.WriteLine("Search for products:");
    var nameP = Console.ReadLine()?.Trim()?? string.Empty;
    if (string.IsNullOrEmpty(nameP) || nameP.Length > 100)
    {
        Console.WriteLine("Product name is required (max 100 characters)");
    }
    
    using var db = new ShopContext();
    var products = await db.Products
        .Where(p => p.ProductName.Contains(nameP))
        .ToListAsync();
    if (!products.Any())
    {
        Console.WriteLine("Product not found");
    }

    foreach (var p in products)
    {
        Console.WriteLine($"{p.ProductName} | {p.StockQuantity} | {p.ProductPrice}");
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
        Console.WriteLine($"" +
                          $"{customer.CustomerId} | " +
                          $"{customer.FirstName} | {customer.LastName} | " +
                          $"{EncryptionHelper.Encrypt(customer.Email)} |" +
                          $"{customer.Address}");
    }
}

static async Task AddCustomerAsync()
{
    Console.WriteLine("First Name: ");
    var fName = Console.ReadLine()?.Trim()??string.Empty;
    if (string.IsNullOrEmpty(fName) || fName.Length > 100)
    {
        Console.WriteLine("Invalid first name.");
        return;
    }
    
    Console.WriteLine("Last Name: ");
    var lName = Console.ReadLine()?.Trim()??string.Empty;
    if (string.IsNullOrEmpty(lName) || lName.Length > 100)
    {
        Console.WriteLine("Invalid last name.");
        return;
    }
    
    Console.WriteLine("Email: ");
    var email = Console.ReadLine()?.Trim()??string.Empty;
    if (string.IsNullOrEmpty(email) || email.Length > 100 || !email.Contains("@"))
    {
        Console.WriteLine("Invalid email.");
        return;
    }
    
    Console.WriteLine("Password: ");
    var password = Console.ReadLine()?.Trim()??string.Empty;
    if (string.IsNullOrEmpty(password))
    {
        Console.WriteLine("Invalid password.");
        return;
    }

    var salt = HashingHelper.GenerateSalt();
    var hash = HashingHelper.HashWithSalt(password, salt);
    
    Console.WriteLine("Address: ");
    var address = Console.ReadLine()?.Trim()??string.Empty;
    if (string.IsNullOrEmpty(address))
    {
        Console.WriteLine("Address is required.");
        return;
    }
    
    using var db = new ShopContext();
    db.Customers.Add(new Customer
    {
        FirstName = fName,
        LastName = lName,
        Email = EncryptionHelper.Encrypt(email),
        Address = address,
        PasswordSalt = salt,
        PasswordHash = hash
    });

    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Customer added");
    }
    catch (DbUpdateException exception)
    {
        Console.WriteLine("DB error"+ exception.GetBaseException().Message);
    }
}

static async Task ListOrdersAsync()
{
    using var db = new ShopContext();
    var orders = await db.Orders.AsNoTracking()
        .OrderBy(o => o.OrderId)
        .Include(order => order.Customer)
        .ToListAsync();
    Console.WriteLine("Id | OrderDate | Customer | Status | TotalAmount");
    foreach (var order in orders)
    {                                                                              
        Console.WriteLine($"{order.OrderId} | {order.OrderDate:yyyy-MM-dd} | " +
                          $"{order.Customer?.FirstName} {order.Customer?.LastName} | " +
                          $"{order.Status} | {order.TotalAmount}");
    }
}

static async Task ListOrderSummeryViewAsync()
{
    using var db = new ShopContext();
    var summaries = await db.OrderSummeries
        .OrderByDescending(o => o.OrderDate)
        .ToListAsync();
    Console.WriteLine("OrderId |OrderDate | CustomeName| CusomerEmail | TotalAmount SEK");
    {
        foreach (var summary in summaries)
        {
            Console.WriteLine($"{summary.OrderId} | " +
                              $"{summary.OrderDate:yyyy-MM-dd} | " +
                              $"{summary.CustomerName} | " +
                              $"{summary.CustomerEmail} | " +
                              $"{summary.TotalAmount} SEK");
        }
    }
}

static async Task AddOrderAsync()
{
    using var db = new ShopContext();
    var customers = await db.Customers.AsNoTracking()
        .OrderBy(c => c.CustomerId)
        .ToListAsync();
    if (!customers.Any())
    {
        Console.WriteLine("No customer exist. Add a customer first");
        return;
    }
    Console.WriteLine("\nAvailable customers: ");
    foreach (var customer in customers)
    {
        Console.WriteLine($"{customer.CustomerId} | {customer.FirstName} | {customer.LastName} | {customer.Email}");
    }
    
    Console.WriteLine("Enter customer id: ");
    if (!int.TryParse(Console.ReadLine(), out var customerId)
        || !customers.Any(c => c.CustomerId == customerId))
    {
        Console.WriteLine("Invalid customer id.");
        return;
    }

    var order = new Order
    {
        CustomerId = customerId,
        OrderDate = DateTime.Now,
        Status = "Pending",
        TotalAmount = 0
    };
    db.Orders.Add(order);
    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Order added");
    }
    catch (DbUpdateException exception)
    {
        Console.WriteLine("DB error"+ exception.GetBaseException().Message);
    }
}

static async Task OrderDetailsAsync()
{
    using var db = new ShopContext();

    await ListOrdersAsync();
    
    Console.WriteLine("Enter order id to see details: ");
    if (!int.TryParse(Console.ReadLine(), out var orderId)
        || !db.Orders.Any(o => o.OrderId == orderId))
    {
        Console.WriteLine("Invalid order id.");
        return;
    }

    var orders = await db.Orders
        .Include(o => o.Customer)
        .Include(o => o.OrderRows)
        .ThenInclude(or => or.Product)
        .FirstOrDefaultAsync(o => o.OrderId == orderId);
    if (orders == null)
    {
        Console.WriteLine("No order found");
        return;
    }
    Console.WriteLine($"Order Id: {orders.OrderId}" + 
                      $"\nDate: {orders.OrderDate:yyyy-MM-dd}" +
                      $"\nStatus: {orders.Status}" +
                      $"\nCustomer: {orders.Customer?.FirstName} {orders.Customer?.LastName}" +
                      $"\nTotal Amount: {orders.TotalAmount} kr");
    
    Console.WriteLine("Products: ");
    Console.WriteLine("Name | Price | Quantity");
    foreach (var p in orders.OrderRows)
    {
        Console.WriteLine($"{p.Product?.ProductName} | {p.Product?.ProductPrice} kr | {p.Quantity} st");
    }
    
}

static async Task ListOrderRowAsync()
{
    using var db = new ShopContext();
    var orderrows = await db.OrderRows.AsNoTracking()
        .OrderBy(or => or.OrderRowId)
        .Include(o => o.Order).ThenInclude(order => order.Customer)
        .Include(p => p.Product)
        .ToListAsync();
    Console.WriteLine("Id | OrderId | Product | Quantity | Unit Price");
    foreach (var row in orderrows)
    {
        Console.WriteLine($"{row.OrderRowId} | {row.OrderId} | {row.Product?.ProductName} |" +
                          $" {row.Quantity} | {row.UnitPrice}");
    }
}

static async Task AddOrderRowAsync()
{
    using var db = new ShopContext();
    var orders = await db.Orders.AsNoTracking()
        .OrderBy(o => o.OrderId)
        .Include(c => c.Customer)
        .ToListAsync();
    if (!orders.Any())
    {
        Console.WriteLine("No order exist. Add a order first");
        return;
    }

    Console.WriteLine("\nAvailable orders: ");
    foreach (var order in orders)
    {
        Console.WriteLine($"{order.OrderId} | {order.Customer?.FirstName} {order.Customer?.LastName} | {order.OrderDate:yyyy-MM-dd}");
    }

    Console.WriteLine("Enter order id: ");
    if (!int.TryParse(Console.ReadLine(), out var orderId)
        || !orders.Any(o => o.OrderId == orderId))
    {
        Console.WriteLine("Invalid order id.");
        return;
    }

    var products = await db.Products.AsNoTracking()
        .OrderBy(p => p.ProductId)
        .ToListAsync();
    if (!products.Any())
    {
        Console.WriteLine("Product not found");
        return;
    }

    Console.WriteLine("\nAvailable products: ");
    foreach (var product in products)
    {
        Console.WriteLine($"{product.ProductId} | {product.ProductName} | {product.ProductPrice}");
    }

    Console.WriteLine("Enter product id: ");

    if (!int.TryParse(Console.ReadLine(), out var productId)
        || !products.Any(p => p.ProductId == productId))
    {
        Console.WriteLine("Invalid product id.");
        return;
    }
    Console.WriteLine("Enter quantity: ");
    if (!int.TryParse(Console.ReadLine(), out var quantity)
        || quantity < 1)
    {
        Console.WriteLine("Invalid quantity( Quantity should be greater than zero)");
        return;
    }
    
    var productEntity = await db.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
    
    var orderrow = new OrderRow
    {
        OrderId = orderId,
        ProductId = productId,
        Quantity = quantity,
        UnitPrice = productEntity.ProductPrice
    };
    var orderToUpdate = await db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
    if (orderToUpdate != null)
    {
        orderToUpdate.TotalAmount += quantity * productEntity.ProductPrice;
    }
    
    db.OrderRows.Add(orderrow);

    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Orderrow added");
    }
    catch (DbUpdateException exception)
    {
        Console.WriteLine("DB error"+ exception.GetBaseException().Message);
    }
}
