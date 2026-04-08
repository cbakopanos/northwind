using Microsoft.AspNetCore.Http;
using Northwind.Crm.Domain;

namespace Northwind.Crm;

public sealed class CrmDomainValidationFilter : IEndpointFilter
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
