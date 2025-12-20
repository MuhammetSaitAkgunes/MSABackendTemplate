using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using MSABackendTemplate.Application;
using MSABackendTemplate.Persistence;
using MSABackendTemplate.WebAPI.Filters;
using MSABackendTemplate.WebAPI.Middlewares; // [ÖNEMLÝ] Middleware namespace'ini eklemeyi unutma
using Serilog;

// 1. Serilog Kurulumu
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Sistem baþlatýlýyor...");

    var builder = WebApplication.CreateBuilder(args);

    // [EKLENDÝ] Serilog'u Host'a baðlýyoruz. Bu olmazsa loglar düzgün çalýþmaz.
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));

    // --- MÝMARÝ ENJEKSÝYON ALANI ---
    builder.Services.AddPersistenceServices(builder.Configuration);
    builder.Services.AddApplicationServices();

    // Controller ve Filtre Ayarlarý
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>(); // Her isteði denetle!
    });
    // [SÝLÝNDÝ] Aþaðýdaki ikinci 'AddControllers' satýrý gereksizdi, sildik.

    // FluentValidation Ayarlarý
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();

    // Default 400 Cevabýný Kapatma
    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // --- PIPELINE (BORU HATTI) DÜZENLEMESÝ ---

    // [KRÝTÝK EKLEME] Hata Yakalayýcý Middleware EN TEPEDE OLMALI.
    // Senin kodunda bu satýr yoktu, o yüzden hatalarý yakalayamýyordun.
    app.UseMiddleware<ErrorHandlerMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // [EKLENDÝ] Serilog HTTP Ýstek Loglamasý
    // Hangi endpoint'e istek geldi, kaç ms sürdü? Bunu görmek için gerekli.
    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Uygulama beklenmedik bir þekilde sonlandý!");
}
finally
{
    Log.CloseAndFlush();
}