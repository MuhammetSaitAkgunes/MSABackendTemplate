namespace MSABackendTemplate.Application.DTOs
{
    public class CreateProductDto
    {
        // Required keyword ensures compile-time safety and Swagger documentation
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}