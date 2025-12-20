using AutoMapper;
using MSABackendTemplate.Domain.Entities;
using MSABackendTemplate.Application.DTOs;
using MSABackendTemplate.Domain.Entities;

namespace NeuroArchitect.Application.Mappings
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            // Entity -> DTO (Veritabanından okurken)
            CreateMap<Product, ProductDto>()
                .ReverseMap(); // DTO -> Entity dönüşümü de gerekirse yap.

            // CreateDto -> Entity (Veritabanına yazarken)
            CreateMap<CreateProductDto, Product>();
        }
    }
}