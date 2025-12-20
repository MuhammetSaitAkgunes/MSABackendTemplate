using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MSABackendTemplate.Domain.Entities;

namespace MSABackendTemplate.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Tablo adı (Opsiyonel ama iyi bir pratik)
        builder.ToTable("Products");

        // Primary Key
        builder.HasKey(p => p.Id);

        // Property Ayarları
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100); // Veritabanında NVARCHAR(100) olacak

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(500);

        // Decimal hassasiyeti (Finansal verilerde kritik!)
        // Toplam 18 basamak, virgülden sonra 2 basamak.
        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        // CreatedDate gibi alanlar veritabanı seviyesinde de default değer alabilir
        // Ama biz Domain'de hallettik (new DateTime...), burası opsiyonel.
    }
}
