using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MSABackendTemplate.Application.Interfaces;
using MSABackendTemplate.Application.Services;
using System.Reflection;

namespace MSABackendTemplate.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Bu Assembly içindeki (Application katmanı) tüm AutoMapper profillerini bul ve kaydet.
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Servisleri Scoped olarak ekliyoruz
            services.AddScoped<IProductService, ProductService>();
        }
    }
}