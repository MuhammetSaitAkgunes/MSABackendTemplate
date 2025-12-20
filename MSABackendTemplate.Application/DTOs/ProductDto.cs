using System;

namespace MSABackendTemplate.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        // Stock bilgisini buraya koymadım mesela. Belki müşteri stok miktarını görmemeli.
        // İşte DTO'nun gücü budur.
    }
}