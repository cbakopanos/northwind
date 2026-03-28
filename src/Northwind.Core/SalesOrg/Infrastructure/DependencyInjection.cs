using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.SalesOrg.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesOrgInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Register SalesOrg infrastructure services.
        return services;
    }
}
