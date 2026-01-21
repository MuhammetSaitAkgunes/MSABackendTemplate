using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MSABackendTemplate.Application;
using MSABackendTemplate.Persistence;
using MSABackendTemplate.Persistence.Contexts;
using MSABackendTemplate.WebAPI.Filters;
using MSABackendTemplate.WebAPI.Middlewares;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

// 1. Serilog Kurulumu
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Sistem başlatılıyor...");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog'u Host'a bağlıyoruz.
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));

    // --- MİMARİ ENJEKSİYON ALANI ---
    builder.Services.AddPersistenceServices(builder.Configuration);
    builder.Services.AddApplicationServices();

    // --- HEALTH CHECKS CONFIGURATION ---
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddHealthChecks()
        .AddSqlServer(
            connectionString!,
            name: "database",
            failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
            tags: new[] { "db", "sql", "ready" });

    // --- RATE LIMITING CONFIGURATION ---
    builder.Services.AddRateLimiter(options =>
    {
        // Fixed Window: 100 requests per minute per IP
        options.AddFixedWindowLimiter("fixed", opt =>
        {
            opt.Window = TimeSpan.FromMinutes(1);
            opt.PermitLimit = 100;
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = 2;
        });

        // Sliding Window: 50 requests per 30 seconds per IP (more strict)
        options.AddSlidingWindowLimiter("sliding", opt =>
        {
            opt.Window = TimeSpan.FromSeconds(30);
            opt.PermitLimit = 50;
            opt.SegmentsPerWindow = 3;
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = 2;
        });

        // Token Bucket: For burst traffic (authentication endpoints)
        options.AddTokenBucketLimiter("auth", opt =>
        {
            opt.TokenLimit = 10;
            opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
            opt.TokensPerPeriod = 5;
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = 1;
        });

        // Global fallback
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.User.Identity?.Name
                    ?? httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown",
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 1000,
                    Window = TimeSpan.FromMinutes(1)
                }));

        // Custom response when rate limit exceeded
        options.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    error = "Too many requests. Please try again later.",
                    retryAfter = retryAfter.TotalSeconds
                }, cancellationToken: token);
            }
            else
            {
                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    error = "Too many requests. Please try again later."
                }, cancellationToken: token);
            }
        };
    });

    // --- JWT AUTHENTICATION CONFIGURATION ---
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["Key"];
    var issuer = jwtSettings["Issuer"];
    var audience = jwtSettings["Audience"];
    if (string.IsNullOrWhiteSpace(secretKey) ||
        string.IsNullOrWhiteSpace(issuer) ||
        string.IsNullOrWhiteSpace(audience))
    {
        throw new InvalidOperationException("JwtSettings configuration is missing required values.");
    }

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
            ClockSkew = TimeSpan.Zero
        };
    });

    // Controller ve Filtre Ayarları
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    });

    // FluentValidation Ayarları
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();

    // Default 400 Cevabını Kapatma
    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

    builder.Services.AddEndpointsApiExplorer();

    // Swagger JWT Integration
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "MSA Backend Template API",
            Version = "v1",
            Description = "JWT Authentication enabled API"
        });

        // JWT Bearer Token için Security Definition
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme."
        });

        // Tüm endpoint'lere JWT gereksinimi ekle
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    var app = builder.Build();

    app.UseMiddleware<ErrorHandlerMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    // RATE LIMITER: Must be after UseAuthentication/UseAuthorization
    app.UseRateLimiter();

    // --- HEALTH CHECK ENDPOINTS ---
    // Basic health check
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    // Readiness probe (for Kubernetes)
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    // Liveness probe (for Kubernetes)
    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = _ => false // No checks, just returns 200 if app is running
    });

    app.MapControllers();

    app.Run();
}
catch (Microsoft.Extensions.Hosting.HostAbortedException)
{
    // EF Core migrations and design-time tools abort the host - this is expected behavior
    // Don't log this as an error to avoid confusion
}
catch (Exception e)
{
    Log.Fatal(e, "Uygulama beklenmedik bir şekilde sonlandı!");
    Log.Error("Hata Detayları: {Message}", e.Message);
    throw;
}
finally
{
    Log.CloseAndFlush();
}
