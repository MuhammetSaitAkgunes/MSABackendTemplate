using MSABackendTemplate.Domain.Common;// BaseEntity'i görmek için

namespace MSABackendTemplate.Domain.Entities;

// Sealed: Bu sınıftan başka sınıf türetilemez. Performans optimizasyonu sağlar.
public sealed class Product : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    // Entity Framework (EF) Core, constructor'sız nesne oluşturamaz. 
    // O yüzden boş bir constructor bırakırız ama private yaparız ki başkası kullanmasın.
    private Product() { }

    // Bizim nesne oluşturma kuralımız budur.
    // Validation (Doğrulama) burada başlar. İsimsiz ürün olamaz!
    public Product(string name, string description, decimal price, int stock)
    {
        // Guard Clause (Koruyucu Şartlar)
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Product name cannot be empty.");

        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
    }

    // Davranışsal Metotlar (Rich Domain Model)
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0) throw new ArgumentException("New price must be valid.");

        Price = newPrice;
        UpdatedAt = DateTime.UtcNow; // Güncelleme tarihini otomatik yönettik
    }

    public void DecreaseStock(int amount)
    {
        if (amount > Stock) throw new InvalidOperationException("Insufficient stock.");

        Stock -= amount;
        UpdatedAt = DateTime.UtcNow;
    }
}
