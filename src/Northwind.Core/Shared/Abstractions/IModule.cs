using Microsoft.AspNetCore.Routing;

namespace Northwind.Shared.Abstractions;

public interface IModule
{
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}
