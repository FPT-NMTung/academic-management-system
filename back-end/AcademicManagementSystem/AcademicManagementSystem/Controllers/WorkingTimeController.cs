using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.WorkingTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class WorkingTimeController : ControllerBase
{
    private readonly AmsContext _context;

    public WorkingTimeController(AmsContext context)
    {
        _context = context;
    }

    //get all working times
    [HttpGet]
    [Route("api/working-times")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetWorkingTimes()
    {
        var workingTimes = _context.WorkingTimes
            .Select(wt => new WorkingTimeResponse()
            {
                Id = wt.Id,
                Value = wt.Value
            });
        return Ok(CustomResponse.Ok("Get all working times successfully", workingTimes));
    }
}