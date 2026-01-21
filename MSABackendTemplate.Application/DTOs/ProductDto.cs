namespace MSABackendTemplate.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    // Stock info intentionally excluded - DTOs control what clients see
}