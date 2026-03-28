using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Northwind.Crm.Controllers;

[ApiController]
[Route("api/crm")]
public sealed class CrmHealthController(ILogger<CrmHealthController> logger) : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Crm");
        return Ok(new { context = "Crm", status = "ok" });
    }
}
