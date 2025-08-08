using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Polly;
using Polly.Extensions.Http;
using ThAmCo.ProductsAPI.Data;
using ThAmCo.ProductsAPI.ProductsRepo;
using ThAmCo.ProductsAPI.UnderCutters;

var builder = WebApplication.CreateBuilder(args);

// Load config files

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);


// Polly resilience policies
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(5, retryAttempt)));
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// auth0 authentication is in configuration
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.Audience = builder.Configuration["Auth:Audience"];
    });

builder.Services.AddAuthorization();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IUnderCuttersService, UnderCuttersServiceFake>();
    builder.Services.AddSingleton<IProductsRepo, ProductRepoFake>();
}
// Configure the database based on environment
builder.Services.AddDbContext<ProductsContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        var folder = Environment.SpecialFolder.MyDocuments;
        var path = Environment.GetFolderPath(folder);
        var dbPath = Path.Join(path, "products.db");

        options.UseSqlite($"Data Source={dbPath}");
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
    else
    {
        var cs = builder.Configuration.GetConnectionString("ProductsContext");

        options.UseSqlServer(cs, sqlOptions =>
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(5),errorNumbersToAdd: null));
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
        
    }
});
// Configure based on environment
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IUnderCuttersService, UnderCuttersServiceFake>();
    builder.Services.AddSingleton<IProductsRepo, ProductRepoFake>();
}
else
{
    builder.Services.AddHttpClient<IUnderCuttersService, UnderCuttersService>();
    builder.Services.AddHttpClient<IProductsRepo, ProductsRepo>()
                    .AddPolicyHandler(GetRetryPolicy());
}

builder.Services.AddTransient<IProductsRepo, ProductsRepo>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var env = services.GetRequiredService<IWebHostEnvironment>();
    if (env.IsDevelopment())
    {
        var context = services.GetRequiredService<ProductsContext>();
        try
        {
            ProductsInitialiser.SeedTestData(context).Wait();
        }
        catch (Exception)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogDebug("Seeding test data failed.");
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

ApplyMigration();

app.Run();

// Method to automatically apply migrations
void ApplyMigration()
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ProductsContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}
