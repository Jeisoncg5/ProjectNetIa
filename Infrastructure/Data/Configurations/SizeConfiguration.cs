using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNetIa.Domain.Entities;

namespace ProjectNetIa.Infrastructure.Data.Configurations;

public sealed class SizeConfiguration : IEntityTypeConfiguration<Size>
{
    public void Configure(EntityTypeBuilder<Size> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(30);

        entity.Property(e => e.IsActive)
            .IsRequired();

        entity.HasIndex(e => e.Name)
            .IsUnique();

        entity.HasData(
            new { Id = 1, Name = "XS", IsActive = true },
            new { Id = 2, Name = "S", IsActive = true },
            new { Id = 3, Name = "M", IsActive = true },
            new { Id = 4, Name = "L", IsActive = true },
            new { Id = 5, Name = "XL", IsActive = true },
            new { Id = 6, Name = "XXL", IsActive = true },
            new { Id = 7, Name = "36", IsActive = true },
            new { Id = 8, Name = "37", IsActive = true },
            new { Id = 9, Name = "38", IsActive = true },
            new { Id = 10, Name = "39", IsActive = true },
            new { Id = 11, Name = "40", IsActive = true },
            new { Id = 12, Name = "41", IsActive = true },
            new { Id = 13, Name = "42", IsActive = true });
    }
}
