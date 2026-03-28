using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Extensions;

namespace Northwind.Crm.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCrmApplication(this IServiceCollection services)
    {
        return services.AddApplicationLayer();
    }
}
