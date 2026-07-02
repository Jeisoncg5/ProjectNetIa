using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNetIa.Domain.Entities;

namespace ProjectNetIa.Infrastructure.Data.Configurations;

public sealed class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Quantity)
            .IsRequired();

        entity.Property(e => e.UnitPrice)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        entity.HasOne(e => e.Sale)
            .WithMany(e => e.Items)
            .HasForeignKey(e => e.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.ProductVariant)
            .WithMany(e => e.SaleItems)
            .HasForeignKey(e => e.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
