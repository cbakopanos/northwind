using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Northwind.Supplier.Application;
using Northwind.Shared.Extensions;

namespace Northwind.Supplier.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSupplierInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Northwind")
            ?? "Host=localhost;Port=5435;Database=northwind;Username=postgres;Password=postgres";

        services.AddDbContext<SupplierDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IGetAllSuppliers, GetAllSuppliersQuery>();

        return services.AddInfrastructureLayer();
    }
}
