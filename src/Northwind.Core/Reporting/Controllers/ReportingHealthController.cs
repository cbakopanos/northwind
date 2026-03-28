using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Northwind.Reporting.Controllers;

[ApiController]
[Route("api/reporting")]
public sealed class ReportingHealthController(ILogger<ReportingHealthController> logger) : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Reporting");
        return Ok(new { context = "Reporting", status = "ok" });
    }
}
