using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSABackendTemplate.Application.Interfaces;
using MSABackendTemplate.Application.Interfaces.Repositories;
using MSABackendTemplate.Domain.Entities;
using MSABackendTemplate.Persistence.Contexts;
using MSABackendTemplate.Persistence.Repositories;
using MSABackendTemplate.Persistence.Services;

namespace MSABackendTemplate.Persistence;
public static class ServiceRegistration
{
    // "this IServiceCollection services" diyerek IServiceCollection'ı genişletiyoruz.
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext'i havuza ekliyoruz.
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // ASP.NET Core Identity servislerini ekliyoruz
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Şifre gereksinimleri (İsteğe bağlı olarak özelleştirilebilir)
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 6;

            // Email benzersiz olmalı
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders(); // Token providers (email confirmation, password reset için)

        // Repository'leri ekliyoruz
        services.AddScoped<IProductRepository, ProductRepository>();

        // Application servislerini ekliyoruz
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // --- CACHE SERVICE            // Cache Service (Singleton - shared across requests)
        services.AddMemoryCache(); // In-Memory cache (built-in .NET)
        services.AddSingleton<ICacheService, MemoryCacheService>();

        // Payment Service (Mock for demonstration)
        services.AddScoped<IPaymentService, MockPaymentService>();

        // Unit of Work (Scoped - one per request)
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
