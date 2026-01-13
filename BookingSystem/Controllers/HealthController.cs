using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Booking System API is running ");
    }
}
