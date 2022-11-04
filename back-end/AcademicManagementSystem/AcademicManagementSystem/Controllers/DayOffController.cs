using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.DayOffController.DayOffModel;
using AcademicManagementSystem.Models.TeacherSkillController;
using AcademicManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class DayOffController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AmsContext _context;
    private const int Theory = 1;
    private const int Practice = 2;
    private const int TheoryExam = 3;
    private const int PracticeExam = 4;

    public DayOffController(IUserService userService, AmsContext context)
    {
        _userService = userService;
        _context = context;
    }

    [HttpGet]
    [Route("api/days-off")]
    [Authorize(Roles = "sro")]
    public IActionResult GetDaysOff()
    {
        var daysOff = GetDayOffsQuery();
        return Ok(CustomResponse.Ok("Get days off successfully", daysOff));
    }

    [HttpGet]
    [Route("api/days-off/teachers/{id}")]
    [Authorize(Roles = "sro")]
    public IActionResult GetDaysOffByTeacherId(int id)
    {
        var centerId = _context.Sros.Include(sro => sro.User)
            .FirstOrDefault(sro => Convert.ToInt32(_userService.GetUserId()) == sro.UserId)?.User.CenterId;

        var isTeacherFound = _context.Teachers.Any(t => t.UserId == id && t.User.CenterId == centerId);
        if (!isTeacherFound)
        {
            return NotFound(CustomResponse.NotFound("Teacher not found"));
        }

        var daysOff = GetDayOffsQuery().Where(x => x.Teacher.Id == id || x.Teacher.Id == null);
        return Ok(CustomResponse.Ok("Get days off of teacher successfully", daysOff));
    }

    [HttpPost]
    [Route("api/days-off")]
    [Authorize(Roles = "sro")]
    public IActionResult CreateDayOff([FromBody] CreateDayOffRequest request)
    {
        var isTeacherFound = _context.Teachers.Any(t => t.UserId == request.TeacherId);
        if (!isTeacherFound && request.TeacherId != null)
        {
            return NotFound(CustomResponse.NotFound("Teacher not found"));
        }

        var errorCode = GetCodeIfExistErrorWhenCreate(request);
        if (errorCode != null)
        {
            var error = ErrorDescription.Error[errorCode];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var dayOff = new DayOff()
        {
            Title = request.Title,
            Date = request.Date,
            TeacherId = request.TeacherId,
            WorkingTimeId = request.WorkingTimeId
        };

        _context.DaysOff.Add(dayOff);
        try
        {
            _context.SaveChanges();
            HandlingSchedule(request);
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0102"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var response = GetDayOffsQuery().First(d => d.Id == dayOff.Id);

        return Ok(CustomResponse.Ok("Create day off successfully", response));
    }

    private void HandlingSchedule(CreateDayOffRequest request)
    {
        var schedule = _context.ClassSchedules
            .Include(c => c.Sessions).FirstOrDefault(s =>
                (request.TeacherId == s.TeacherId || request.TeacherId == null ) &&
                s.WorkingTimeId == request.WorkingTimeId);

        if (schedule == null) return;

        var existSession = schedule.Sessions.FirstOrDefault(s =>
            s.LearningDate.Date.Date == request.Date.Date && s.ClassSchedule.WorkingTimeId == request.WorkingTimeId);

        if (existSession != null)
        {
            var sessions = schedule.Sessions.Where(s => s.LearningDate.Date.Date >= existSession.LearningDate.Date)
                .ToList();

            // change date to the next day
            var i = 0;
            while (i < sessions.Count)
            {
                // last date or exam sessions
                if (i == sessions.Count - 1 || sessions[i].SessionTypeId is TheoryExam or PracticeExam)
                {
                    break;
                }

                var session = sessions[i];
                var nextSession = sessions[++i];
                session.LearningDate = nextSession.LearningDate;
            }

            // get list day off of teacher
            var dayOff = _context.DaysOff.Where(d => d.Date.Date >= request.Date.Date &&
                                                     (d.TeacherId == null || d.TeacherId == request.TeacherId));

            var teacherDayOff = dayOff.ToList();
            var globalDayOff = dayOff.Where(d => d.TeacherId == null).ToList();

            //module have both practice and theory exam
            var teExamSession = schedule.Sessions.FirstOrDefault(s => s.SessionTypeId == TheoryExam);
            var peExamSession = schedule.Sessions.FirstOrDefault(s => s.SessionTypeId == PracticeExam);

            if (teExamSession != null && peExamSession != null)
            {
                schedule.EndDate = sessions[i].LearningDate;
                // off last day while have both theory and practice exam(last day is practice exam)
                if (request.Date.Date == peExamSession.LearningDate.Date)
                {
                    var practiceExamDate = peExamSession.LearningDate.AddDays(1);
                    while (true)
                    {
                        if (IsValidLearningDate(schedule, practiceExamDate, globalDayOff, false))
                        {
                            peExamSession.LearningDate = practiceExamDate;
                            schedule.PracticalExamDate = practiceExamDate;
                            break;
                        }

                        practiceExamDate = practiceExamDate.AddDays(1);
                    }
                }
                else
                {
                    sessions[i].LearningDate = sessions[i + 1].LearningDate;
                    schedule.TheoryExamDate = sessions[i + 1].LearningDate;
                    var nextExamDate = sessions[i + 1].LearningDate.AddDays(1);
                    while (true)
                    {
                        if (IsValidLearningDate(schedule, nextExamDate, globalDayOff, false))
                        {
                            sessions[i + 1].LearningDate = nextExamDate;
                            schedule.PracticalExamDate = nextExamDate;
                            break;
                        }

                        nextExamDate = nextExamDate.AddDays(1);
                    }
                }
            }

            // module have only theory exam or practice exam
            if ((teExamSession != null && peExamSession == null) || (peExamSession != null && teExamSession == null))
            {
                schedule.EndDate = sessions[i].LearningDate;
                var learningDate = sessions[i].LearningDate.AddDays(1);

                while (true)
                {
                    if (IsValidLearningDate(schedule, learningDate, globalDayOff, false))
                    {
                        sessions[i].LearningDate = learningDate;
                        break;
                    }

                    learningDate = learningDate.AddDays(1);
                }

                if (teExamSession != null && peExamSession == null)
                {
                    teExamSession.LearningDate = learningDate;
                }

                if (peExamSession != null && teExamSession == null)
                {
                    peExamSession.LearningDate = learningDate;
                }
            }


            // module is session theory or practice
            if (sessions[i].SessionTypeId is Theory or Practice)
            {
                schedule.EndDate = sessions[i].LearningDate;
                var learningDate = sessions[i].LearningDate.AddDays(1);
                while (true)
                {
                    if (IsValidLearningDate(schedule, learningDate, teacherDayOff, true))
                    {
                        sessions[i].LearningDate = learningDate;
                        schedule.EndDate = learningDate;
                        break;
                    }

                    learningDate = learningDate.AddDays(1);
                }
            }

            _context.UpdateRange(sessions);
            _context.Update(schedule);
        }
    }

    private string? GetCodeIfExistErrorWhenCreate(CreateDayOffRequest request)
    {
        var isDayOffExist = _context.DaysOff.Any(d => d.Date.Date == request.Date.Date &&
                                                      d.WorkingTimeId == request.WorkingTimeId &&
                                                      d.TeacherId == request.TeacherId);
        if (isDayOffExist)
        {
            return "E0100";
        }

        if (request.Date < DateTime.Now)
        {
            return "E0101";
        }

        return null;
    }

    private bool IsValidLearningDate(ClassSchedule request, DateTime learningDate, List<DayOff> listDayOff,
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

        if (isDayOff != null) return false;
        // if (isTeacherDayOff && isDayOff != null)
        // {
        //     if (isDayOff.WorkingTimeId == 7)
        //     {
        //         return false;
        //     }
        //
        //     if (new List<int> { 1, 4, 5 }.Contains(isDayOff.WorkingTimeId) &&
        //         (IsTimeInRange(TimeSpan.FromHours(8), TimeSpan.FromHours(12), request.ClassHourStart)
        //          || IsTimeInRange(TimeSpan.FromHours(8), TimeSpan.FromHours(12), request.ClassHourEnd)))
        //     {
        //         return false;
        //     }
        //
        //     if (new List<int> { 2, 4, 6 }.Contains(isDayOff.WorkingTimeId) &&
        //         (IsTimeInRange(TimeSpan.FromHours(13), TimeSpan.FromHours(17), request.ClassHourStart)
        //          || IsTimeInRange(TimeSpan.FromHours(13), TimeSpan.FromHours(17), request.ClassHourEnd)))
        //     {
        //         return false;
        //     }
        //
        //     if (new List<int> { 3, 5, 6 }.Contains(isDayOff.WorkingTimeId) &&
        //         (IsTimeInRange(TimeSpan.FromHours(18), TimeSpan.FromHours(22), request.ClassHourStart)
        //          || IsTimeInRange(TimeSpan.FromHours(18), TimeSpan.FromHours(22), request.ClassHourEnd)))
        //     {
        //         return false;
        //     }
        // }
        //
        // if (!isTeacherDayOff && isDayOff != null)
        // {
        //     return false;
        // }

        return true;
    }

    private bool IsTimeInRange(TimeSpan startHour, TimeSpan endHour, TimeSpan timeToCheck)
    {
        return timeToCheck <= endHour && timeToCheck >= startHour;
    }

    private IQueryable<DayOffResponse> GetDayOffsQuery()
    {
        var daysOff = _context.DaysOff.Select(dayOff => new DayOffResponse
        {
            Id = dayOff.Id,
            Title = dayOff.Title,
            Date = dayOff.Date,
            WorkingTimeId = dayOff.WorkingTimeId,
            Teacher = new BasicTeacherInformationResponse()
            {
                Id = dayOff.TeacherId,
                FirstName = dayOff.Teacher.User.FirstName,
                LastName = dayOff.Teacher.User.LastName,
                EmailOrganization = dayOff.Teacher.User.EmailOrganization,
            }
        }).OrderBy(d => d.Date);
        return daysOff;
    }
}