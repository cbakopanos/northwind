using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        endpoints.MapModuleHealthEndpoint("/api/supplier", "Supplier");

        var group = endpoints.MapGroup("/api/supplier");

        group.MapGet(
            "/suppliers",
            async (IGetAllSuppliers query, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            {
                var logger = loggerFactory.CreateLogger("Northwind.Supplier.Controllers");
                logger.LogInformation("Handling {Endpoint}", "GET /api/supplier/suppliers");

                var suppliers = await query.Execute(cancellationToken);

                logger.LogInformation("Returning {SupplierCount} suppliers", suppliers.Count);
                return Results.Ok(suppliers);
            })
            .WithName("GetAllSuppliers")
            .WithTags("Supplier")
            .Produces<IReadOnlyList<SupplierListItem>>(StatusCodes.Status200OK);

        return endpoints;
    }
}
