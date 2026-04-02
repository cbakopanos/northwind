using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Northwind.Crm.Application;
using Northwind.Crm.Infrastructure;
using Northwind.Shared.Abstractions;

namespace Northwind.Crm;

file static class Routes
{
    public const string Health      = "/api/crm/health";
    public const string Customers   = "/api/crm/customers";
    public const string CustomerById = "/api/crm/customers/{customerId}";
}

public sealed class CrmModule : IModule
{
    public void RegisterServices(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<CrmDbContext>(o => o.UseNpgsql(connectionString));
        services.AddScoped<ICustomersRepository, CustomersRepository>();
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(Routes.Health, async (ICustomersRepository repository, ILogger<CrmModule> logger,
                CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Crm");
                var count = await repository.GetCountAsync(cancellationToken);
                return Results.Ok(new { context = "Crm", status = "ok", count });
            })
            .WithName("GetCrmHealth")
            .WithTags("Crm")
            .AddEndpointFilter<HandlingLogFilter>()
            .Produces(StatusCodes.Status200OK);

        endpoints.MapGet(
                Routes.Customers,
                async (int? page, int? pageSize, ICustomersRepository query, ILogger<CrmModule> logger,
                    CancellationToken cancellationToken) =>
                {
                    var currentPage = page ?? 1;
                    var currentPageSize = Math.Clamp(pageSize ?? 10, 1, 100);

                    var result = await query.GetAllAsync(currentPage, currentPageSize, cancellationToken);

                    logger.LogInformation("Returning {CustomerCount} of {TotalCount} customers", result.Items.Count,
                        result.TotalCount);
                    return Results.Ok(result);
                })
            .WithName("GetAllCustomers")
            .WithTags("Crm")
            .AddEndpointFilter<HandlingLogFilter>()
            .Produces<PagedResult<CustomerSummaryDto>>();

        endpoints.MapGet(
                Routes.CustomerById,
                async (string customerId, ICustomersRepository query, ILogger<CrmModule> logger,
                    CancellationToken cancellationToken) =>
                {
                    var customer = await query.GetByIdAsync(customerId, cancellationToken);

                    if (customer is null)
                    {
                        logger.LogWarning("Customer {CustomerId} not found", customerId);
                        return Results.NotFound();
                    }

                    return Results.Ok(customer);
                })
            .WithName("GetCustomerById")
            .WithTags("Crm")
            .AddEndpointFilter<HandlingLogFilter>()
            .Produces<CustomerDetailsDto>()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(
                Routes.Customers,
                async (CustomerRequest request, ICustomersRepository repository,
                    CancellationToken cancellationToken) =>
                {
                    var createdCustomerId = await repository.CreateAsync(request, cancellationToken);

                    return Results.Created(
                        $"/api/crm/customers/{createdCustomerId}",
                        new { customerId = createdCustomerId });
                })
            .WithName("CreateCustomer")
            .WithTags("Crm")
            .AddEndpointFilter<HandlingLogFilter>()
            .AddEndpointFilter<ValidationFilter<CustomerRequest>>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        endpoints.MapPut(
                Routes.CustomerById,
                async (string customerId, CustomerRequest request, ICustomersRepository repository,
                    ILogger<CrmModule> logger, CancellationToken cancellationToken) =>
                {
                    var updated = await repository.UpdateAsync(customerId, request, cancellationToken);

                    if (!updated)
                    {
                        logger.LogWarning("Customer {CustomerId} not found for update", customerId);
                        return Results.NotFound();
                    }

                    return Results.NoContent();
                })
            .WithName("UpdateCustomer")
            .WithTags("Crm")
            .AddEndpointFilter<HandlingLogFilter>()
            .AddEndpointFilter<ValidationFilter<CustomerRequest>>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
        return endpoints;
    }

}