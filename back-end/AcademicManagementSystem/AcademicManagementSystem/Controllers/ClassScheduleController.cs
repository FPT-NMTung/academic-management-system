using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.ClassDaysController;
using AcademicManagementSystem.Models.ClassScheduleController.ClassScheduleModel;
using AcademicManagementSystem.Models.ClassStatusController;
using AcademicManagementSystem.Models.TeacherSkillController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class ClassScheduleController : ControllerBase
{
    private readonly AmsContext _context;
    private const int StatusScheduled = 1;
    private const int Theory = 1;
    private const int Practice = 2;
    private const int TheoryExam = 3;
    private const int PracticeExam = 4;

    public ClassScheduleController(AmsContext context)
    {
        _context = context;
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

    [HttpPost]
    [Route("api/classes/{classId:int}/schedules")]
    [Authorize(Roles = "sro")]
    public IActionResult CreateClassSchedule(int classId, [FromBody] CreateClassScheduleRequest request)
    {
        var classContext = _context.Classes.Find(classId);
        if (classContext == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }

        var module = GetModulesBelongToThisClass(classId)
            .FirstOrDefault(m => m.Id == request.ModuleId);

        if (module == null)
        {
            var error = ErrorDescription.Error["E0077"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var errorCode = GetErrorCodeOccurWhenCreate(classId, request);
        if (errorCode != null)
        {
            var error = ErrorDescription.Error[errorCode];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var errorCode2 = GetCodeIfStartDateNotTheNextOfLastSession(classId, request);
        if (errorCode2 != null)
        {
            var error = ErrorDescription.Error[errorCode2];
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

        // auto add session base on durations
        var i = 0;
        while (i < request.Duration)
        {
            if (IsValidLearningDate(request, learningDate))
            {
                i++;
                var session = new Session
                {
                    SessionTypeId = Theory,
                    RoomId = request.TheoryRoomId,
                    Title = module.ModuleName + " - T" + i,
                    LearningDate = learningDate,
                    StartTime = request.ClassHourStart,
                    EndTime = request.ClassHourEnd,
                };

                if (practiceSessions.Any(practice => practice == i))
                {
                    session.SessionTypeId = Practice;
                    session.Title = module.ModuleName + " - P" + i;
                    session.RoomId = request.LabRoomId;
                }

                classScheduleToCreate.Sessions.Add(session);
            }

            if (i == request.Duration)
            {
                endDate = learningDate;
            }

            learningDate += TimeSpan.FromDays(1);
        }

        classScheduleToCreate.EndDate = endDate;

        var theoryExamDate = endDate + TimeSpan.FromDays(2);

        // increase day to check if day is not valid
        while (!IsValidLearningDate(request, theoryExamDate))
        {
            theoryExamDate += TimeSpan.FromDays(1);
        }

        // theory exam take 1 hour
        var theoryExamSession = new Session()
        {
            SessionTypeId = TheoryExam,
            Title = module.ModuleName + " Theory Exam",
            LearningDate = theoryExamDate,
            StartTime = request.ClassHourStart + TimeSpan.FromHours(1),
            EndTime = request.ClassHourStart + TimeSpan.FromHours(2),
            RoomId = request.ExamRoomId
        };

        var practiceExamDate = theoryExamDate + TimeSpan.FromDays(2);

        // increase day to check if day is not valid
        while (!IsValidLearningDate(request, practiceExamDate))
        {
            practiceExamDate += TimeSpan.FromDays(1);
        }

        // practice exam take 1.5 hours
        var practiceExamSession = new Session()
        {
            SessionTypeId = PracticeExam,
            Title = module.ModuleName + " Practice Exam",
            LearningDate = practiceExamDate,
            StartTime = request.ClassHourStart + TimeSpan.FromHours(1),
            EndTime = request.ClassHourStart + TimeSpan.FromHours(2.5),
            RoomId = request.ExamRoomId
        };

        // add exam session base on exam type
        switch (module.ExamType)
        {
            // theory exam
            case 1:
                classScheduleToCreate.TheoryExamDate = theoryExamDate;
                classScheduleToCreate.Sessions.Add(theoryExamSession);
                break;
            // practice exam
            case 2:
                classScheduleToCreate.PracticalExamDate = practiceExamDate;
                classScheduleToCreate.Sessions.Add(practiceExamSession);
                break;
            // both theory and practice exam
            case 3:
                classScheduleToCreate.TheoryExamDate = theoryExamDate;
                classScheduleToCreate.PracticalExamDate = practiceExamDate;
                classScheduleToCreate.Sessions.Add(theoryExamSession);
                classScheduleToCreate.Sessions.Add(practiceExamSession);
                break;
            // no exam
            case 4:
                break;
        }

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

    private string? GetCodeIfStartDateNotTheNextOfLastSession(int classId, CreateClassScheduleRequest request)
    {
        // last session date
        var lastSession = _context.Sessions
            .Where(s => s.ClassSchedule.ClassId == classId)
            .OrderByDescending(s => s.LearningDate)
            .FirstOrDefault();

        if (lastSession == null) return null;

        return request.StartDate.Date <= lastSession.LearningDate.Date ? "E0089" : null;
    }

    private bool IsValidLearningDate(CreateClassScheduleRequest request, DateTime learningDate)
    {
        var isDayOff = _context.DaysOff.Any(dayOff =>
            dayOff.Date.Date == learningDate.Date &&
            (dayOff.TeacherId == null || dayOff.TeacherId == request.TeacherId));
        switch (request.ClassDaysId)
        {
            // monday, wednesday, friday
            case 1:
                if (isDayOff || learningDate.DayOfWeek is DayOfWeek.Tuesday
                        or DayOfWeek.Thursday or DayOfWeek.Saturday or DayOfWeek.Sunday)
                {
                    return false;
                }

                break;
            // tuesday, thursday, saturday
            case 2:
                if (isDayOff || learningDate.DayOfWeek is DayOfWeek.Monday
                        or DayOfWeek.Wednesday or DayOfWeek.Friday or DayOfWeek.Sunday)
                {
                    return false;
                }

                break;
        }

        return true;
    }

    private string? GetErrorCodeOccurWhenCreate(int classId, CreateClassScheduleRequest request)
    {
        var isModuleScheduledForThisClass =
            _context.ClassSchedules.Any(cs => cs.ClassId == classId && cs.ModuleId == request.ModuleId);

        var theoryRoom = _context.Rooms.Find(request.TheoryRoomId);
        var labRoom = _context.Rooms.Find(request.LabRoomId);

        var isDayOff =
            _context.DaysOff.Any(dayOff => dayOff.Date.Date == request.StartDate.Date && dayOff.TeacherId == null);
        var isTeacherDayOff =
            _context.DaysOff.Any(d => d.Date.Date == request.StartDate.Date && d.TeacherId == request.TeacherId);

        // Check if module is already scheduled for this class -> can't create new schedule
        if (isModuleScheduledForThisClass)
        {
            return "E0079";
        }

        if (request.Duration <= 0)
        {
            return "E0080";
        }

        if (request.StartDate.Date < DateTime.Now.Date)
        {
            return "E0081";
        }

        // sunday or day off  
        if (isDayOff || request.StartDate.DayOfWeek == DayOfWeek.Sunday)
        {
            return "E0085";
        }

        if (isTeacherDayOff)
        {
            return "E0086";
        }

        switch (request.ClassDaysId)
        {
            // check if request start date match to class days
            case 1 when request.StartDate.DayOfWeek != DayOfWeek.Monday &&
                        request.StartDate.DayOfWeek != DayOfWeek.Wednesday &&
                        request.StartDate.DayOfWeek != DayOfWeek.Friday:

            case 2 when request.StartDate.DayOfWeek != DayOfWeek.Tuesday &&
                        request.StartDate.DayOfWeek != DayOfWeek.Thursday &&
                        request.StartDate.DayOfWeek != DayOfWeek.Saturday:
                return "E0087";
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
        var isTeacherBusy = classSchedule.Sessions.Any(s =>
            s.ClassSchedule.TeacherId == request.TeacherId && s.LearningDate.Date == request.StartDate.Date
                                                           && (IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourStart)
                                                               || IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourEnd)));


        var isTheoryRoomBusy = classSchedule.Sessions.Any(s =>
            s.RoomId == request.TheoryRoomId && s.LearningDate.Date == request.StartDate.Date
                                             && (IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourStart)
                                             || IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourEnd)));
        
        var isLabRoomBusy = classSchedule.Sessions.Any(s =>
            s.RoomId == request.LabRoomId && s.LearningDate.Date == request.StartDate.Date
                                          && (IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourStart)
                                              || IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourEnd)));

        var isExamRoomBusy = classSchedule.Sessions.Any(s => s.SessionTypeId is PracticeExam or TheoryExam &&
                                                             s.RoomId == request.ExamRoomId &&
                                                             s.LearningDate.Date == request.StartDate.Date
                                                             && (IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourStart)
                                                                 || IsTimeInRange(s.StartTime, s.EndTime, request.ClassHourEnd)));
        
        if (isTeacherBusy)
        {
            return "E0093";
        }

        if (isTheoryRoomBusy)
        {
            return "E0090";
        }

        if (isLabRoomBusy)
        {
            return "E0091";
        }

        return isExamRoomBusy ? "E0092" : null;
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