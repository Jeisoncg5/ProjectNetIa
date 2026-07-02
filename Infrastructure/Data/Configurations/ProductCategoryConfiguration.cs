using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNetIa.Domain.Entities;

namespace ProjectNetIa.Infrastructure.Data.Configurations;

public sealed class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(e => e.Description)
            .HasMaxLength(300);

        entity.HasIndex(e => e.Name)
            .IsUnique();

        entity.HasData(
            new { Id = 1, Name = "Camisetas", Description = "Camisetas y tops casuales", IsActive = true },
            new { Id = 2, Name = "Camisas", Description = "Camisas formales e informales", IsActive = true },
            new { Id = 3, Name = "Pantalones", Description = "Pantalones de uso diario", IsActive = true },
            new { Id = 4, Name = "Jeans", Description = "Jeans y prendas denim", IsActive = true },
            new { Id = 5, Name = "Chaquetas", Description = "Chaquetas y prendas exteriores", IsActive = true },
            new { Id = 6, Name = "Vestidos", Description = "Vestidos y enterizos", IsActive = true },
            new { Id = 7, Name = "Zapatos", Description = "Calzado", IsActive = true },
            new { Id = 8, Name = "Accesorios", Description = "Complementos y accesorios", IsActive = true });
    }
}
