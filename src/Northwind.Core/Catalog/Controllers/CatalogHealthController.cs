using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Northwind.Catalog.Controllers;

[ApiController]
[Route("api/catalog")]
public sealed class CatalogHealthController(ILogger<CatalogHealthController> logger) : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Catalog");
        return Ok(new { context = "Catalog", status = "ok" });
    }
}
