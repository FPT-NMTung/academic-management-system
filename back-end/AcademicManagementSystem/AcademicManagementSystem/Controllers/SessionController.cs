using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.RoomController.RoomModel;
using AcademicManagementSystem.Models.Sessions;
using AcademicManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class SessionController : ControllerBase
{
    private readonly AmsContext _context;
    private readonly IUserService _userService;

    public SessionController(AmsContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpGet]
    [Route("api/classes-schedules/{id:int}/sessions")]
    [Authorize (Roles = "teacher")]
    public IActionResult GetSessionsByScheduleIdForTeacher(int id)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        
        var classSchedule = _context.ClassSchedules
            .Include(cs => cs.Sessions)
            .ThenInclude(s => s.Room)
            .ThenInclude(r => r.Center)
            .FirstOrDefault(cs => cs.Id == id);

        if (classSchedule == null)
        {
            return NotFound(CustomResponse.NotFound("Class schedule not found"));
        }
        
        if(classSchedule.TeacherId != userId)
        {
            return Unauthorized(CustomResponse.Unauthorized("You are not authorized to access this resource"));
        }
        
        var sessions = classSchedule.Sessions
            .Select(s => new SessionResponse()
            {
                Id = s.Id,
                Title = s.Title,
                LearningDate = s.LearningDate,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Room = new RoomResponse()
                {
                    Id = s.Room.Id,
                    Name = s.Room.Name,
                    Capacity = s.Room.Capacity,
                    CenterId = s.Room.Center.Id,
                    CenterName = s.Room.Center.Name,
                    IsActive = s.Room.IsActive
                },
                SessionType = s.SessionTypeId
            });

        return Ok(CustomResponse.Ok("Get sessions by class schedule successfully", sessions));
    }
}