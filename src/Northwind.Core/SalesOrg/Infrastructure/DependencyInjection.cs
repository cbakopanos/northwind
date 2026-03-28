using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Extensions;

namespace Northwind.SalesOrg.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesOrgInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddInfrastructureLayer();
    }
}
