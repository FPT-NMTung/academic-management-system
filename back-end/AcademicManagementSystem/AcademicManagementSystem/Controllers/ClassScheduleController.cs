using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.ClassDaysController;
using AcademicManagementSystem.Models.ClassScheduleController.ClassScheduleModel;
using AcademicManagementSystem.Models.ClassStatusController;
using AcademicManagementSystem.Models.RoomController.RoomModel;
using AcademicManagementSystem.Models.RoomController.RoomTypeModel;
using AcademicManagementSystem.Models.Sessions;
using AcademicManagementSystem.Models.TeacherSkillController;
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
    private const int StatusNotScheduled = 5;

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
    public IActionResult GetClassScheduleByClassIdAndModuleId(int classId, int moduleId)
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

        /*
        * Check input data like: module for this class or not, durations in range, startDate must < today,
        * LearningTime must match with workingTime(1,2,3), room type must correct, range of learningTime...
        */
        var errorCode = GetCodeIfErrorOccurWhenCreate(classId, request, centerId, module);
        if (errorCode != null)
        {
            var error = ErrorDescription.Error[errorCode];
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
            (d.TeacherId == null || d.TeacherId == request.TeacherId) && d.Date.Date >= request.StartDate.Date &&
            d.CenterId == centerId);

        var teacherDayOff = dayOff.ToList();
        var globalDayOff = dayOff.Where(d => d.TeacherId == null).ToList();

        // auto add session base on durations
        var i = 0;
        while (i < request.Duration)
        {
            if (IsValidLearningDate(request, learningDate, teacherDayOff, true))
            {
                if (i == 0)
                {
                    classScheduleToCreate.StartDate = learningDate;
                }

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
                else if (module.ModuleType == 3 && practiceSessions.All(practice => practice != i))
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

        var isRangeTimeInvalid = IsRangeTimeInvalid(classScheduleToCreate, false);
        if (isRangeTimeInvalid)
        {
            var error = ErrorDescription.Error["E2070"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var isTeacherBusy = CheckTeacherBusy(classScheduleToCreate, false);
        if (isTeacherBusy)
        {
            var error = ErrorDescription.Error["E2064"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var isRoomBusy = CheckRoomBusy(classScheduleToCreate, false);
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

    [HttpPut]
    [Route("api/classes-schedules/{scheduleId:int}")]
    [Authorize(Roles = "sro")]
    public IActionResult UpdateClassScheduleById(int scheduleId, [FromBody] UpdateClassScheduleRequest request)
    {
        var centerId = _context.Sros.Include(sro => sro.User)
            .FirstOrDefault(sro => sro.UserId == int.Parse(_userService.GetUserId()))?.User.CenterId;

        var classScheduleContext = _context.ClassSchedules
            .Include(cs => cs.Module)
            .ThenInclude(m => m.Center)
            .Include(cs => cs.Sessions)
            .FirstOrDefault(cs => cs.Id == scheduleId && cs.Module.Center.Id == centerId);
        if (classScheduleContext == null)
        {
            var error = ErrorDescription.Error["E0096"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check this schedule is for this center
        if (centerId != classScheduleContext.Module.Center.Id)
        {
            var error = ErrorDescription.Error["E0097"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var module = _context.Modules.First(m => m.Id == classScheduleContext.ModuleId);


        /*
        * Check input data like: durations in range, startDate must < today,
        * LearningTime must match with workingTime(1,2,3), room type must correct, range of learningTime...
        */
        var errorCode = GetCodeIfErrorOccurWhenUpdate(request, centerId, module);
        if (errorCode != null)
        {
            var error = ErrorDescription.Error[errorCode];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var oldSessions = classScheduleContext.Sessions.OrderBy(s => s.LearningDate).ToList();
        var updatedSessions = new List<Session>();
        var isStart = oldSessions.Any(s => s.LearningDate.Date <= DateTime.Now.Date);

        // check schedule is started or not
        if (isStart)
        {
            var error = ErrorDescription.Error["E0098"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // remove sessions 
        // foreach (var session in oldSessions)
        // {
        //     classScheduleContext.Sessions.Remove(session);
        // }
        //
        // _context.Sessions.RemoveRange(oldSessions);

        var practiceSessions = new List<int>();

        // remove duplicate practice sessions
        if (request.PracticeSession != null)
        {
            practiceSessions = request.PracticeSession.Distinct().ToList();
        }

        var learningDate = request.StartDate;
        var endDate = new DateTime();

        // get list day off of teacher
        var dayOff = _context.DaysOff.Where(d =>
            (d.TeacherId == null || d.TeacherId == request.TeacherId) && d.Date.Date >= request.StartDate.Date &&
            d.CenterId == centerId);
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
                else if (module.ModuleType == 3 && practiceSessions.All(practice => practice != i))
                {
                    session.SessionTypeId = Theory;
                    session.Title = module.ModuleName + " - T" + i;
                    session.RoomId = (int)request.TheoryRoomId!;
                }

                // add to new list
                updatedSessions.Add(session);
            }

            if (i == request.Duration)
                endDate = learningDate;

            learningDate += TimeSpan.FromDays(1);
        }

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
                    classScheduleContext.TheoryExamDate = learningDate;
                    learningDate += TimeSpan.FromDays(1);
                    // add to new list
                    updatedSessions.Add(session);
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
                    classScheduleContext.PracticalExamDate = learningDate;
                    // add to new list
                    updatedSessions.Add(session);
                    break;
                }

                learningDate += TimeSpan.FromDays(1);
            }
        }

        var oldStartDate = oldSessions.First().LearningDate.Date;
        
        /*
         * number of old sessions >= number of new sessions:
         * - update old sessions by new each session till new sessions end
         * - remove old sessions after new sessions end
         */
        if (oldSessions.Count >= updatedSessions.Count)
        {
            for (var j = 0; j < updatedSessions.Count; j++)
            {
                oldSessions[j].SessionTypeId = updatedSessions[j].SessionTypeId;
                oldSessions[j].Title = updatedSessions[j].Title;
                oldSessions[j].RoomId = updatedSessions[j].RoomId;
                oldSessions[j].LearningDate = updatedSessions[j].LearningDate;
                oldSessions[j].StartTime = updatedSessions[j].StartTime;
                oldSessions[j].EndTime = updatedSessions[j].EndTime;
            }

            if (oldSessions.Count > updatedSessions.Count)
            {
                // list of old sessions after new sessions end (redundant)
                var removeSessions = oldSessions.Skip(updatedSessions.Count).ToList();

                foreach (var session in removeSessions)
                {
                    oldSessions.Remove(session);
                    _context.Sessions.Remove(session);
                }
            }
        }

        /*
        * number of old sessions < number of new sessions:
        * - update old sessions by new each session till new sessions end
        * - add new sessions after old sessions end
        */
        if (oldSessions.Count < updatedSessions.Count)
        {
            for (var j = 0; j < oldSessions.Count; j++)
            {
                oldSessions[j].SessionTypeId = updatedSessions[j].SessionTypeId;
                oldSessions[j].Title = updatedSessions[j].Title;
                oldSessions[j].RoomId = updatedSessions[j].RoomId;
                oldSessions[j].LearningDate = updatedSessions[j].LearningDate;
                oldSessions[j].StartTime = updatedSessions[j].StartTime;
                oldSessions[j].EndTime = updatedSessions[j].EndTime;
            }

            for (var j = oldSessions.Count; j < updatedSessions.Count; j++)
            {
                oldSessions.Add(updatedSessions[j]);
            }
        }

        // re assign value for class schedule

        classScheduleContext.Sessions = oldSessions;

        classScheduleContext.TeacherId = request.TeacherId;
        classScheduleContext.ClassDaysId = request.ClassDaysId;
        classScheduleContext.WorkingTimeId = request.WorkingTimeId;
        classScheduleContext.TheoryRoomId = request.TheoryRoomId;
        classScheduleContext.LabRoomId = request.LabRoomId;
        classScheduleContext.ExamRoomId = request.ExamRoomId;
        classScheduleContext.StartDate = oldSessions.First().LearningDate.Date;
        classScheduleContext.EndDate = endDate;
        classScheduleContext.Note = request.Note;
        classScheduleContext.ClassHourStart = request.ClassHourStart;
        classScheduleContext.ClassHourEnd = request.ClassHourEnd;
        classScheduleContext.UpdatedAt = DateTime.Now;

        var isTeacherBusy = CheckTeacherBusy(classScheduleContext, true);
        if (isTeacherBusy)
        {
            var error = ErrorDescription.Error["E2064"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var isRoomBusy = CheckRoomBusy(classScheduleContext, true);
        if (isRoomBusy)
        {
            var error = ErrorDescription.Error["E2065"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var isHandled = CheckHandleSchedulesOfThisClass(classScheduleContext, oldStartDate);

        if (!isHandled)
        {
            var error = ErrorDescription.Error["E0099"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0078"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var response = GetClassSchedulesResponse(classScheduleContext.ClassId)
            .First(cs => cs.Id == classScheduleContext.Id);

        return Ok(CustomResponse.Ok("Update class schedule successfully", response));
    }

    private bool CheckHandleSchedulesOfThisClass(ClassSchedule classScheduleUpdated, DateTime oldStartDate)
    {
        // get all next schedule of requested schedule order by start date ascending for this class
        var schedules = _context.ClassSchedules
            .Include(cs => cs.Sessions)
            .Where(cs => cs.ClassId == classScheduleUpdated.ClassId && classScheduleUpdated.Id != cs.Id)
            .OrderBy(cs => cs.StartDate)
            .ToList();

        var sessionsUpdated = classScheduleUpdated.Sessions.OrderBy(cs => cs.LearningDate).ToList();

        var dayOff = _context.DaysOff.Where(d =>
                (d.TeacherId == null || d.TeacherId == classScheduleUpdated.TeacherId) &&
                d.Date.Date >= classScheduleUpdated.StartDate.Date &&
                d.CenterId == classScheduleUpdated.Module.CenterId)
            .ToList();
        var teacherDayOff = dayOff.ToList();
        var globalDayOff = dayOff.Where(d => d.TeacherId == null).ToList();
        var lastUpdatedLearningDate = sessionsUpdated.Last().LearningDate.Date;

        foreach (var schedule in schedules)
        {
            var sessions = schedule.Sessions.OrderBy(s => s.LearningDate).ToList();
            var firstSession = sessions.First(); // startDate
            var lastSession = sessions.Last();

            var isStarted = sessions.Any(s => s.LearningDate.Date <= DateTime.Today);

            /*
                this is previous schedule 
                also startDate of updated schedule is <= endDate of any previous schedule
                but schedule is started (done or learning) -> can't update
             */
            if (lastSession.LearningDate.Date < oldStartDate.Date &&
                classScheduleUpdated.StartDate.Date <= lastSession.LearningDate.Date && isStarted)
            {
                return false;
            }

            // skip update next schedule if last updated session smaller than first session of next schedule
            if (lastUpdatedLearningDate < firstSession.LearningDate.Date)
                continue;

            /*
                this is previous schedule but updated startDate bigger than last session of previous schedule (not started) 
                (previous unaffected)
                -> skip update this previous schedule
             */
            if (lastSession.LearningDate.Date < oldStartDate.Date &&
                classScheduleUpdated.StartDate.Date > lastSession.LearningDate.Date)
                continue;

            // for method call below to check valid learningDate base on this properties
            var fakeUpdateScheduleRequest = new UpdateClassScheduleRequest
            {
                ClassDaysId = schedule.ClassDaysId,
                ClassHourStart = schedule.ClassHourStart,
                ClassHourEnd = schedule.ClassHourEnd
            };

            var learningDate = sessionsUpdated.Last().LearningDate.Date + TimeSpan.FromDays(1);
            var i = 0;
            var isTeacherDayOff = true;
            var daysOff = teacherDayOff;

            // update learning date for each session of next schedule
            while (i < schedule.Sessions.Count)
            {
                // if session is theory exam or practice exam -> don't check teacher day off
                if (sessions[i].SessionTypeId is TheoryExam or PracticeExam)
                {
                    isTeacherDayOff = false;
                    daysOff = globalDayOff;
                }

                if (IsValidLearningDate(fakeUpdateScheduleRequest, learningDate, daysOff, isTeacherDayOff))
                {
                    if (i == 0)
                    {
                        schedule.StartDate = learningDate;
                    }

                    if (sessions[i].SessionTypeId is Theory or Practice)
                    {
                        schedule.EndDate = learningDate;
                    }

                    if (sessions[i].SessionTypeId is TheoryExam)
                    {
                        schedule.TheoryExamDate = learningDate;
                    }

                    if (sessions[i].SessionTypeId is PracticeExam)
                    {
                        schedule.PracticalExamDate = learningDate;
                    }

                    sessions[i].LearningDate = learningDate;
                    i++;
                }

                learningDate += TimeSpan.FromDays(1);
            }
            schedule.UpdatedAt = DateTime.Now;
            sessionsUpdated = schedule.Sessions.OrderBy(s => s.LearningDate).ToList();
            _context.Update(schedule);
            lastUpdatedLearningDate = sessionsUpdated.Last().LearningDate.Date;
        }

        return true;
    }

    [HttpDelete]
    [Route("api/classes/{classId:int}/schedules/{classScheduleId:int}")]
    [Authorize(Roles = "sro")]
    public IActionResult DeleteClassSchedule(int classId, int classScheduleId)
    {
        var classContext = _context.Classes.Include(c => c.ClassSchedules).FirstOrDefault(c => c.Id == classId);
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

            // update class status if all schedule is deleted
            classContext.ClassSchedules.Remove(classSchedule);
            if (!classContext.ClassSchedules.Any())
            {
                classContext.ClassStatusId = StatusNotScheduled;
                _context.SaveChanges();
            }
        }
        catch (Exception)
        {
            var error = ErrorDescription.Error["E2069"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Delete class schedule successfully", null!));
    }

    /*
    * Get all session that duplicate TEACHER in same time for this center
    */
    [HttpGet]
    [Route("api/classes-schedules/sessions-duplicate-teacher")]
    [Authorize(Roles = "sro")]
    public IActionResult GetSessionsDuplicateTeacherInSameTime()
    {
        var centerId = _context.Sros.Include(sro => sro.User)
            .FirstOrDefault(sro => sro.UserId == int.Parse(_userService.GetUserId()))?.User.CenterId;

        var sessions = _context.Sessions
            .Include(s => s.ClassSchedule.Class)
            .Include(s => s.ClassSchedule.Module)
            .Include(s => s.ClassSchedule.Teacher)
            .ThenInclude(t => t.User)
            .Where(s => s.ClassSchedule.Class.CenterId == centerId)
            .ToList();

        // group by learning date, working time, teacherId to get sessions that has duplicate teachers in that time
        var sessionsDuplicate = sessions
            .GroupBy(s => new { s.LearningDate.Date, s.ClassSchedule.WorkingTimeId, s.ClassSchedule.TeacherId })
            .Where(g => g.Count() > 1)
            .Select(g => new SessionDuplicateTeacherResponse()
            {
                LearningDate = g.Key.Date,
                WorkingTimeId = g.Key.WorkingTimeId,
                Teacher = new BasicTeacherInformationResponse()
                {
                    Id = g.Key.TeacherId,
                    FirstName = g.First().ClassSchedule.Teacher.User.FirstName,
                    LastName = g.First().ClassSchedule.Teacher.User.LastName,
                    EmailOrganization = g.First().ClassSchedule.Teacher.User.EmailOrganization
                },

                Sessions = g.Select(s => new BasicSessionDuplicateResponse()
                {
                    Id = s.Id,
                    Title = s.Title,
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
                }).ToList()
            })
            .OrderBy(s => s.LearningDate)
            .ToList();

        return Ok(CustomResponse.Ok("Get sessions duplicate teacher successfully", sessionsDuplicate));
    }

    /*
     * Get all session that duplicate ROOM in same time for this center
     */
    [HttpGet]
    [Route("api/classes-schedules/sessions-duplicate-room")]
    [Authorize(Roles = "sro")]
    public IActionResult GetSessionsDuplicateRoomInSameTime()
    {
        var centerId = _context.Sros.Include(sro => sro.User)
            .FirstOrDefault(sro => sro.UserId == int.Parse(_userService.GetUserId()))?.User.CenterId;

        var sessions = _context.Sessions
            .Include(s => s.ClassSchedule.Class)
            .Include(s => s.ClassSchedule.Module)
            .Include(s => s.Room)
            .Include(s => s.Room.RoomType)
            .Where(s => s.ClassSchedule.Class.CenterId == centerId)
            .ToList();

        // group by learning date, working time, roomId to get sessions that has duplicate rooms in that time
        var sessionsDuplicate = sessions
            .GroupBy(s => new { s.LearningDate.Date, s.ClassSchedule.WorkingTimeId, s.RoomId })
            .Where(g => g.Count() > 1)
            .Select(g => new SessionDuplicateRoomResponse()
            {
                LearningDate = g.Key.Date,
                WorkingTimeId = g.Key.WorkingTimeId,
                Room = new RoomResponse()
                {
                    Id = g.First().Room.Id,
                    Name = g.First().Room.Name,
                    Capacity = g.First().Room.Capacity,
                    Room = new RoomTypeResponse()
                    {
                        Id = g.First().Room.RoomType.Id,
                        Value = g.First().Room.RoomType.Value,
                    }
                },

                Sessions = g.Select(s => new BasicSessionDuplicateResponse()
                {
                    Id = s.Id,
                    Title = s.Title,
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
                }).ToList()
            })
            .OrderBy(s => s.LearningDate)
            .ToList();

        return Ok(CustomResponse.Ok("Get sessions duplicate teacher successfully", sessionsDuplicate));
    }

    private bool CheckTeacherBusy(ClassSchedule classScheduleToCreate, bool isUpdate)
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

        // if update, not check for current schedule
        if (isUpdate)
        {
            listSchedule = listSchedule.Where(cs =>
                cs.Id != classScheduleToCreate.Id && cs.ClassId != classScheduleToCreate.ClassId);
        }

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

    private bool CheckRoomBusy(ClassSchedule classScheduleToCreate, bool isUpdate)
    {
        var check = classScheduleToCreate.Sessions.ToList()
            .Find(s =>
                _context.Sessions.Where(s1 =>
                    s1.LearningDate.Date == s.LearningDate.Date &&
                    s1.RoomId == s.RoomId &&
                    s1.ClassSchedule.WorkingTimeId == classScheduleToCreate.WorkingTimeId
                ).ToList().Count > 0);

        // if update, not check for current schedule and this class
        if (isUpdate)
        {
            check = classScheduleToCreate.Sessions.ToList()
                .Find(s =>
                    _context.Sessions.Where(s1 =>
                        s1.ClassScheduleId != classScheduleToCreate.Id &&
                        s1.ClassSchedule.ClassId != classScheduleToCreate.ClassId &&
                        s1.LearningDate.Date == s.LearningDate.Date &&
                        s1.RoomId == s.RoomId &&
                        s1.ClassSchedule.WorkingTimeId == classScheduleToCreate.WorkingTimeId
                    ).ToList().Count > 0);
        }

        if (check != null) return true;

        return false;
    }

    private bool IsRangeTimeInvalid(ClassSchedule classScheduleToCreate, bool isUpdate)
    {
        var sessions = classScheduleToCreate.Sessions.OrderBy(s => s.LearningDate).ToList();
        var firstSession = sessions.First();
        var lastSession = sessions.Last();

        var listSchedule = _context.ClassSchedules
            .Include(cs => cs.Sessions)
            .Where(cs => cs.ClassId == classScheduleToCreate.ClassId)
            .ToList();

        if (isUpdate)
        {
            listSchedule = listSchedule.Where(cs => cs.Id != classScheduleToCreate.Id).ToList();
        }

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

    // for create
    private bool IsValidLearningDate(CreateClassScheduleRequest request, DateTime learningDate,
        List<DayOff> listDayOff, bool isTeacherDayOff)
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
            if (isDayOff.WorkingTimeId == 1 &&
                (IsTimeInRange(TimeSpan.FromHours(8), TimeSpan.FromHours(12), request.ClassHourStart)
                 || IsTimeInRange(TimeSpan.FromHours(8), TimeSpan.FromHours(12), request.ClassHourEnd)))
            {
                return false;
            }

            if (isDayOff.WorkingTimeId == 2 &&
                (IsTimeInRange(TimeSpan.FromHours(13), TimeSpan.FromHours(17), request.ClassHourStart)
                 || IsTimeInRange(TimeSpan.FromHours(13), TimeSpan.FromHours(17), request.ClassHourEnd)))
            {
                return false;
            }

            if (isDayOff.WorkingTimeId == 3 &&
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

    // for update
    private bool IsValidLearningDate(UpdateClassScheduleRequest request, DateTime learningDate,
        List<DayOff> listDayOff, bool isTeacherDayOff)
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
            if (isDayOff.WorkingTimeId == 1 &&
                (IsTimeInRange(TimeSpan.FromHours(8), TimeSpan.FromHours(12), request.ClassHourStart)
                 || IsTimeInRange(TimeSpan.FromHours(8), TimeSpan.FromHours(12), request.ClassHourEnd)))
            {
                return false;
            }

            if (isDayOff.WorkingTimeId == 2 &&
                (IsTimeInRange(TimeSpan.FromHours(13), TimeSpan.FromHours(17), request.ClassHourStart)
                 || IsTimeInRange(TimeSpan.FromHours(13), TimeSpan.FromHours(17), request.ClassHourEnd)))
            {
                return false;
            }

            if (isDayOff.WorkingTimeId == 3 &&
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

    private string? GetCodeIfErrorOccurWhenCreate(int classId, CreateClassScheduleRequest request, int? centerId,
        Module module)
    {
        var isModuleScheduledForThisClass =
            _context.ClassSchedules.Any(cs => cs.ClassId == classId && cs.ModuleId == request.ModuleId);

        var theoryRoom = _context.Rooms.FirstOrDefault(r => request.TheoryRoomId == r.Id && r.CenterId == centerId);
        var labRoom = _context.Rooms.FirstOrDefault(r => request.LabRoomId == r.Id && r.CenterId == centerId);
        var examRoom = _context.Rooms.FirstOrDefault(r => request.ExamRoomId == r.Id && r.CenterId == centerId);

        // Check if module is already scheduled for this class -> can't create new schedule
        if (isModuleScheduledForThisClass)
            return "E0079";

        if (request.Duration is <= 0 or > 50)
            return "E0080";

        if (request.StartDate.Date <= DateTime.Now.Date)
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

        if (theoryRoom == null && new List<int> { 1, 3 }.Contains(module.ModuleType))
        {
            return "E0084_1";
        }

        if (labRoom == null && new List<int> { 2, 3 }.Contains(module.ModuleType))
        {
            return "E0084_1";
        }

        if (examRoom == null && module.ExamType != 4)
        {
            return "E0084_1";
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

    private string? GetCodeIfErrorOccurWhenUpdate(UpdateClassScheduleRequest request, int? centerId, Module module)
    {
        var theoryRoom = _context.Rooms.FirstOrDefault(r => request.TheoryRoomId == r.Id && r.CenterId == centerId);
        var labRoom = _context.Rooms.FirstOrDefault(r => request.LabRoomId == r.Id && r.CenterId == centerId);
        var examRoom = _context.Rooms.FirstOrDefault(r => request.ExamRoomId == r.Id && r.CenterId == centerId);

        if (request.Duration is <= 0 or > 50)
            return "E0080";

        if (request.StartDate.Date <= DateTime.Now.Date)
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

        if (theoryRoom == null && new List<int> { 1, 3 }.Contains(module.ModuleType))
        {
            return "E0084_1";
        }

        if (labRoom == null && new List<int> { 2, 3 }.Contains(module.ModuleType))
        {
            return "E0084_1";
        }

        if (examRoom == null && module.ExamType != 4)
        {
            return "E0084_1";
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