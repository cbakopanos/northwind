using Northwind;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
	options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
	options.SingleLine = true;
	options.IncludeScopes = false;
});

builder.Services.AddOpenApi();
builder.Services.AddNorthwindCoreModules(builder.Configuration);

var app = builder.Build();

app.Logger.LogInformation("Starting Northwind API on environment {EnvironmentName}", app.Environment.EnvironmentName);

app.Use(async (context, next) =>
{
	var startedAt = Stopwatch.GetTimestamp();

	await next();

	var elapsed = Stopwatch.GetElapsedTime(startedAt);
	app.Logger.LogInformation(
		"HTTP {Method} {Path} -> {StatusCode} in {ElapsedMs} ms",
		context.Request.Method,
		context.Request.Path,
		context.Response.StatusCode,
		elapsed.TotalMilliseconds);
});

app.MapGet("/", () => "Northwind API");
app.MapGet("/api/health", () => new { service = "Northwind.Api", status = "ok" });

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.MapNorthwindCoreModuleEndpoints();

app.Run();
