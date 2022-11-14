using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.ClassDaysController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class ClassDaysController : ControllerBase
{
    private readonly AmsContext _context;

    public ClassDaysController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/class-days")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetClassDays()
    {
        var classDays =
            _context.ClassDays.Select(cd => new ClassDaysResponse()
            {
                Id = cd.Id,
                Value = cd.Value
            });
        return Ok(CustomResponse.Ok("get all Class days successfully", classDays));
    }
}