using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNetIa.Domain.Entities;

namespace ProjectNetIa.Infrastructure.Data.Configurations;

public sealed class SaleOriginConfiguration : IEntityTypeConfiguration<SaleOrigin>
{
    public void Configure(EntityTypeBuilder<SaleOrigin> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);

        entity.HasIndex(e => e.Name)
            .IsUnique();

        entity.HasData(
            new { Id = 1, Name = "Manual" },
            new { Id = 2, Name = "Chatbot" });
    }
}
