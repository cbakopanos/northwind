using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Extensions;

namespace Northwind.SalesOrg.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesOrgControllers(this IServiceCollection services)
    {
        return services.AddControllersLayer();
    }

    public static IEndpointRouteBuilder MapSalesOrgEndpoints(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapModuleHealthEndpoint("/api/sales-org", "SalesOrg");
    }
}
