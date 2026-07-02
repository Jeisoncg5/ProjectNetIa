using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNetIa.Domain.Entities;

namespace ProjectNetIa.Infrastructure.Data.Configurations;

public sealed class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Quantity)
            .IsRequired();

        entity.Property(e => e.Description)
            .HasMaxLength(300);

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasOne(e => e.ProductVariant)
            .WithMany(e => e.InventoryMovements)
            .HasForeignKey(e => e.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.InventoryMovementType)
            .WithMany(e => e.InventoryMovements)
            .HasForeignKey(e => e.InventoryMovementTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
