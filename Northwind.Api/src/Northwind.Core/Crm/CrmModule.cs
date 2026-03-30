using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Northwind.Shared.Abstractions;
using Northwind.Crm.Application;
using Northwind.Crm.Infrastructure;

namespace Northwind.Crm;

[Module]
[Infrastructure(typeof(CrmDbContext))]
[Repository(typeof(ICustomersRepository), typeof(CustomersRepository))]
public sealed class CrmModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/crm/health", async (ICustomersRepository repository, ILogger<CrmModule> logger,
                CancellationToken cancellationToken) =>
            {
                logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Crm");
                var count = await repository.GetCountAsync(cancellationToken);
                return Results.Ok(new { context = "Crm", status = "ok", count });
            });

        endpoints.MapGet(
                "/api/crm/customers",
                async (int? page, int? pageSize, ICustomersRepository query, ILogger<CrmModule> logger,
                    CancellationToken cancellationToken) =>
                {
                    var currentPage = page ?? 1;
                    var currentPageSize = Math.Clamp(pageSize ?? 10, 1, 100);
                    logger.LogInformation("Handling {Endpoint} (page {Page}, pageSize {PageSize})",
                        "GET /api/crm/customers", currentPage, currentPageSize);

                    var result = await query.GetAllAsync(currentPage, currentPageSize, cancellationToken);

                    logger.LogInformation("Returning {CustomerCount} of {TotalCount} customers", result.Items.Count,
                        result.TotalCount);
                    return Results.Ok(result);
                })
            .WithName("GetAllCustomers")
            .WithTags("Crm")
            .Produces<PagedResult<CustomerSummaryDto>>();

        endpoints.MapGet(
                "/api/crm/customers/{customerId}",
                async (string customerId, ICustomersRepository query, ILogger<CrmModule> logger,
                    CancellationToken cancellationToken) =>
                {
                    logger.LogInformation("Handling {Endpoint} for CustomerId {CustomerId}",
                        "GET /api/crm/customers/{customerId}", customerId);

                    var customer = await query.GetByIdAsync(customerId, cancellationToken);

                    if (customer is null)
                    {
                        logger.LogInformation("Customer {CustomerId} not found", customerId);
                        return Results.NotFound();
                    }

                    logger.LogInformation("Returning customer {CustomerId}", customerId);
                    return Results.Ok(customer);
                })
            .WithName("GetCustomerById")
            .WithTags("Crm")
            .Produces<CustomerDetailsDto>()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(
                "/api/crm/customers",
                async (CustomerRequest request, ICustomersRepository repository, ILogger<CrmModule> logger,
                    CancellationToken cancellationToken) =>
                {
                    logger.LogInformation("Handling {Endpoint}", "POST /api/crm/customers");

                    if (string.IsNullOrWhiteSpace(request.CompanyName))
                    {
                        logger.LogInformation("Create customer request rejected: CompanyName is required");
                        return Results.BadRequest(new { error = "CompanyName is required." });
                    }

                    var createdCustomerId = await repository.CreateAsync(request, cancellationToken);

                    logger.LogInformation("Created customer {CustomerId}", createdCustomerId);
                    return Results.Created($"/api/crm/customers/{createdCustomerId}", new { customerId = createdCustomerId });
                })
            .WithName("CreateCustomer")
            .WithTags("Crm")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        endpoints.MapPut(
                "/api/crm/customers/{customerId}",
                async (string customerId, CustomerRequest request, ICustomersRepository repository,
                    ILogger<CrmModule> logger, CancellationToken cancellationToken) =>
                {
                    logger.LogInformation("Handling {Endpoint} for CustomerId {CustomerId}",
                        "PUT /api/crm/customers/{customerId}", customerId);

                    if (string.IsNullOrWhiteSpace(request.CompanyName))
                    {
                        logger.LogInformation(
                            "Update customer request rejected for CustomerId {CustomerId}: CompanyName is required",
                            customerId);
                        return Results.BadRequest(new { error = "CompanyName is required." });
                    }

                    var updated = await repository.UpdateAsync(customerId, request, cancellationToken);

                    if (!updated)
                    {
                        logger.LogInformation("Customer {CustomerId} not found for update", customerId);
                        return Results.NotFound();
                    }

                    logger.LogInformation("Updated customer {CustomerId}", customerId);
                    return Results.NoContent();
                })
            .WithName("UpdateCustomer")
            .WithTags("Crm")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
        return endpoints;
    }
}