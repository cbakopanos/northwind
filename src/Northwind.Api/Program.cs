using Northwind;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddNorthwindCoreModules(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Northwind API");
app.MapGet("/api/health", () => new { service = "Northwind.Api", status = "ok" });

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.MapNorthwindCoreEndpoints();

app.Run();
