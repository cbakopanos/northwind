using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Northwind.SalesOrdering.Controllers;

[ApiController]
[Route("api/sales-ordering")]
public sealed class SalesOrderingHealthController(ILogger<SalesOrderingHealthController> logger) : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        logger.LogInformation("Health endpoint requested for module {ModuleContext}", "SalesOrdering");
        return Ok(new { context = "SalesOrdering", status = "ok" });
    }
}
