using Microsoft.AspNetCore.Http;

namespace Northwind.Shared.Abstractions;

public sealed class ValidationFilter<T> : IEndpointFilter where T : IValidatable
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<T>().FirstOrDefault();

        if (request is not null)
        {
            var errors = request.Validate();
            if (errors.Count > 0)
                return Results.BadRequest(new { errors });
        }

        return await next(context);
    }
}
