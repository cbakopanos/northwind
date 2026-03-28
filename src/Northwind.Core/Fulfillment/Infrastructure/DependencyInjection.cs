using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Fulfillment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFulfillmentInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Register Fulfillment infrastructure services.
        return services;
    }
}
