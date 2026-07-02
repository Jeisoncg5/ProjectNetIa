using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNetIa.Domain.Entities;

namespace ProjectNetIa.Infrastructure.Data.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.FullName)
            .IsRequired()
            .HasMaxLength(150);

        entity.Property(e => e.DocumentNumber)
            .HasMaxLength(50);

        entity.Property(e => e.Email)
            .HasMaxLength(150);

        entity.Property(e => e.Phone)
            .HasMaxLength(30);

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasOne(e => e.CustomerDocumentType)
            .WithMany(e => e.Customers)
            .HasForeignKey(e => e.CustomerDocumentTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
