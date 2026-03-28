using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Extensions;

namespace Northwind.Fulfillment.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddFulfillmentControllers(this IServiceCollection services)
    {
        return services.AddControllersLayer();
    }

    public static IEndpointRouteBuilder MapFulfillmentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapModuleHealthEndpoint("/api/fulfillment", "Fulfillment");
    }
}
