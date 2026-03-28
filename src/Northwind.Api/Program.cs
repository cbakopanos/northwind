using Northwind;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNorthwindCoreModules(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Northwind API");
app.MapNorthwindCoreEndpoints();

app.Run();
