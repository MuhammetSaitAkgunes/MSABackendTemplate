using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MSABackendTemplate.Domain.Entities;
using System.Reflection;

namespace MSABackendTemplate.Persistence.Contexts;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    // MİMAR DOKUNUŞU: DbContext yerine IdentityDbContext<ApplicationUser> yapıyoruz.
    // Bu, veritabanına AspNetUsers, AspNetRoles gibi tabloları otomatik getirecek.
    // Constructor: WebAPI'den gelen options'ı (connection string vb.) alır, base'e iletir.
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets: Tablolarımız
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Burası SİHİRLİ SATIRDIR.
        // Bu proje (Persistence) içindeki IEntityTypeConfiguration arayüzünü uygulayan 
        // TÜM sınıfları bulur ve otomatik olarak uygular.
        // Yani ProductConfiguration otomatik olarak devreye girer.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
