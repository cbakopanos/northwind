using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Supplier.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSupplierInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Register Supplier infrastructure services.
        return services;
    }
}
