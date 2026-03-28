using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Extensions;

namespace Northwind.Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        return services.AddApplicationLayer();
    }
}
