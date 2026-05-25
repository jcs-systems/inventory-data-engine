using Inventory.Application.Interfaces;
using Inventory.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SqlConnectionString") 
    ?? throw new InvalidOperationException("Connection string 'SqlConnectionString' not found.");

builder.Services.AddScoped<IProductRepository>(provider => new ProductRepository(connectionString));
builder.Services.AddScoped<IInventoryTransactionRepository, Inventory.Infrastructure.Repositories.InventoryTransactionRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddControllers();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


//app.UseHttpsRedirection();
app.UseAuthorization();

// Endpoints
app.MapControllers();
app.MapGet("/api/test", () => Results.Ok("El servidor sí está mapeando rutas dinámicas"));

app.Run();
