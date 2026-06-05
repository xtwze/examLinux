using Microsoft.AspNetCore.Mvc;

namespace ExamApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    // GET api/health
    [HttpGet]
    public IActionResult Check()
    {
        return Ok(new
        {
            status    = "Healthy",
            timestamp = DateTime.UtcNow,
            version   = "1.0.0"
        });
    }
}
