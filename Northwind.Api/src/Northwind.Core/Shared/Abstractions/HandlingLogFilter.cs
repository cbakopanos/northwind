using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Northwind.Shared.Abstractions;

public sealed class HandlingLogFilter : IEndpointFilter
{
    private readonly ILogger<HandlingLogFilter> _logger;

    public HandlingLogFilter(ILogger<HandlingLogFilter> logger) => _logger = logger;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        _logger.LogInformation("Handling {Method} {Endpoint}",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path);

        return await next(context);
    }
}
