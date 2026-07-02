using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNetIa.Domain.Entities;

namespace ProjectNetIa.Infrastructure.Data.Configurations;

public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasOne(e => e.Customer)
            .WithMany(e => e.Sales)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.SaleOrigin)
            .WithMany(e => e.Sales)
            .HasForeignKey(e => e.SaleOriginId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.SaleStatus)
            .WithMany(e => e.Sales)
            .HasForeignKey(e => e.SaleStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
