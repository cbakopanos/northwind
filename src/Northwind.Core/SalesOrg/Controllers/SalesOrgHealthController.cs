using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Northwind.SalesOrg.Controllers;

[ApiController]
[Route("api/sales-org")]
public sealed class SalesOrgHealthController(ILogger<SalesOrgHealthController> logger) : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        logger.LogInformation("Health endpoint requested for module {ModuleContext}", "SalesOrg");
        return Ok(new { context = "SalesOrg", status = "ok" });
    }
}
