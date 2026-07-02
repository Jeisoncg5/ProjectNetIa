using Microsoft.EntityFrameworkCore;
using ProjectNetIa.Domain.Entities;

namespace ProjectNetIa.Infrastructure.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerDocumentType> CustomerDocumentTypes => Set<CustomerDocumentType>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    public DbSet<InventoryMovementType> InventoryMovementTypes => Set<InventoryMovementType>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceStatus> InvoiceStatuses => Set<InvoiceStatus>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<SaleOrigin> SaleOrigins => Set<SaleOrigin>();
    public DbSet<SaleStatus> SaleStatuses => Set<SaleStatus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasOne(product => product.ProductCategory)
            .WithMany(category => category.Products)
            .HasForeignKey(product => product.ProductCategoryId);

        modelBuilder.Entity<Inventory>()
            .HasOne(inventory => inventory.Product)
            .WithOne(product => product.Inventory)
            .HasForeignKey<Inventory>(inventory => inventory.ProductId);

        modelBuilder.Entity<InventoryMovement>()
            .HasOne(movement => movement.Product)
            .WithMany(product => product.InventoryMovements)
            .HasForeignKey(movement => movement.ProductId);

        modelBuilder.Entity<InventoryMovement>()
            .HasOne(movement => movement.InventoryMovementType)
            .WithMany(type => type.InventoryMovements)
            .HasForeignKey(movement => movement.InventoryMovementTypeId);

        modelBuilder.Entity<Customer>()
            .HasOne(customer => customer.CustomerDocumentType)
            .WithMany(type => type.Customers)
            .HasForeignKey(customer => customer.CustomerDocumentTypeId);

        modelBuilder.Entity<Sale>()
            .HasOne(sale => sale.Customer)
            .WithMany(customer => customer.Sales)
            .HasForeignKey(sale => sale.CustomerId);

        modelBuilder.Entity<Sale>()
            .HasOne(sale => sale.SaleOrigin)
            .WithMany(origin => origin.Sales)
            .HasForeignKey(sale => sale.SaleOriginId);

        modelBuilder.Entity<Sale>()
            .HasOne(sale => sale.SaleStatus)
            .WithMany(status => status.Sales)
            .HasForeignKey(sale => sale.SaleStatusId);

        modelBuilder.Entity<SaleItem>()
            .HasOne(item => item.Sale)
            .WithMany(sale => sale.Items)
            .HasForeignKey(item => item.SaleId);

        modelBuilder.Entity<SaleItem>()
            .HasOne(item => item.Product)
            .WithMany(product => product.SaleItems)
            .HasForeignKey(item => item.ProductId);

        modelBuilder.Entity<Invoice>()
            .HasOne(invoice => invoice.Sale)
            .WithOne(sale => sale.Invoice)
            .HasForeignKey<Invoice>(invoice => invoice.SaleId);

        modelBuilder.Entity<Invoice>()
            .HasOne(invoice => invoice.InvoiceStatus)
            .WithMany(status => status.Invoices)
            .HasForeignKey(invoice => invoice.InvoiceStatusId);
    }
}
