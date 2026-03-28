using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Northwind.Supplier.Controllers;

[ApiController]
[Route("api/supplier")]
public sealed class SupplierHealthController(ILogger<SupplierHealthController> logger) : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Supplier");
        return Ok(new { context = "Supplier", status = "ok" });
    }
}
