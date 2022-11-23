using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.AttendanceStatusController.AttendanceStatusModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class AttendanceStatusController : ControllerBase
{
    private readonly AmsContext _context;

    public AttendanceStatusController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/attendance-statuses")]
    [Authorize (Roles = "sro, teacher")]
    public IActionResult GetAttendanceStatuses()
    {
        var statuses = _context.AttendanceStatuses.Select(s => new AttendanceStatusResponse()
        {
            Id = s.Id,
            Value = s.Value
        }).ToList();
        return Ok(CustomResponse.Ok("Get Attendance Statuses successfully", statuses));
    }
}