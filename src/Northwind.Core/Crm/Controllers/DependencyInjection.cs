using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Extensions;

namespace Northwind.Crm.Controllers;

public static class DependencyInjection
{
    public static IServiceCollection AddCrmControllers(this IServiceCollection services)
    {
        return services.AddControllersLayer();
    }

    public static IEndpointRouteBuilder MapCrmEndpoints(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapModuleHealthEndpoint("/api/crm", "Crm");
    }
}
