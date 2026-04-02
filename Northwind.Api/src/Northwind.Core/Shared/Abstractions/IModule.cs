using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Shared.Abstractions;

public interface IModule
{
    void RegisterServices(IServiceCollection services, string connectionString);
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}