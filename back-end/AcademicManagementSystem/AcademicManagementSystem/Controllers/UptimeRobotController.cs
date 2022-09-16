using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

// Controller for UptimeRobot to ping the site every 5 minutes
// ========== DO NOT DELETE ========== 
[ApiController]
public class UptimeRobotController : ControllerBase
{
    [HttpGet]
    [Route("api/uptime-robot")]
    public IActionResult Get()
    {
        return Ok();
    }
}