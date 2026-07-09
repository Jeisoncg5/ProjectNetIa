using Microsoft.EntityFrameworkCore;
using ProjectNetIa.Domain.Entities;
using Pgvector.EntityFrameworkCore;

namespace ProjectNetIa.Infrastructure.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Color> Colors => Set<Color>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerDocumentType> CustomerDocumentTypes => Set<CustomerDocumentType>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    public DbSet<InventoryMovementType> InventoryMovementTypes => Set<InventoryMovementType>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceStatus> InvoiceStatuses => Set<InvoiceStatus>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<SaleOrigin> SaleOrigins => Set<SaleOrigin>();
    public DbSet<SaleStatus> SaleStatuses => Set<SaleStatus>();
    public DbSet<Size> Sizes => Set<Size>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
