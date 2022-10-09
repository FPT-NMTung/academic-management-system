using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.ClassStatusController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class ClassStatusController : ControllerBase
{
    private readonly AmsContext _context;

    public ClassStatusController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/class-statuses")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetClassStatuses()
    {
        var classStatuses =
            _context.ClassStatuses.Select(cs => new ClassStatusResponse()
            {
                Id = cs.Id,
                Value = cs.Value
            });
        return Ok(CustomResponse.Ok("get all Class statuses successfully", classStatuses));
    }
}