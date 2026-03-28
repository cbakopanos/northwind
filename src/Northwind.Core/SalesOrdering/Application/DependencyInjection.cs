using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Extensions;

namespace Northwind.SalesOrdering.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesOrderingApplication(this IServiceCollection services)
    {
        return services.AddApplicationLayer();
    }
}
