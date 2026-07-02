using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNetIa.Domain.Entities;

namespace ProjectNetIa.Infrastructure.Data.Configurations;

public sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Sku)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(e => e.IsActive)
            .IsRequired();

        entity.HasIndex(e => e.Sku)
            .IsUnique();

        entity.HasIndex(e => new { e.ProductId, e.SizeId, e.ColorId })
            .IsUnique();

        entity.HasOne(e => e.Product)
            .WithMany(e => e.ProductVariants)
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Size)
            .WithMany(e => e.ProductVariants)
            .HasForeignKey(e => e.SizeId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Color)
            .WithMany(e => e.ProductVariants)
            .HasForeignKey(e => e.ColorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
