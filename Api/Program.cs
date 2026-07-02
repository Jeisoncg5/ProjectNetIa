var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/", () => Results.Ok(new { message = "ProjectNetIa API running" }));

app.Run();
