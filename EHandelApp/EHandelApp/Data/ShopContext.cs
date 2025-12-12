using EHandelApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EHandelApp.Data;

public class ShopContext : DbContext
{
    // Database tables
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderRow> OrderRows => Set<OrderRow>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    /// <summary>
    /// Set up SqLite as the database
    /// Create the SQLite database file inside the applications directory
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "EHandelApp.Data.db");
        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }

    // Configure table structure, relationships and constraints
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(c =>
        {
            c.HasKey(x => x.CustomerId);
            c.Property(x => x.FirstName)
                .IsRequired().HasMaxLength(100);
            c.Property(x => x.LastName)
                .IsRequired().HasMaxLength(100);
            c.Property(x => x.Email)
                .IsRequired().HasMaxLength(100);
            c.Property(x => x.PasswordHash)
                .IsRequired().HasMaxLength(256);
            c.Property(x => x.PasswordSalt)
                .IsRequired().HasMaxLength(128);
            c.Property(x => x.Address)
                .IsRequired();

            c.HasIndex(x => x.Email)
                .IsUnique();
        });

        modelBuilder.Entity<Order>(o =>
        {
            o.HasKey(x => x.OrderId);
            o.Property(x => x.OrderDate)
                .IsRequired();
            o.Property(x => x.Status)
                .IsRequired();
            o.Property(x => x.TotalAmount)
                .IsRequired();

            o.HasOne(x => x.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderRow>(or =>
        {
            or.HasKey(x => x.OrderRowId);
            or.Property(x => x.Quantity)
                .IsRequired();
            or.Property(x => x.UnitPrice)
                .IsRequired();

            or.HasOne(x => x.Order)
                .WithMany(o => o.OrderRows)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            or.HasOne(x => x.Product)
                .WithMany(p => p.OrderRows)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Product>(p =>
        {
            p.HasKey(x => x.ProductId);
            p.Property(x => x.ProductName)
                .IsRequired().HasMaxLength(100);
            p.Property(x => x.ProductDescription)
                .HasMaxLength(250);
            p.Property(x => x.ProductPrice)
                .IsRequired();
            p.Property(x => x.StockQuantity)
                .IsRequired();

            p.HasOne(x => x.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>(c =>
        {
            c.HasKey(x => x.CategoryId);
            c.Property(x => x.CategoryName)
                .IsRequired().HasMaxLength(100);
            c.Property(x => x.CategoryDescription)
                .HasMaxLength(250);
            
            c.HasIndex(x => x.CategoryName)
                .IsUnique();
        });
    }
}