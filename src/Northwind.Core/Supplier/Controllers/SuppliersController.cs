using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Northwind.Supplier.Application;

namespace Northwind.Supplier.Controllers;

[ApiController]
[Route("api/supplier")]
public sealed class SuppliersController(
    IGetAllSuppliers query,
    ILogger<SuppliersController> logger) : ControllerBase
{
    [HttpGet("suppliers")]
    [ProducesResponseType<IReadOnlyList<SupplierListItem>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SupplierListItem>>> GetAll(CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {Endpoint}", "GET /api/supplier/suppliers");

        var suppliers = await query.Execute(cancellationToken);

        logger.LogInformation("Returning {SupplierCount} suppliers", suppliers.Count);
        return Ok(suppliers);
    }
}
