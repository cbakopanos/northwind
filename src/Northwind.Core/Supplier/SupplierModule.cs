using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Northwind.Shared.Abstractions;
using Northwind.Supplier.Application;
using Northwind.Supplier.Infrastructure;

namespace Northwind.Supplier;

[Module(order: 60)]
public sealed class SupplierModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Northwind")
            ?? "Host=localhost;Port=5435;Database=northwind;Username=postgres;Password=postgres";

        services.AddDbContext<SupplierDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IGetAllSuppliers, GetAllSuppliersQuery>();

        return services;
    }
}
