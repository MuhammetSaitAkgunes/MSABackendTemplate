using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSABackendTemplate.Application.Interfaces.Repositories;
using MSABackendTemplate.Persistence.Contexts;
using MSABackendTemplate.Persistence.Repositories;

namespace MSABackendTemplate.Persistence;
public static class ServiceRegistration
{
    // "this IServiceCollection services" diyerek IServiceCollection'ı genişletiyoruz.
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext'i havuza ekliyoruz.
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // İleride Repository'leri de burada ekleyeceğiz.
        services.AddScoped<IProductRepository, ProductRepository>();
    }
}
