namespace MSABackendTemplate.Application.DTOs
{
    public class CreateProductDto
    {
        // Burada basit DataAnnotations kullanabiliriz.
        // Ama daha profesyonel bir doğrulama için ileride "FluentValidation" kullanacağız.

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}