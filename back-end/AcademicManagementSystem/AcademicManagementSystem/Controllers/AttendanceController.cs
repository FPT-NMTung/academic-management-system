using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
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

    [HttpPost]
    [Route("api/classes-schedules/{scheduleId:int}/sessions/{sessionId:int}/attendances/sros")]
    [Authorize(Roles = "sro")]
    public IActionResult SroTakeAttendance(int scheduleId, int sessionId, List<StudentAttendanceRequest> request)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        var user = _context.Users.First(u => u.Id == userId);

        var schedule = _context.ClassSchedules
            .Include(cs => cs.Sessions)
            .Include(cs => cs.Class)
            .Include(cs => cs.Class.StudentsClasses)
            .FirstOrDefault(cs => cs.Id == scheduleId);
        if (schedule != null)
        {
            if (user.CenterId != schedule.Class.CenterId)
            {
                return Unauthorized(CustomResponse.Unauthorized("Access denied, center not match"));
            }

            var session = schedule.Sessions.FirstOrDefault(s => s.Id == sessionId);

            // check session in this schedule
            if (session == null)
            {
                var error = ErrorDescription.Error["E0203"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            // remove attendance before take attendance
            var attendances = _context.Attendances.Where(a => a.SessionId == sessionId).ToList();
            _context.Attendances.RemoveRange(attendances);

            if (attendances.Any())
            {
                _context.SaveChanges();
            }

            // can't take attendance before learning date
            // if (DateTime.Today < session.LearningDate.Date)
            // {
            //     var error = ErrorDescription.Error["E0205"];
            //     return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            // }

            // remove request have duplicate studentId
            request = request.DistinctBy(r => r.StudentId).ToList();

            var studentsIdInClass = schedule.Class.StudentsClasses.Select(sc => sc.StudentId).ToList();

            var requestStudentsId = request.Select(r => r.StudentId).ToList();

            // students from request that not in this class
            var requestStudentIdNotInClass = requestStudentsId.Except(studentsIdInClass).ToList();

            // students in class but not in request
            var studentsNotInRequest = studentsIdInClass.Except(requestStudentsId).ToList();

            // check request is have all student in class (neither excess nor lack)
            if (requestStudentIdNotInClass.Any() || studentsNotInRequest.Any())
            {
                var error = ErrorDescription.Error["E0202"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            foreach (var r in request)
            {
                var attendance = new Attendance
                {
                    SessionId = sessionId,
                    StudentId = r.StudentId,
                    AttendanceStatusId = r.AttendanceStatusId
                };
                _context.Attendances.Add(attendance);
            }

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                var error = ErrorDescription.Error["E0200"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            var response = GetAllSessionsWithAttendances(user.CenterId, scheduleId)
                .Where(a => a.SessionId == sessionId);

            return Ok(CustomResponse.Ok("SRO take attendances successfully", response));
        }

        var err = ErrorDescription.Error["E0204"];
        return BadRequest(CustomResponse.BadRequest(err.Message, err.Type));
    }

    [HttpPost]
    [Route("api/classes-schedules/{scheduleId:int}/sessions/{sessionId:int}/attendances/teachers")]
    [Authorize(Roles = "teacher")]
    public IActionResult TeacherTakeAttendance(int scheduleId, int sessionId, List<StudentAttendanceRequest> request)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        var user = _context.Users.First(u => u.Id == userId);

        var schedule = _context.ClassSchedules
            .Include(cs => cs.Sessions)
            .Include(cs => cs.Class)
            .Include(cs => cs.Class.StudentsClasses)
            .FirstOrDefault(cs => cs.Id == scheduleId);
        if (schedule != null)
        {
            // schedule not equal logged in user
            if (user.CenterId != schedule.Class.CenterId)
            {
                return Unauthorized(CustomResponse.Unauthorized("Access denied, center not match"));
            }

            // teacher not teach this class schedule
            if (user.Id != schedule.TeacherId)
            {
                return Unauthorized(CustomResponse.Unauthorized("Access denied, schedule not for this teacher"));
            }

            var session = schedule.Sessions.FirstOrDefault(s => s.Id == sessionId);

            // check session in this schedule
            if (session == null)
            {
                var error = ErrorDescription.Error["E0203"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            // remove attendance before take attendance
            var attendances = _context.Attendances.Where(a => a.SessionId == sessionId).ToList();
            _context.Attendances.RemoveRange(attendances);

            if (attendances.Any())
            {
                _context.SaveChanges();
            }

            // can't take attendance before learning date
            if (DateTime.Today < session.LearningDate.Date)
            {
                var error = ErrorDescription.Error["E0205"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            // remove request have duplicate studentId
            request = request.DistinctBy(r => r.StudentId).ToList();

            var studentsIdInClass = schedule.Class.StudentsClasses.Select(sc => sc.StudentId).ToList();

            var requestStudentsId = request.Select(r => r.StudentId).ToList();

            // students from request that not in this class
            var requestStudentIdNotInClass = requestStudentsId.Except(studentsIdInClass).ToList();

            // students in class but not in request
            var studentsNotInRequest = studentsIdInClass.Except(requestStudentsId).ToList();

            // check request is have all student in class (neither excess nor lack)
            if (requestStudentIdNotInClass.Any() || studentsNotInRequest.Any())
            {
                var error = ErrorDescription.Error["E0202"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            foreach (var r in request)
            {
                var attendance = new Attendance
                {
                    SessionId = sessionId,
                    StudentId = r.StudentId,
                    AttendanceStatusId = r.AttendanceStatusId
                };
                _context.Attendances.Add(attendance);
            }

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                var error = ErrorDescription.Error["E0200"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            var response = GetAllSessionsWithAttendances(user.CenterId, scheduleId)
                .Where(a => a.SessionId == sessionId);

            return Ok(CustomResponse.Ok("SRO take attendances successfully", response));
        }

        var err = ErrorDescription.Error["E0204"];
        return BadRequest(CustomResponse.BadRequest(err.Message, err.Type));
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