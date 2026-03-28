using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared.Abstractions;
using Northwind.Supplier.Application;
using Northwind.Supplier.Infrastructure;

namespace Northwind.Supplier;

[Module]
[Infrastructure(typeof(SupplierDbContext))]
public sealed class SupplierModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IGetAllSuppliers, GetAllSuppliersQuery>();

        return services;
    }
}
