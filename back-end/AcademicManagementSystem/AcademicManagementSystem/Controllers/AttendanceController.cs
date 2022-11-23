using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.AttendanceController.AttendanceModel;
using AcademicManagementSystem.Models.AttendanceStatusController.AttendanceStatusModel;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class AttendanceController : ControllerBase
{
    private readonly AmsContext _context;
    private readonly IUserService _userService;

    public AttendanceController(AmsContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }
    
    // get all attendances of this class for 1 module (class schedule)
    [HttpGet]
    [Route("api/classes-schedules/{id:int}/attendances")]
    [Authorize(Roles = "sro, teacher")]
    public IActionResult GetAttendancesOfClassSchedule(int id)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        var user = _context.Users.First(u => u.Id == userId);

        var attendances = GetAllSessionsWithAttendances(user.CenterId, id);
        
        return Ok(CustomResponse.Ok("Get attendance for all sessions successfully", attendances));
    }

    [HttpGet]
    [Route("api/classes-schedules/{scheduleId:int}/sessions/{sessionId:int}/attendances")]
    [Authorize(Roles = "sro, teacher")]
    public IActionResult GetAttendancesBySessionId(int scheduleId, int sessionId)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        var user = _context.Users.First(u => u.Id == userId);

        var attendancesInSession = GetAllSessionsWithAttendances(user.CenterId, scheduleId)
            .Where(a => a.SessionId == sessionId);
        
        return Ok(CustomResponse.Ok("Get attendance for session successfully", attendancesInSession));
    }

    // get all sessions with attendances in a class schedule
    private IQueryable<SessionAttendanceResponse> GetAllSessionsWithAttendances(int centerId, int scheduleId)
    {
        var attendances = _context.Sessions
            .Include(s => s.ClassSchedule)
            .Include(s => s.ClassSchedule.Class)
            .Include(s => s.ClassSchedule.Module)
            .Include(s => s.Attendances)
            .Where(s => s.ClassSchedule.Class.CenterId == centerId && s.ClassSchedule.Id == scheduleId)
            .Select(s => new SessionAttendanceResponse()
            {
                SessionId = s.Id,
                Title = s.Title,
                LearningDate = s.LearningDate,

                ClassSchedule = new BasicClassScheduleResponse()
                {
                    Id = s.ClassScheduleId,
                    Class = new BasicClassResponse()
                    {
                        Id = s.ClassSchedule.Class.Id,
                        Name = s.ClassSchedule.Class.Name
                    },
                    Module = new BasicModuleResponse()
                    {
                        Id = s.ClassSchedule.Module.Id,
                        Name = s.ClassSchedule.Module.ModuleName
                    }
                },
                
                StudentAttendances = s.Attendances.Select(a => new StudentAttendanceResponse()
                {
                    Student = new BasicStudentResponse()
                    {
                        UserId = a.StudentId,
                        EnrollNumber = a.Student.EnrollNumber,
                        EmailOrganization = a.Student.User.EmailOrganization,
                        FirstName = a.Student.User.FirstName,
                        LastName = a.Student.User.LastName
                    },
                    AttendanceStatus = new AttendanceStatusResponse()
                    {
                        Id = a.AttendanceStatus.Id,
                        Value = a.AttendanceStatus.Value
                    },
                    Note = a.Note
                }).ToList()
            });
        return attendances;
    }
}