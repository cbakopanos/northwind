using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Shared.Abstractions;

public interface IModule
{
    IServiceCollection AddModule(IServiceCollection services, IConfiguration configuration);

    WebApplication MapEndpoints(WebApplication app);
}
