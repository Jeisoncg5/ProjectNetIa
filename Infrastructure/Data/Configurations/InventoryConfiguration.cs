using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNetIa.Domain.Entities;

namespace ProjectNetIa.Infrastructure.Data.Configurations;

public sealed class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Quantity)
            .IsRequired();

        entity.Property(e => e.MinimumQuantity)
            .IsRequired();

        entity.Property(e => e.UpdatedAt)
            .IsRequired();

        entity.HasOne(e => e.ProductVariant)
            .WithOne(e => e.Inventory)
            .HasForeignKey<Inventory>(e => e.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(e => e.ProductVariantId)
            .IsUnique();
    }
}
