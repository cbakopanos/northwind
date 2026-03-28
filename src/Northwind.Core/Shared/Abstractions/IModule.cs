using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Shared.Abstractions;

public interface IModule
{
    IServiceCollection AddModule(IServiceCollection services);
}
