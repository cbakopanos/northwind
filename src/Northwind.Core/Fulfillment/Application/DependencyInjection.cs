using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Extensions;

namespace Northwind.Fulfillment.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddFulfillmentApplication(this IServiceCollection services)
    {
        return services.AddApplicationLayer();
    }
}
