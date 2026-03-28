using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Extensions;

namespace Northwind.Supplier.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSupplierApplication(this IServiceCollection services)
    {
        return services.AddApplicationLayer();
    }
}
