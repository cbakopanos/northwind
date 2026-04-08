using Microsoft.AspNetCore.Http;
using Northwind.Catalog.Domain;

namespace Northwind.Catalog;

public sealed class CatalogDomainValidationFilter : IEndpointFilter
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
