using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNetIa.Domain.Entities;

namespace ProjectNetIa.Infrastructure.Data.Configurations;

public sealed class InventoryMovementTypeConfiguration : IEntityTypeConfiguration<InventoryMovementType>
{
    public void Configure(EntityTypeBuilder<InventoryMovementType> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        entity.HasIndex(e => e.Name)
            .IsUnique();

        entity.HasData(
            new { Id = 1, Name = "InitialStock" },
            new { Id = 2, Name = "ManualAdjustment" },
            new { Id = 3, Name = "Sale" },
            new { Id = 4, Name = "Return" });
    }
}
