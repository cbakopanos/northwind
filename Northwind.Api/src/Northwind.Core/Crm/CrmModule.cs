using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Northwind.Crm.Application;
using Northwind.Crm.Domain;
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
        services.AddScoped<ICustomersService, CustomersService>();
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(Routes.Health, async (ICustomersService service, ILogger<CrmModule> logger,
                CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Crm");
                var count = await service.GetCountAsync(cancellationToken);
                return Results.Ok(new { context = "Crm", status = "ok", count });
            })
            .WithName("GetCrmHealth")
            .WithTags("Crm")
            .AddEndpointFilter<HandlingLogFilter>()
            .Produces(StatusCodes.Status200OK);

        endpoints.MapGet(
                Routes.Customers,
                async (int? page, int? pageSize, ICustomersService service, ILogger<CrmModule> logger,
                    CancellationToken cancellationToken) =>
                {
                    var currentPage = page ?? 1;
                    var currentPageSize = Math.Clamp(pageSize ?? 10, 1, 100);

                    var result = await service.GetAllAsync(currentPage, currentPageSize, cancellationToken);

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
                async (string customerId, ICustomersService service, ILogger<CrmModule> logger,
                    CancellationToken cancellationToken) =>
                {
                    CustomerDetailsDto? customer;

                    try
                    {
                        customer = await service.GetByIdAsync(customerId, cancellationToken);
                    }
                    catch (DomainValidationException ex)
                    {
                        return Results.BadRequest(new { errors = ex.Errors });
                    }

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
                async (CustomerRequest request, ICustomersService service,
                    CancellationToken cancellationToken) =>
                {
                    string createdCustomerId;

                    try
                    {
                        createdCustomerId = await service.CreateAsync(request, cancellationToken);
                    }
                    catch (DomainValidationException ex)
                    {
                        return Results.BadRequest(new { errors = ex.Errors });
                    }

                    return Results.Created(
                        $"/api/crm/customers/{createdCustomerId}",
                        new { customerId = createdCustomerId });
                })
            .WithName("CreateCustomer")
            .WithTags("Crm")
            .AddEndpointFilter<HandlingLogFilter>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        endpoints.MapPut(
                Routes.CustomerById,
                async (string customerId, CustomerRequest request, ICustomersService service,
                    ILogger<CrmModule> logger, CancellationToken cancellationToken) =>
                {
                    bool updated;

                    try
                    {
                        updated = await service.UpdateAsync(customerId, request, cancellationToken);
                    }
                    catch (DomainValidationException ex)
                    {
                        return Results.BadRequest(new { errors = ex.Errors });
                    }

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
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
        return endpoints;
    }

}