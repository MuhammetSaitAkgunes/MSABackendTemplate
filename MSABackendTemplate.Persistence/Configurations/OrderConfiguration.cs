using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MSABackendTemplate.Domain.Entities;

namespace MSABackendTemplate.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Table name
        builder.ToTable("Orders");

        // Primary Key
        builder.HasKey(o => o.Id);

        // Property configurations
        builder.Property(o => o.ProductId)
            .IsRequired();

        builder.Property(o => o.Quantity)
            .IsRequired();

        // Decimal precision for financial data (18 total digits, 2 after decimal point)
        builder.Property(o => o.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<int>();  // Store enum as integer

        builder.Property(o => o.PaymentTransactionId)
            .HasMaxLength(200);

        // Index on ProductId for better query performance
        builder.HasIndex(o => o.ProductId);

        // Index on Status for filtering orders by status
        builder.HasIndex(o => o.Status);
    }
}
