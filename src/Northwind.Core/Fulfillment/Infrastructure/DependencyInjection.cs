using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Extensions;

namespace Northwind.Fulfillment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFulfillmentInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddInfrastructureLayer();
    }
}
