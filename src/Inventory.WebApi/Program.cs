using Inventory.Application.Interfaces;
using Inventory.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SqlConnectionString") 
    ?? throw new InvalidOperationException("Connection string 'SqlConnectionString' not found.");

builder.Services.AddScoped<IProductRepository>(provider => new ProductRepository(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers(); // Mapea los controladores que crearemos a continuación

app.Run();