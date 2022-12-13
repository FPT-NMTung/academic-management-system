using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.AttendanceController.AttendanceModel;
using AcademicManagementSystem.Models.AttendanceStatusController.AttendanceStatusModel;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.SessionTypeController.SessionTypeModel;
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
    private const int RoleTeacher = 3;

    public AttendanceController(AmsContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    // get all attendances of this class for 1 module (class schedule)
    // if teacher logged in, get attendances of all students in this schedule (all sessions)
    [HttpGet]
    [Route("api/classes-schedules/{id:int}/attendances")]
    [Authorize(Roles = "sro, teacher")]
    public IActionResult GetAttendancesOfClassSchedule(int id)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        var user = _context.Users.First(u => u.Id == userId);

        var schedule = _context.ClassSchedules
            .Include(cs => cs.Class)
            .FirstOrDefault(cs => cs.Id == id);
        if (schedule == null)
        {
            return NotFound(CustomResponse.NotFound("Class schedule not found"));
        }

        if (schedule.Class.CenterId != user.CenterId)
        {
            var error = ErrorDescription.Error["E0600"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // if teacher logged in, check if he is the teacher of this class
        if (user.RoleId == RoleTeacher && schedule.TeacherId != user.Id)
        {
            var error = ErrorDescription.Error["E0600"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

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

        var schedule = _context.ClassSchedules
            .Include(cs => cs.Class)
            .Include(cs => cs.Sessions)
            .FirstOrDefault(cs => cs.Id == scheduleId);
        if (schedule == null)
        {
            return NotFound(CustomResponse.NotFound("Class schedule not found"));
        }

        // not same center
        if (schedule.Class.CenterId != user.CenterId)
        {
            var error = ErrorDescription.Error["E0600"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // if teacher logged in, check if he is the teacher of this class
        if (user.RoleId == RoleTeacher && schedule.TeacherId != user.Id)
        {
            var error = ErrorDescription.Error["E0600"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (schedule.Sessions.All(s => s.Id != sessionId))
        {
            return NotFound(CustomResponse.NotFound("Session not found"));
        }

        var attendancesInSession = GetAllSessionsWithAttendances(user.CenterId, scheduleId)
            .Where(a => a.SessionId == sessionId);

        return Ok(CustomResponse.Ok("Get attendance for session successfully", attendancesInSession));
    }

    // student get his/her attendances of 1 current class, module
    [HttpGet]
    [Route("api/classes/{classId:int}/modules/{moduleId:int}/attendances/student")]
    [Authorize(Roles = "student")]
    public IActionResult StudentGetAttendancesByClassIdAndModuleId(int classId, int moduleId)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());

        // get class that have logged in student in this class and modules for this class
        var classContext = _context.Classes
            .Include(c => c.Center)
            .Include(c => c.StudentsClasses)
            .Include(c => c.CourseFamily)
            .ThenInclude(cf => cf.Courses)
            .ThenInclude(c => c.CoursesModulesSemesters)
            .ThenInclude(cms => cms.Module)
            .FirstOrDefault(c => c.Id == classId
                                 && c.StudentsClasses.Any(sc => sc.StudentId == userId));

        if (classContext == null)
        {
            return NotFound(CustomResponse.NotFound("Student not in this class"));
        }

        var isModuleInClass = classContext.CourseFamily.Courses
            .SelectMany(c => c.CoursesModulesSemesters)
            .Any(cms => cms.ModuleId == moduleId);

        if (!isModuleInClass)
        {
            return NotFound(CustomResponse.NotFound("Module not found for this class"));
        }

        var attendances = _context.Sessions
            .Include(s => s.Room)
            .Include(s => s.SessionType)
            .Include(s => s.ClassSchedule)
            .Include(s => s.ClassSchedule.Teacher.User)
            .Include(s => s.ClassSchedule.Class)
            .Include(s => s.ClassSchedule.Class.StudentsClasses)
            .Include(s => s.ClassSchedule.Module)
            .Include(s => s.Attendances)
            .Where(s => s.ClassSchedule.Class.Id == classId && s.ClassSchedule.ModuleId == moduleId)
            .Select(s => new SessionAttendanceForStudentResponse()
            {
                Id = s.Id,
                Title = s.Title,
                LearningDate = s.LearningDate,
                StartTime = s.StartTime,
                EndTime = s.EndTime,

                AttendanceStatus = s.Attendances.Where(a => a.StudentId == userId)
                    .Select(a => new AttendanceStatusResponse()
                    {
                        Id = a.AttendanceStatus.Id,
                        Value = a.AttendanceStatus.Value
                    }).FirstOrDefault(),
                SessionType = new SessionTypeResponse()
                {
                    Id = s.SessionType.Id,
                    Value = s.SessionType.Value
                },

                Note = s.Attendances.Where(a => a.StudentId == userId)
                    .Select(a => a.Note).FirstOrDefault(),

                Room = new BasicRoomResponse()
                {
                    Id = s.Room.Id,
                    Name = s.Room.Name
                },

                Class = new BasicClassResponse()
                {
                    Id = s.ClassSchedule.Class.Id,
                    Name = s.ClassSchedule.Class.Name
                },

                Module = new BasicModuleResponse()
                {
                    Id = s.ClassSchedule.Module.Id,
                    Name = s.ClassSchedule.Module.ModuleName
                },

                Teacher = new BasicTeacherInformationResponse()
                {
                    Id = s.ClassSchedule.Teacher.User.Id,
                    EmailOrganization = s.ClassSchedule.Teacher.User.EmailOrganization,
                    FirstName = s.ClassSchedule.Teacher.User.FirstName,
                    LastName = s.ClassSchedule.Teacher.User.LastName,
                }
            });

        return Ok(CustomResponse.Ok("Get attendances for student successfully", attendances));
    }

    [HttpPost]
    [Route("api/classes-schedules/{scheduleId:int}/sessions/{sessionId:int}/attendances/sros")]
    [Authorize(Roles = "sro")]
    public IActionResult SroTakeAttendance(int scheduleId, int sessionId,
        [FromBody] List<StudentAttendanceRequest> request)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        var user = _context.Users.First(u => u.Id == userId);

        var schedule = _context.ClassSchedules
            .Include(cs => cs.Sessions)
            .Include(cs => cs.Class)
            .Include(cs => cs.Class.StudentsClasses)
            .ThenInclude(sc => sc.Student)
            .FirstOrDefault(cs => cs.Id == scheduleId);
        if (schedule != null)
        {
            if (user.CenterId != schedule.Class.CenterId)
            {
                var error = ErrorDescription.Error["E0600"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
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

            // can't take attendance before learning date
            // if (DateTime.Today < session.LearningDate.Date)
            // {
            //     var error = ErrorDescription.Error["E0205"];
            //     return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            // }

            // remove request have duplicate studentId
            request = request.DistinctBy(r => r.StudentId).ToList();

            // get student in class (student that is active in class or not is draft)) 
            var studentsIdInClass = schedule.Class.StudentsClasses.Where(sc => sc.IsActive && !sc.Student.IsDraft)
                .Select(sc => sc.StudentId).ToList();

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
                    AttendanceStatusId = r.AttendanceStatusId,
                    Note = r.Note
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
    public IActionResult TeacherTakeAttendance(int scheduleId, int sessionId,
        [FromBody] List<StudentAttendanceRequest> request)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        var user = _context.Users.First(u => u.Id == userId);

        var schedule = _context.ClassSchedules
            .Include(cs => cs.Sessions)
            .Include(cs => cs.Class)
            .Include(cs => cs.Class.StudentsClasses)
            .ThenInclude(sc => sc.Student)
            .FirstOrDefault(cs => cs.Id == scheduleId);
        if (schedule != null)
        {
            // schedule not equal logged in user
            if (user.CenterId != schedule.Class.CenterId)
            {
                var error = ErrorDescription.Error["E0600"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            // teacher not teach this class schedule
            if (user.Id != schedule.TeacherId)
            {
                var error = ErrorDescription.Error["E0600"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
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

            // can't take attendance before learning date
            if (DateTime.Today < session.LearningDate.Date)
            {
                var error = ErrorDescription.Error["E0205"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            // remove request have duplicate studentId
            request = request.DistinctBy(r => r.StudentId).ToList();

            // get student in class (student that is active in class or not is draft)) 
            var studentsIdInClass = schedule.Class.StudentsClasses.Where(sc => sc.IsActive && !sc.Student.IsDraft)
                .Select(sc => sc.StudentId).ToList();

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
                    AttendanceStatusId = r.AttendanceStatusId,
                    Note = r.Note
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
            .Include(s => s.ClassSchedule.Class.StudentsClasses)
            .ThenInclude(sc => sc.Student)
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

                // all student in class and isActive,also not draft student 
                StudentAttendances = s.ClassSchedule.Class.StudentsClasses
                    .Where(sc => sc.IsActive && !sc.Student.IsDraft)
                    .Select(sc => new StudentAttendanceResponse()
                    {
                        Student = new BasicStudentResponse()
                        {
                            UserId = sc.StudentId,
                            EnrollNumber = sc.Student.EnrollNumber,
                            EmailOrganization = sc.Student.User.EmailOrganization,
                            FirstName = sc.Student.User.FirstName,
                            LastName = sc.Student.User.LastName,
                            Avatar = sc.Student.User.Avatar
                        },
                        AttendanceStatus = s.Attendances.Where(a => a.StudentId == sc.StudentId)
                            .Select(a => new AttendanceStatusResponse()
                            {
                                Id = a.AttendanceStatus.Id,
                                Value = a.AttendanceStatus.Value
                            }).FirstOrDefault(),
                        Note = s.Attendances.Where(a => a.StudentId == sc.StudentId)
                            .Select(a => a.Note).FirstOrDefault()
                    }).ToList()
            });
        return attendances;
    }
}