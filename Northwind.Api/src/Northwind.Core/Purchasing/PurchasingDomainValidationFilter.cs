using Microsoft.AspNetCore.Http;
using Northwind.Purchasing.Domain;

namespace Northwind.Purchasing;

public sealed class PurchasingDomainValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            return await next(context);
        }
        catch (DomainValidationException ex)
        {
            return Results.BadRequest(new { errors = ex.Errors });
        }
    }
}
