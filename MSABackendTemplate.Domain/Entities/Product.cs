using MSABackendTemplate.Domain.Common;

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
        // Guard Clauses - Centralized validation
        Guard.AgainstNullOrEmpty(name, nameof(name), "Product name cannot be empty.");
        Guard.AgainstNegativeOrZero(price, nameof(price), "Price must be greater than zero.");
        Guard.AgainstNegative(stock, nameof(stock), "Stock cannot be negative.");

        Name = name;
        Description = description ?? string.Empty;
        Price = price;
        Stock = stock;
    }

    // Protected helper to manage UpdatedAt centrally (DRY principle)
    private void MarkAsModified()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    // Davranışsal Metotlar (Rich Domain Model)
    public void UpdatePrice(decimal newPrice)
    {
        Guard.AgainstNegativeOrZero(newPrice, nameof(newPrice), "New price must be valid.");

        Price = newPrice;
        MarkAsModified();
    }

    public void DecreaseStock(int amount)
    {
        Guard.AgainstInvalidOperation(amount > Stock, "Insufficient stock.");

        Stock -= amount;
        MarkAsModified();
    }
}
