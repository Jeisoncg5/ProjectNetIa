using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNetIa.Domain.Entities;

namespace ProjectNetIa.Infrastructure.Data.Configurations;

public sealed class ColorConfiguration : IEntityTypeConfiguration<Color>
{
    public void Configure(EntityTypeBuilder<Color> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);

        entity.Property(e => e.HexCode)
            .HasMaxLength(7);

        entity.Property(e => e.IsActive)
            .IsRequired();

        entity.HasIndex(e => e.Name)
            .IsUnique();

        entity.HasData(
            new { Id = 1, Name = "Negro", HexCode = "#000000", IsActive = true },
            new { Id = 2, Name = "Blanco", HexCode = "#FFFFFF", IsActive = true },
            new { Id = 3, Name = "Azul", HexCode = "#0000FF", IsActive = true },
            new { Id = 4, Name = "Rojo", HexCode = "#FF0000", IsActive = true },
            new { Id = 5, Name = "Verde", HexCode = "#008000", IsActive = true },
            new { Id = 6, Name = "Beige", HexCode = "#F5F5DC", IsActive = true },
            new { Id = 7, Name = "Gris", HexCode = "#808080", IsActive = true });
    }
}
