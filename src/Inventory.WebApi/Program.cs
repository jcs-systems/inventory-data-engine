using Inventory.Application.Interfaces;
using Inventory.Application.Models;
using Inventory.Infrastructure.Repositories;
using Inventory.Infrastructure.Services;
using Inventory.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SqlConnectionString") 
    ?? throw new InvalidOperationException("Connection string 'SqlConnectionString' not found.");

// Repositorios
builder.Services.AddScoped<IProductRepository>(provider => new ProductRepository(connectionString));
builder.Services.AddScoped<IStockRepository>(provider => new StockRepository(connectionString));
builder.Services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();

// Configuración SMTP y Servicio de Correo
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

// Endpoints
app.MapControllers();
app.MapGet("/api/test", () => Results.Ok("El servidor sí está mapeando rutas dinámicas"));

app.Run();