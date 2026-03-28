using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Extensions;

namespace Northwind.SalesOrg.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesOrgApplication(this IServiceCollection services)
    {
        return services.AddApplicationLayer();
    }
}
