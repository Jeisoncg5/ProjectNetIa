using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNetIa.Domain.Entities;

namespace ProjectNetIa.Infrastructure.Data.Configurations;

public sealed class SaleStatusConfiguration : IEntityTypeConfiguration<SaleStatus>
{
    public void Configure(EntityTypeBuilder<SaleStatus> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);

        entity.HasIndex(e => e.Name)
            .IsUnique();

        entity.HasData(
            new { Id = 1, Name = "Pending" },
            new { Id = 2, Name = "Completed" },
            new { Id = 3, Name = "Cancelled" });
    }
}
