using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TaskManager.API;
using TaskManager.API.Middleware;
using TaskManager.API.Services;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Infrastructure;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// CORS - Allow frontend from environment or default to localhost for development
var frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:4200";
var allowedOrigins = frontendUrl.Split(';', StringSplitOptions.RemoveEmptyEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Manager API",
        Version = "v1"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Bearer token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

// DbContext - Handle Railway's PostgreSQL URL format
var connectionString = configuration.GetConnectionString("DefaultConnection");

// Convert PostgreSQL URL format (postgresql://user:pass@host:port/db) to Npgsql format
if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
{
    try
    {
        var uri = new Uri(connectionString);
        var host = uri.Host;
        var dbPort = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/');
        var username = uri.UserInfo.Split(':')[0];
        var password = uri.UserInfo.Split(':').Length > 1 ? uri.UserInfo.Split(':')[1] : "";
        
        // Build Npgsql connection string
        connectionString = $"Host={host};Port={dbPort};Database={database};Username={username};Password={Uri.UnescapeDataString(password)}";
        
        // Update configuration
        configuration["ConnectionStrings:DefaultConnection"] = connectionString;
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Failed to parse PostgreSQL URL format, using as-is: {ConnectionString}", connectionString);
    }
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Auth configuration
builder.Services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
builder.Services.Configure<AuthSettings>(configuration.GetSection("Auth"));

// Authentication
var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization
builder.Services.AddAuthorization();

// DI registrations
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Apply schema and seed data with retry logic for Docker startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    // Retry logic for database connection (useful when database is still starting)
    var maxRetries = 10;
    var delay = TimeSpan.FromSeconds(3);
    var retryCount = 0;
    var success = false;

    while (retryCount < maxRetries && !success)
    {
        try
        {
            // For this project we use EnsureCreated to create the schema based on the model.
            // Migrations are not defined, so using Migrate() alone would leave the schema empty.
            dbContext.Database.EnsureCreated();
            await SeedData.EnsureSeedDataAsync(dbContext, configuration);
            success = true;
            logger.LogInformation("Database initialized and seeded successfully.");
        }
        catch (Exception ex) when (retryCount < maxRetries - 1)
        {
            retryCount++;
            logger.LogWarning("Database connection attempt {RetryCount}/{MaxRetries} failed. Retrying in {Delay}s... Error: {Error}", 
                retryCount, maxRetries, delay.TotalSeconds, ex.Message);
            await Task.Delay(delay);
        }
    }

    if (!success)
    {
        logger.LogError("Failed to connect to database after {MaxRetries} attempts.", maxRetries);
        throw new InvalidOperationException("Database connection failed after multiple retries.");
    }
}

// Middleware pipeline
app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker" || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure port from environment variable (Railway provides PORT)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();

public partial class Program { }

