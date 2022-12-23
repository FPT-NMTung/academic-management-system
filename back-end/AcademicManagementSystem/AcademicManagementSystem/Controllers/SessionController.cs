using AcademicManagementSystem.Context;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.AttendanceController.AttendanceModel;
using AcademicManagementSystem.Models.AttendanceStatusController.AttendanceStatusModel;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.RoomController.RoomModel;
using AcademicManagementSystem.Models.RoomController.RoomTypeModel;
using AcademicManagementSystem.Models.SessionController;
using AcademicManagementSystem.Models.SessionTypeController.SessionTypeModel;
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
    private const int SessionTypeTheoryExam = 3;
    private const int SessionTypePracticalExam = 4;
    private const int ClassStatusMerged = 6;


    public SessionController(AmsContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpGet]
    [Route("api/classes-schedules/{id:int}/sessions")]
    [Authorize(Roles = "teacher")]
    public IActionResult GetSessionsByScheduleIdForTeacher(int id)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());

        var classSchedule = _context.ClassSchedules
            .Include(cs => cs.Class)
            .Include(cs => cs.Sessions)
            .ThenInclude(s => s.Room)
            .ThenInclude(r => r.Center)
            .FirstOrDefault(cs => cs.Id == id);

        if (classSchedule == null)
        {
            return NotFound(CustomResponse.NotFound("Class schedule not found"));
        }

        if (classSchedule.Class.ClassStatusId == ClassStatusMerged)
        {
            var error = ErrorDescription.Error["E0401"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (classSchedule.TeacherId != userId)
        {
            var error = ErrorDescription.Error["E0600"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
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

    [HttpGet]
    [Route("api/sessions/students")]
    [Authorize(Roles = "student")]
    public IActionResult GetSessionsOfStudent()
    {
        var userId = Convert.ToInt32(_userService.GetUserId());

        // get all session that this student has learn and in current class (IsActive) and except merged class
        var sessions = _context.Sessions
            .Include(s => s.ClassSchedule)
            .Include(s => s.ClassSchedule.Class)
            .Include(s => s.ClassSchedule.Class.StudentsClasses)
            .ThenInclude(sc => sc.Student)
            .Where(s => s.ClassSchedule.Class.StudentsClasses.Any(sc => sc.StudentId == userId && sc.IsActive)
                        && s.ClassSchedule.Class.ClassStatusId != ClassStatusMerged)
            .Select(s => new SessionDateForStudentResponse()
            {
                Id = s.Id,
                Title = s.Title,
                LearningDate = s.LearningDate,
                SessionType = s.SessionTypeId
            });

        return Ok(CustomResponse.Ok("Student get session with learning date successfully", sessions));
    }

    // get dates that have session(s) teach by teacher 
    [HttpGet]
    [Route("api/sessions/teachers")]
    [Authorize(Roles = "teacher")]
    public IActionResult GetSessionsOfTeacher()
    {
        var userId = Convert.ToInt32(_userService.GetUserId());

        // get all session that this student has learn and in current class (IsActive)
        var dates = _context.Sessions
            .Include(s => s.ClassSchedule)
            .Include(s => s.ClassSchedule.Class)
            // except session type exams and merged class
            .Where(s => s.ClassSchedule.TeacherId == userId &&
                        s.SessionTypeId != SessionTypeTheoryExam && s.SessionTypeId != SessionTypePracticalExam
                        && s.ClassSchedule.Class.ClassStatusId != ClassStatusMerged)
            .OrderBy(s => s.LearningDate.Date)
            .Select(s => s.LearningDate.Date).Distinct();

        return Ok(CustomResponse.Ok("Teacher get dates that have session(s) teach by this teacher successfully",
            dates));
    }

    [HttpPost]
    [Route("api/sessions/detail/teachers-get")]
    [Authorize(Roles = "teacher")]
    public IActionResult GetSessionsDetailOfTeacher([FromBody] DetailSessionForTeacherRequest request)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());

        var sessions = _context.Sessions
            .Include(s => s.ClassSchedule)
            .Include(s => s.ClassSchedule.Class)
            .Include(s => s.ClassSchedule.Module)
            .Include(s => s.Room)
            .Include(s => s.SessionType)
            .Where(s => s.LearningDate.Date == request.TeachDate.Date &&
                        s.ClassSchedule.TeacherId == userId &&
                        s.SessionTypeId != SessionTypeTheoryExam &&
                        s.SessionTypeId != SessionTypePracticalExam &&
                        s.ClassSchedule.Class.ClassStatusId != ClassStatusMerged)
            .Select(s => new DetailSessionForTeacherResponse()
            {
                Class = new BasicClassResponse()
                {
                    Id = s.ClassSchedule.Class.Id,
                    Name = s.ClassSchedule.Class.Name,
                },
                Module = new BasicModuleResponse()
                {
                    Id = s.ClassSchedule.Module.Id,
                    Name = s.ClassSchedule.Module.ModuleName,
                },
                SessionId = s.Id,
                SessionTitle = s.Title,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                SessionType = new SessionTypeResponse()
                {
                    Id = s.SessionType.Id,
                    Value = s.SessionType.Value
                },
                Room = new BasicRoomResponse()
                {
                    Id = s.Room.Id,
                    Name = s.Room.Name
                }
            });

        return Ok(CustomResponse.Ok("Teacher Get sessions detail successfully", sessions));
    }


    [HttpPost]
    [Route("api/sessions/detail/students/get")]
    [Authorize(Roles = "student")]
    public IActionResult GetDetailSessionByLearningDate([FromBody] DetailSessionForStudentRequest request)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());

        // get detail session by learning date of student
        var session = _context.Sessions
            .Include(s => s.ClassSchedule)
            .Include(s => s.ClassSchedule.Class)
            .Include(s => s.ClassSchedule.Module)
            .Include(s => s.ClassSchedule.Teacher)
            .Include(s => s.Room)
            .Include(s => s.ClassSchedule.Class.StudentsClasses)
            .ThenInclude(sc => sc.Student)
            .Include(s => s.Attendances)
            .ThenInclude(a => a.AttendanceStatus)
            .Where(s => s.ClassSchedule.Class.StudentsClasses.Any(sc => sc.StudentId == userId && sc.IsActive)
                        && s.LearningDate.Date == request.LearningDate.Date
                        && s.ClassSchedule.Class.ClassStatusId != ClassStatusMerged)
            .Select(s => new DetailSessionForStudentResponse()
            {
                Id = s.Id,
                Title = s.Title,
                LearningDate = s.LearningDate,
                SessionType = new SessionTypeResponse()
                {
                    Id = s.SessionType.Id,
                    Value = s.SessionType.Value
                },
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Attendance = new StudentAttendanceResponse()
                {
                    Student = new BasicStudentResponse()
                    {
                        UserId = s.ClassSchedule.Class.StudentsClasses
                            .FirstOrDefault(sc => sc.StudentId == userId)!.Student.UserId,
                        EnrollNumber = s.ClassSchedule.Class.StudentsClasses
                            .FirstOrDefault(sc => sc.StudentId == userId)!.Student.EnrollNumber,
                        EmailOrganization = s.ClassSchedule.Class.StudentsClasses
                            .FirstOrDefault(sc => sc.StudentId == userId)!.Student.User.EmailOrganization,
                        FirstName = s.ClassSchedule.Class.StudentsClasses
                            .FirstOrDefault(sc => sc.StudentId == userId)!.Student.User.FirstName,
                        LastName = s.ClassSchedule.Class.StudentsClasses
                            .FirstOrDefault(sc => sc.StudentId == userId)!.Student.User.LastName,
                        Avatar = s.ClassSchedule.Class.StudentsClasses
                            .FirstOrDefault(sc => sc.StudentId == userId)!.Student.User.Avatar,
                    },

                    AttendanceStatus = s.Attendances.Select(a => new AttendanceStatusResponse()
                    {
                        Id = a.AttendanceStatus.Id,
                        Value = a.AttendanceStatus.Value
                    }).FirstOrDefault(),

                    Note = s.Attendances.FirstOrDefault(a => a.StudentId == userId)!.Note
                },
                Class = new BasicClassResponse()
                {
                    Id = s.ClassSchedule.Class.Id,
                    Name = s.ClassSchedule.Class.Name,
                },
                Module = new BasicModuleResponse()
                {
                    Id = s.ClassSchedule.Module.Id,
                    Name = s.ClassSchedule.Module.ModuleName,
                },
                Teacher = new BasicTeacherInformationResponse()
                {
                    Id = s.ClassSchedule.Teacher.UserId,
                    EmailOrganization = s.ClassSchedule.Teacher.User.EmailOrganization,
                    FirstName = s.ClassSchedule.Teacher.User.FirstName,
                    LastName = s.ClassSchedule.Teacher.User.LastName
                },
                Room = new RoomResponse()
                {
                    Id = s.Room.Id,
                    Name = s.Room.Name,
                    Capacity = s.Room.Capacity,
                    CenterId = s.Room.Center.Id,
                    CenterName = s.Room.Center.Name,
                    RoomType = new RoomTypeResponse()
                    {
                        Id = s.Room.RoomTypeId,
                        Value = s.Room.RoomType.Value
                    },
                    IsActive = s.Room.IsActive
                }
            }).FirstOrDefault();

        if (session == null)
        {
            return Ok(CustomResponse.Ok("Session not found for you", null!));
        }

        var isGpaTaken = _context.GpaRecords
            .Include(gr => gr.Class)
            .Include(gr => gr.Module)
            .Include(gr => gr.Teacher)
            .Include(gr => gr.Session)
            .Include(gr => gr.Student)
            .Any(gr => gr.ClassId == session.Class.Id
                       && gr.ModuleId == session.Module.Id
                       && gr.TeacherId == session.Teacher.Id
                       && gr.SessionId == session.Id
                       && gr.StudentId == userId);

        if (session.SessionType.Id is SessionTypePracticalExam or SessionTypeTheoryExam)
        {
            session.CanTakeGpa = false;
        }
        else
        {
            if (session.LearningDate.Date > DateTime.Today || isGpaTaken)
            {
                session.CanTakeGpa = false;
            }

            if (session.LearningDate <= DateTime.Today && !isGpaTaken)
            {
                session.CanTakeGpa = true;
            }
        }

        return Ok(CustomResponse.Ok("Student get detail session successfully", session));
    }
}