using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Northwind.Fulfillment.Controllers;

[ApiController]
[Route("api/fulfillment")]
public sealed class FulfillmentHealthController(ILogger<FulfillmentHealthController> logger) : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        logger.LogInformation("Health endpoint requested for module {ModuleContext}", "Fulfillment");
        return Ok(new { context = "Fulfillment", status = "ok" });
    }
}
