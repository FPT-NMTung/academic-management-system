using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.ClassDaysController;
using AcademicManagementSystem.Models.ClassScheduleController.ClassScheduleModel;
using AcademicManagementSystem.Models.ClassStatusController;
using AcademicManagementSystem.Models.RoomController.RoomModel;
using AcademicManagementSystem.Models.RoomController.RoomTypeModel;
using AcademicManagementSystem.Models.Sessions;
using AcademicManagementSystem.Models.TeacherSkillController;
using AcademicManagementSystem.Models.UserController.TeacherController;
using AcademicManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class ClassScheduleController : ControllerBase
{
    private readonly AmsContext _context;
    private readonly IUserService _userService;
    private const int StatusScheduled = 1;
    private const int Theory = 1;
    private const int Practice = 2;
    private const int TheoryExam = 3;
    private const int PracticeExam = 4;

    public ClassScheduleController(AmsContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpGet]
    [Route("api/classes/{classId:int}/schedules")]
    [Authorize(Roles = "sro")]
    public IActionResult GetClassScheduleByClassId(int classId)
    {
        var classContext = _context.Classes.Find(classId);
        if (classContext == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }

        var classSchedule = GetClassSchedulesResponse(classId).OrderBy(response => response.StartDate);
        return Ok(CustomResponse.Ok("Get class schedule successfully", classSchedule));
    }

    [HttpGet]
    [Route("api/classes/{classId:int}/schedules/modules/{moduleId:int}")]
    [Authorize(Roles = "sro")]
    public IActionResult GetClassScheduleByClassIdAndScheduleId(int classId, int moduleId)
    {
        var classSelect = _context.Classes.Find(classId);
        if (classSelect == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }

        var classSchedule = _context.ClassSchedules
            .Include(cs => cs.Teacher)
            .ThenInclude(t => t.User)
            .Include(cs => cs.ClassStatus)
            .Include(cs => cs.ClassDays)
            .Include(cs => cs.Module)
            .Include(cs => cs.Sessions)
            .ThenInclude(cs => cs.SessionType)
            .Include(s => s.Sessions)
            .ThenInclude(r => r.Room)
            .ThenInclude(r => r.RoomType)
            .FirstOrDefault(cs => cs.ClassId == classId && cs.ModuleId == moduleId);

        if (classSchedule == null)
        {
            var check = _context.CoursesModulesSemesters
                .Include(cms => cms.Course)
                .Any(cms =>
                    cms.Course.CourseFamilyCode == classSelect.CourseFamilyCode &&
                    cms.ModuleId == moduleId);

            if (!check)
            {
                var error = ErrorDescription.Error["E2066"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            return NotFound(CustomResponse.NotFound("Class schedule not found"));
        }

        var res = new ClassScheduleResponse()
        {
            Id = classSchedule.Id,
            ClassId = classSchedule.ClassId,
            Duration = classSchedule.Duration,
            StartDate = classSchedule.StartDate,
            EndDate = classSchedule.EndDate,
            Teacher = new BasicTeacherInformationResponse()
            {
                Id = classSchedule.Teacher.UserId,
                LastName = classSchedule.Teacher.User.LastName,
                FirstName = classSchedule.Teacher.User.FirstName,
                EmailOrganization = classSchedule.Teacher.User.EmailOrganization,
            },
            ClassStatus = new ClassStatusResponse()
            {
                Id = classSchedule.ClassStatus.Id,
                Value = classSchedule.ClassStatus.Value,
            },
            ModuleId = classSchedule.Module.Id,
            ModuleName = classSchedule.Module.ModuleName,
            ClassDays = new ClassDaysResponse()
            {
                Id = classSchedule.ClassDays.Id,
                Value = classSchedule.ClassDays.Value,
            },
            ClassHourStart = classSchedule.ClassHourStart,
            ClassHourEnd = classSchedule.ClassHourEnd,
            Note = classSchedule.Note,
            Sessions = classSchedule.Sessions.Select(s => new SessionResponse()
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
                    Room = new RoomTypeResponse()
                    {
                        Id = s.Room.RoomType.Id,
                        Value = s.Room.RoomType.Value,
                    }
                },
                SessionType = s.SessionType.Id,
            }).ToList(),
            CreatedAt = classSchedule.CreatedAt,
            UpdatedAt = classSchedule.UpdatedAt,
        };

        return Ok(CustomResponse.Ok("Get detail schedule successfully", res));
    }

    [HttpPost]
    [Route("api/classes/{classId:int}/schedules")]
    [Authorize(Roles = "sro")]
    public IActionResult CreateClassSchedule(int classId, [FromBody] CreateClassScheduleRequest request)
    {
        var centerId = _context.Sros.Include(sro => sro.User)
            .FirstOrDefault(sro => sro.UserId == Int32.Parse(_userService.GetUserId()))?.User.CenterId;

        var classContext = _context.Classes.FirstOrDefault(cl => cl.CenterId == centerId && cl.Id == classId);
        if (classContext == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }

        var isDraftStudent =
            _context.StudentsClasses.Any(sc => sc.ClassId == classContext.Id && sc.Student.IsDraft);

        if (isDraftStudent)
        {
            var error = ErrorDescription.Error["E0094"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var module = GetModulesBelongToThisClass(classId)
            .FirstOrDefault(m => m.Id == request.ModuleId && centerId == m.CenterId);

        if (module == null)
        {
            var error = ErrorDescription.Error["E0077"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var teacher =
            _context.Teachers.FirstOrDefault(t => t.UserId == request.TeacherId && t.User.CenterId == centerId);

        if (teacher == null)
        {
            var error = ErrorDescription.Error["E0077_1"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var errorCode = GetErrorCodeOccurWhenCreate(classId, request);
        if (errorCode != null)
        {
            var error = ErrorDescription.Error[errorCode];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var errorCode3 = GetErrorCodeWhenConflictResource(classId, request);
        if (errorCode3 != null)
        {
            var error = ErrorDescription.Error[errorCode3];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var classScheduleToCreate = new ClassSchedule
        {
            ClassId = classId,
            ModuleId = request.ModuleId,
            TeacherId = request.TeacherId,
            ClassDaysId = request.ClassDaysId,
            WorkingTimeId = request.WorkingTimeId,
            ClassStatusId = StatusScheduled,
            TheoryRoomId = request.TheoryRoomId,
            LabRoomId = request.LabRoomId,
            ExamRoomId = request.ExamRoomId,
            Duration = request.Duration,
            StartDate = request.StartDate,
            ClassHourStart = request.ClassHourStart,
            ClassHourEnd = request.ClassHourEnd,
            Note = request.Note,
            Sessions = new List<Session>(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var practiceSessions = new List<int>();
        // remove duplicate
        if (request.PracticeSession != null)
        {
            practiceSessions = request.PracticeSession.Distinct().ToList();
        }

        var learningDate = request.StartDate;
        var endDate = new DateTime();

        // get list day off of teacher
        var dayOff = _context.DaysOff.Where(d =>
            (d.TeacherId == null || d.TeacherId == request.TeacherId) && d.Date.Date >= request.StartDate.Date);
        var teacherDayOff = dayOff.ToList();
        var globalDayOff = dayOff.Where(d => d.TeacherId == null).ToList();

        // auto add session base on durations
        var i = 0;
        while (i < request.Duration)
        {
            if (IsValidLearningDate(request, learningDate, teacherDayOff, true))
            {
                i++;
                var session = new Session
                {
                    SessionTypeId = Theory,
                    Title = module.ModuleName + " - T" + i,
                    LearningDate = learningDate,
                    StartTime = request.ClassHourStart,
                    EndTime = request.ClassHourEnd,
                };

                if (module.ModuleType == 1)
                {
                    session.SessionTypeId = Theory;
                    session.Title = module.ModuleName + " - T" + i;
                    session.RoomId = (int)request.TheoryRoomId!;
                }

                if (module.ModuleType == 2)
                {
                    session.SessionTypeId = Practice;
                    session.Title = module.ModuleName + " - P" + i;
                    session.RoomId = (int)request.LabRoomId!;
                }

                if (module.ModuleType == 3 && practiceSessions.Any(practice => practice == i))
                {
                    session.SessionTypeId = Practice;
                    session.Title = module.ModuleName + " - P" + i;
                    session.RoomId = (int)request.LabRoomId!;
                }
                else if (module.ModuleType == 3 && !practiceSessions.Any(practice => practice == i))
                {
                    session.SessionTypeId = Theory;
                    session.Title = module.ModuleName + " - T" + i;
                    session.RoomId = (int)request.TheoryRoomId!;
                }

                classScheduleToCreate.Sessions.Add(session);
            }

            if (i == request.Duration)
                endDate = learningDate;

            learningDate += TimeSpan.FromDays(1);
        }

        classScheduleToCreate.EndDate = endDate;

        // check if module has theory exam
        if (new List<int>() { 1, 3 }.Contains(module.ExamType))
        {
            while (true)
            {
                if (IsValidLearningDate(request, learningDate, globalDayOff, false))
                {
                    var session = new Session
                    {
                        SessionTypeId = TheoryExam,
                        RoomId = (int)request.ExamRoomId!,
                        Title = module.ModuleName + " - TheoryExam",
                        LearningDate = learningDate,
                        StartTime = request.ClassHourStart + TimeSpan.FromHours(1),
                        EndTime = request.ClassHourStart + TimeSpan.FromHours(2),
                    };
                    classScheduleToCreate.TheoryExamDate = learningDate;
                    learningDate += TimeSpan.FromDays(1);
                    classScheduleToCreate.Sessions.Add(session);
                    break;
                }

                learningDate += TimeSpan.FromDays(1);
            }
        }

        // check if module has practice exam
        if (new List<int>() { 2, 3 }.Contains(module.ExamType))
        {
            while (true)
            {
                if (IsValidLearningDate(request, learningDate, globalDayOff, false))
                {
                    var session = new Session
                    {
                        SessionTypeId = PracticeExam,
                        RoomId = (int)request.ExamRoomId!,
                        Title = module.ModuleName + " - PracticeExam",
                        LearningDate = learningDate,
                        StartTime = request.ClassHourStart + TimeSpan.FromHours(1),
                        EndTime = request.ClassHourStart + TimeSpan.FromHours(2.5),
                    };
                    classScheduleToCreate.PracticalExamDate = learningDate;
                    classScheduleToCreate.Sessions.Add(session);
                    break;
                }

                learningDate += TimeSpan.FromDays(1);
            }
        }

        var isRangeTimeInvalid = IsRangeTimeInvalid(classScheduleToCreate);
        if (isRangeTimeInvalid)
        {
            var error = ErrorDescription.Error["E2070"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var isTeacherBusy = CheckTeacherBusy(classScheduleToCreate);
        if (isTeacherBusy)
        {
            var error = ErrorDescription.Error["E2064"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var isRoomBusy = CheckRoomBusy(classScheduleToCreate);
        if (isRoomBusy)
        {
            var error = ErrorDescription.Error["E2065"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // update class status
        classContext.ClassStatusId = StatusScheduled;

        _context.ClassSchedules.Add(classScheduleToCreate);

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0078"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var classScheduleResponse =
            GetClassSchedulesResponse(classId).First(cs => cs.Id == classScheduleToCreate.Id);

        return Ok(CustomResponse.Ok("Create class schedule successfully", classScheduleResponse));
    }

    [HttpDelete]
    [Route("api/classes/{classId:int}/schedules/{classScheduleId:int}")]
    [Authorize(Roles = "sro")]
    public IActionResult DeleteClassSchedule(int classId, int classScheduleId)
    {
        var classContext = _context.Classes.Find(classId);
        if (classContext == null)
        {
            var error = ErrorDescription.Error["E2066"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var classSchedule = _context.ClassSchedules.Include(cs => cs.Sessions)
            .FirstOrDefault(cs => cs.Id == classScheduleId);
        if (classSchedule == null)
        {
            var error = ErrorDescription.Error["E2067"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        classSchedule.Sessions.ToList().ForEach(s => _context.Sessions.Remove(s));

        try
        {
            _context.ClassSchedules.Remove(classSchedule);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E2069"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Delete class schedule successfully", null!));
    }

    private bool CheckTeacherBusy(ClassSchedule classScheduleToCreate)
    {
        var listSchedule = _context.ClassSchedules
            .Include(cs => cs.Sessions)
            .Where(cs =>
                cs.TeacherId == classScheduleToCreate.TeacherId &&
                cs.WorkingTimeId == classScheduleToCreate.WorkingTimeId &&
                cs.ClassDaysId == classScheduleToCreate.ClassDaysId &&
                !((classScheduleToCreate.StartDate < cs.StartDate && classScheduleToCreate.EndDate < cs.StartDate) ||
                  (classScheduleToCreate.StartDate > cs.EndDate && classScheduleToCreate.EndDate > cs.EndDate))
            );

        foreach (var schedule in listSchedule)
        {
            var sessions = schedule.Sessions
                .Where(s => s.SessionTypeId != TheoryExam && s.SessionTypeId != PracticeExam)
                .OrderBy(s => s.LearningDate).ToList();

            var firstSession = sessions.First();
            var lastSession = sessions.Last();

            if ((classScheduleToCreate.StartDate < firstSession.LearningDate &&
                 classScheduleToCreate.EndDate < firstSession.LearningDate) ||
                (classScheduleToCreate.StartDate > lastSession.LearningDate &&
                 classScheduleToCreate.EndDate > lastSession.LearningDate))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private bool CheckRoomBusy(ClassSchedule classScheduleToCreate)
    {
        var check = classScheduleToCreate.Sessions.ToList()
            .Find(s =>
                _context.Sessions.Where(s1 =>
                    s1.LearningDate.Date == s.LearningDate.Date &&
                    s1.RoomId == s.RoomId &&
                    s1.ClassSchedule.WorkingTimeId == classScheduleToCreate.WorkingTimeId
                ).ToList().Count > 0);

        if (check != null) return true;

        return false;
    }

    private bool IsRangeTimeInvalid(ClassSchedule classScheduleToCreate)
    {
        var sessions = classScheduleToCreate.Sessions.ToList();
        var firstSession = sessions.First();
        var lastSession = sessions.Last();

        var listSchedule = _context.ClassSchedules
            .Include(cs => cs.Sessions)
            .Where(cs => cs.ClassId == classScheduleToCreate.ClassId)
            .ToList();
        foreach (var schedule in listSchedule)
        {
            var listSession = schedule.Sessions.ToList();
            foreach (var session in listSession)
            {
                if (firstSession.LearningDate.Date <= session.LearningDate.Date &&
                    session.LearningDate.Date <= lastSession.LearningDate.Date)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsValidLearningDate(CreateClassScheduleRequest request, DateTime learningDate, List<DayOff> listDayOff,
        bool isTeacherDayOff)
    {
        if (learningDate.DayOfWeek is DayOfWeek.Sunday)
            return false;

        switch (request.ClassDaysId)
        {
            // monday, wednesday, friday
            case 1:
                if (learningDate.DayOfWeek is DayOfWeek.Tuesday or DayOfWeek.Thursday or DayOfWeek.Saturday)
                    return false;
                break;
            // tuesday, thursday, saturday
            case 2:
                if (learningDate.DayOfWeek is DayOfWeek.Monday or DayOfWeek.Wednesday or DayOfWeek.Friday)
                    return false;
                break;
        }

        var isDayOff = listDayOff.Find(item => item.Date.Date == learningDate.Date);

        if (isTeacherDayOff && isDayOff != null)
        {
            if (isDayOff.WorkingTimeId == 7)
            {
                return false;
            }

            if (new List<int> { 1, 4, 5 }.Contains(isDayOff.WorkingTimeId) &&
                (IsTimeInRange(TimeSpan.FromHours(8), TimeSpan.FromHours(12), request.ClassHourStart)
                 || IsTimeInRange(TimeSpan.FromHours(8), TimeSpan.FromHours(12), request.ClassHourEnd)))
            {
                return false;
            }

            if (new List<int> { 2, 4, 6 }.Contains(isDayOff.WorkingTimeId) &&
                (IsTimeInRange(TimeSpan.FromHours(13), TimeSpan.FromHours(17), request.ClassHourStart)
                 || IsTimeInRange(TimeSpan.FromHours(13), TimeSpan.FromHours(17), request.ClassHourEnd)))
            {
                return false;
            }

            if (new List<int> { 3, 5, 6 }.Contains(isDayOff.WorkingTimeId) &&
                (IsTimeInRange(TimeSpan.FromHours(18), TimeSpan.FromHours(22), request.ClassHourStart)
                 || IsTimeInRange(TimeSpan.FromHours(18), TimeSpan.FromHours(22), request.ClassHourEnd)))
            {
                return false;
            }
        }

        if (!isTeacherDayOff && isDayOff != null)
        {
            return false;
        }

        return true;
    }

    private string? GetErrorCodeOccurWhenCreate(int classId, CreateClassScheduleRequest request)
    {
        var isModuleScheduledForThisClass =
            _context.ClassSchedules.Any(cs => cs.ClassId == classId && cs.ModuleId == request.ModuleId);

        var theoryRoom = _context.Rooms.Find(request.TheoryRoomId);
        var labRoom = _context.Rooms.Find(request.LabRoomId);

        // Check if module is already scheduled for this class -> can't create new schedule
        if (isModuleScheduledForThisClass)
            return "E0079";

        if (request.Duration <= 0)
            return "E0080";

        if (request.StartDate.Date < DateTime.Now.Date)
            return "E0081";

        switch (request.WorkingTimeId)
        {
            case 1:
                if (!IsTimeInRange(TimeSpan.FromHours(8), TimeSpan.FromHours(12), request.ClassHourStart)
                    || !IsTimeInRange(TimeSpan.FromHours(8), TimeSpan.FromHours(12), request.ClassHourEnd))
                {
                    return "E0095";
                }

                break;
            case 2:
                if (!IsTimeInRange(TimeSpan.FromHours(13), TimeSpan.FromHours(17), request.ClassHourStart)
                    || !IsTimeInRange(TimeSpan.FromHours(13), TimeSpan.FromHours(17), request.ClassHourEnd))
                {
                    return "E0095";
                }

                break;
            case 3:
                if (!IsTimeInRange(TimeSpan.FromHours(18), TimeSpan.FromHours(22), request.ClassHourStart)
                    || !IsTimeInRange(TimeSpan.FromHours(18), TimeSpan.FromHours(22), request.ClassHourEnd))
                {
                    return "E0095";
                }

                break;
            default: return "E0095";
        }

        if (request.PracticeSession != null && request.PracticeSession.Any() &&
            request.PracticeSession.Any(practice => practice > request.Duration || practice <= 0))
        {
            return "E0088";
        }

        if (request.ClassHourEnd - request.ClassHourStart < TimeSpan.FromHours(1)
            || request.ClassHourEnd - request.ClassHourStart > TimeSpan.FromHours(4))
        {
            return "E0082";
        }

        if (request.ClassHourEnd > TimeSpan.FromHours(22) || request.ClassHourStart < TimeSpan.FromHours(8))
        {
            return "E0082_1";
        }

        // not theory room
        if (theoryRoom != null && theoryRoom.RoomTypeId != 1)
        {
            return "E0083";
        }

        //not lab room
        if (labRoom != null && labRoom.RoomTypeId != 2)
        {
            return "E0084";
        }

        return null;
    }

    // check all resource in class schedule is busy in session date
    private string? GetErrorCodeWhenConflictResource(int classId, CreateClassScheduleRequest request)
    {
        var classSchedule = _context.ClassSchedules
            .Include(cs => cs.Sessions)
            .FirstOrDefault(cs => cs.ClassId != classId);

        if (classSchedule == null) return null;

        // check teacher busy in schedule
        // var isTeacherBusy = classSchedule.Sessions.Any(s
        //     => s.ClassSchedule.TeacherId == request.TeacherId &&
        //        s.LearningDate.Date == request.StartDate.Date &&
        //        (IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourStart) ||
        //         IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourEnd)));
        //
        // if (isTeacherBusy)
        //     return "E0093";
        //
        // var isTheoryRoomBusy = classSchedule.Sessions.Any(s
        //     => s.RoomId == request.TheoryRoomId &&
        //        s.LearningDate.Date == request.StartDate.Date &&
        //        (IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourStart) ||
        //         IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourEnd)));
        //
        // if (isTheoryRoomBusy)
        //     return "E0090";
        //
        // var isLabRoomBusy = classSchedule.Sessions.Any(s
        //     => s.RoomId == request.LabRoomId &&
        //        s.LearningDate.Date == request.StartDate.Date &&
        //        (IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourStart) ||
        //         IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourEnd)));
        //
        // if (isLabRoomBusy)
        //     return "E0091";
        //
        // var isExamRoomBusy = classSchedule.Sessions.Any(s
        //     => s.SessionTypeId is PracticeExam or TheoryExam &&
        //        s.RoomId == request.ExamRoomId &&
        //        s.LearningDate.Date == request.StartDate.Date &&
        //        (IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourStart) ||
        //         IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourEnd)));
        //
        // if (isExamRoomBusy)
        //     return "E0092";

        return null;
    }

    private IQueryable<ClassScheduleResponse> GetClassSchedulesResponse(int classId)
    {
        return _context.ClassSchedules
            .Where(c => c.ClassId == classId)
            .Select(cs => new ClassScheduleResponse
            {
                Id = cs.Id,
                ClassId = cs.ClassId,
                ClassName = cs.Class.Name,
                ModuleId = cs.ModuleId,
                ModuleName = cs.Module.ModuleName,
                Teacher = new BasicTeacherInformationResponse()
                {
                    Id = cs.TeacherId,
                    FirstName = cs.Teacher.User.FirstName,
                    LastName = cs.Teacher.User.LastName,
                    EmailOrganization = cs.Teacher.User.EmailOrganization,
                },
                ClassDays = new ClassDaysResponse()
                {
                    Id = cs.ClassDaysId,
                    Value = cs.ClassDays.Value,
                },
                ClassStatus = new ClassStatusResponse()
                {
                    Id = cs.ClassStatusId,
                    Value = cs.ClassStatus.Value,
                },
                TheoryRoomId = cs.TheoryRoomId,
                TheoryRoomName = cs.TheoryRoom.Name,
                LabRoomId = cs.LabRoomId,
                LabRoomName = cs.LabRoom.Name,
                ExamRoomId = cs.ExamRoomId,
                ExamRoomName = cs.ExamRoom.Name,
                Duration = cs.Duration,
                StartDate = cs.StartDate,
                EndDate = cs.EndDate,
                TheoryExamDate = cs.TheoryExamDate,
                PracticalExamDate = cs.PracticalExamDate,
                ClassHourStart = cs.ClassHourStart,
                ClassHourEnd = cs.ClassHourEnd,
                Note = cs.Note,
                CreatedAt = cs.CreatedAt,
                UpdatedAt = cs.UpdatedAt
            });
    }

    // method get module in this class
    private IQueryable<Module> GetModulesBelongToThisClass(int classId)
    {
        return _context.Modules
            .Include(m => m.CoursesModulesSemesters)
            .ThenInclude(cms => cms.Course)
            .ThenInclude(c => c.CourseFamily)
            .ThenInclude(cf => cf.Classes)
            .Where(m => m.CoursesModulesSemesters.Any(cms => cms.Course.CourseFamily.Classes
                .Any(c => c.Id == classId)));
    }

    private bool IsTimeInRange(TimeSpan startHour, TimeSpan endHour, TimeSpan timeToCheck)
    {
        return timeToCheck <= endHour && timeToCheck >= startHour;
    }
}