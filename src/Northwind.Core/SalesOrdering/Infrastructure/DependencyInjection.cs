using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.SalesOrdering.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesOrderingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Register SalesOrdering infrastructure services.
        return services;
    }
}
