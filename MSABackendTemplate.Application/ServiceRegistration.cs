using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MSABackendTemplate.Application.Interfaces;
using MSABackendTemplate.Application.Services;
using System.Reflection;

namespace MSABackendTemplate.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Bu Assembly içindeki (Application katmanı) tüm AutoMapper profillerini bul ve kaydet.
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // --- APPLICATION SERVICES ---
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}