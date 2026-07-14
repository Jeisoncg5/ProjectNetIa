using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNetIa.Domain.Entities;
using ProjectNetIa.Infrastructure.Search;

namespace ProjectNetIa.Infrastructure.Data.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(150);

        entity.Property(e => e.Description)
            .HasMaxLength(500);

        entity.Property(e => e.ImageUrl)
            .HasMaxLength(500);

        entity.Property(e => e.Price)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        entity.Property(e => e.IsActive)
            .IsRequired();

        entity.Property(e => e.Embedding)
            .HasColumnType($"vector({ProductEmbeddingGenerator.Dimension})");

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasIndex(e => e.Embedding)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");

        entity.HasOne(e => e.ProductCategory)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.ProductCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
