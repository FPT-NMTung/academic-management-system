using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.TeacherTypeController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class TeacherTypeController : ControllerBase
{
    private readonly AmsContext _context;

    public TeacherTypeController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/teacher-types")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetTeacherType()
    {
        var teacherTypes = _context.TeacherTypes.Select(tt => new TeacherTypeResponse()
        {
            Id = tt.Id,
            Value = tt.Value
        });
        return Ok(CustomResponse.Ok("Get all teacher types successfully", teacherTypes));
    }
}