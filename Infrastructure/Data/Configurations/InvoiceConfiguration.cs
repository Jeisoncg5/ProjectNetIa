using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNetIa.Domain.Entities;

namespace ProjectNetIa.Infrastructure.Data.Configurations;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(30);

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasIndex(e => e.InvoiceNumber)
            .IsUnique();

        entity.HasOne(e => e.Sale)
            .WithOne(e => e.Invoice)
            .HasForeignKey<Invoice>(e => e.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.InvoiceStatus)
            .WithMany(e => e.Invoices)
            .HasForeignKey(e => e.InvoiceStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
