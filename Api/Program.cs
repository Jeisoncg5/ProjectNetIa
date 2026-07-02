using Microsoft.EntityFrameworkCore;
using ProjectNetIa.Application.Interfaces;
using ProjectNetIa.Infrastructure.Data;
using ProjectNetIa.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/", () => Results.Ok(new { message = "ProjectNetIa API running" }));

app.Run();
