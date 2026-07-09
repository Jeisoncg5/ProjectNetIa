using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectNetIa.Api.Configuration;
using ProjectNetIa.Application.Interfaces;
using ProjectNetIa.Infrastructure.Data;
using ProjectNetIa.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.Configure<ChatbotOptions>(
    builder.Configuration.GetSection(ChatbotOptions.SectionName));
builder.Services.AddHttpClient<IChatService, ChatService>((serviceProvider, client) =>
{
    var options = serviceProvider
        .GetRequiredService<IOptions<ChatbotOptions>>()
        .Value;

    if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri))
    {
        throw new InvalidOperationException("La URL base del chatbot no es valida.");
    }

    client.BaseAddress = baseUri;
    client.Timeout = TimeSpan.FromSeconds(Math.Max(1, options.TimeoutSeconds));
});

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "http://localhost:3000",
                "http://127.0.0.1:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await StoreCatalogSeeder.SeedAsync(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ReactPolicy");
app.MapControllers();
app.MapGet("/", () => Results.Ok(new { message = "ProjectNetIa API running" }));

app.Run();
