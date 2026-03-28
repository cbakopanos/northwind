using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Supplier.Application;
using Northwind.Shared.Extensions;

namespace Northwind.Supplier.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddSupplierControllers(this IServiceCollection services)
    {
        return services.AddControllersLayer();
    }

    public static IEndpointRouteBuilder MapSupplierEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/supplier");

        group.MapGet("/health", () => new { context = "Supplier", status = "ok" });

        group.MapGet(
            "/suppliers",
            async (IGetAllSuppliers query, CancellationToken cancellationToken) =>
            {
                var suppliers = await query.Execute(cancellationToken);
                return Results.Ok(suppliers);
            })
            .WithName("GetAllSuppliers")
            .WithTags("Supplier")
            .Produces<IReadOnlyList<SupplierListItem>>(StatusCodes.Status200OK);

        return endpoints;
    }
}
