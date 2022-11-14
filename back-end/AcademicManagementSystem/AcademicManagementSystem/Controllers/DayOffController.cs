﻿using AcademicManagementSystem.Context;
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
        var daysOff = _context.DaysOff.Select(d => d.Date.Date).Distinct().ToList();
        return Ok(CustomResponse.Ok("Get days off successfully", daysOff));
    }

    [HttpPost]
    [Route("api/days-off/detail")]
    [Authorize(Roles = "sro")]
    public IActionResult GetDayOff([FromBody] GetDetailDayOffRequest request)
    {
        var selectDayOff = _context.DaysOff.Where(d => d.Date.Date == request.Date.Date).ToList();

        return Ok(CustomResponse.Ok("Get day off successfully", selectDayOff));
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

        if (request.WorkingTimeIds.Length == 0)
        {
            var error = ErrorDescription.Error["E2071"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.WorkingTimeIds.Any(time => time is > 3 or < 1))
        {
            var error = ErrorDescription.Error["E2071"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        foreach (var item in request.WorkingTimeIds)
        {
            var selectDayOffs = _context.DaysOff
                .Where(d => d.Date.Date == request.Date.Date && d.WorkingTimeId == item);

            if (!selectDayOffs.Any())
                continue;

            if (selectDayOffs.First().TeacherId == null)
            {
                // can not create day off because day off global is already exist
                var error = ErrorDescription.Error["E2073"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            if (selectDayOffs.First().TeacherId != null && request.TeacherId == null)
            {
                foreach (var selectDayOff in selectDayOffs)
                {
                    _context.DaysOff.Remove(selectDayOff);
                }

                continue;
            }

            if (selectDayOffs.First().TeacherId != null && request.TeacherId != null)
            {
                foreach (var selectDayOff in selectDayOffs)
                {
                    if (selectDayOff.TeacherId == request.TeacherId)
                    {
                        // can not create day off because day off of teacher is already exist
                        var error = ErrorDescription.Error["E2073"];
                        return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                    }
                }
            }
        }

        // get list schedule affected by day off
        var scheduleAffected = _context.Sessions
            .Include(s => s.ClassSchedule)
            .Where(s => s.LearningDate.Date == request.Date.Date)
            .Where(s => request.WorkingTimeIds.Contains(s.ClassSchedule.WorkingTimeId))
            .Select(s => s.ClassSchedule);

        if (request.TeacherId != null)
            scheduleAffected = scheduleAffected.Where(s => s.TeacherId == request.TeacherId);

        var listScheduleAffected = scheduleAffected.ToList();
        listScheduleAffected.ForEach(schedule => { UpdateSchedule(schedule, request); });

        foreach (var item in request.WorkingTimeIds)
        {
            _context.DaysOff.Add(new DayOff
            {
                Title = request.Title,
                Date = request.Date,
                TeacherId = request.TeacherId,
                WorkingTimeId = item
            });
        }

        _context.SaveChanges();

        return Ok(CustomResponse.Ok("Create day off successfully", null!));
    }

    [HttpDelete]
    [Route("api/days-off/{id}")]
    [Authorize(Roles = "sro")]
    public IActionResult DeleteDayOff(int id)
    {
        var dayOff = _context.DaysOff.FirstOrDefault(d => d.Id == id);
        if (dayOff == null)
        {
            return NotFound(CustomResponse.NotFound("Day off not found"));
        }

        _context.DaysOff.Remove(dayOff);

        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E2072"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Delete day off successfully", null!));
    }

    private void UpdateSchedule(ClassSchedule schedule, CreateDayOffRequest request)
    {
        var listSessionNeedChange = _context.Sessions
            .Where(s => s.ClassScheduleId == schedule.Id)
            .Where(s => s.LearningDate.Date >= request.Date.Date)
            .ToList();

        // update schedule: learning date of current session to next session
        for (var index = 0; index < listSessionNeedChange.Count; index++)
        {
            var selectedSession = listSessionNeedChange[index];
            var nextSession = index + 1 < listSessionNeedChange.Count ? listSessionNeedChange[index + 1] : null;

            if (nextSession == null)
            {
                var nextDay = GetNextLearningDateValid(selectedSession.LearningDate.Date, schedule);
                selectedSession.LearningDate = nextDay;

                var sessionDub = _context.Sessions
                    .Include(s => s.ClassSchedule)
                    .FirstOrDefault(s =>
                        s.ClassSchedule.ClassId == schedule.ClassId &&
                        s.LearningDate.Date == nextDay.Date);

                if (sessionDub != null)
                {
                    var nextSchedule = sessionDub.ClassSchedule;
                    UpdateSchedule(nextSchedule, request);
                }

                break;
            }

            selectedSession.LearningDate = nextSession.LearningDate;
        }
    }

    private DateTime GetNextLearningDateValid(DateTime learningDate, ClassSchedule schedule)
    {
        while (true)
        {
            learningDate = learningDate.AddDays(1);
            if (learningDate.DayOfWeek is DayOfWeek.Sunday)
                continue;

            switch (schedule.ClassDaysId)
            {
                case 1 when learningDate.DayOfWeek is DayOfWeek.Tuesday or DayOfWeek.Thursday or DayOfWeek.Saturday:
                case 2 when learningDate.DayOfWeek is DayOfWeek.Monday or DayOfWeek.Wednesday or DayOfWeek.Friday:
                    continue;
            }

            if (_context.DaysOff.Any(d =>
                    d.Date.Date == learningDate.Date && d.WorkingTimeId == schedule.WorkingTimeId))
                continue;

            return learningDate;
        }
    }

    private string? GetCodeIfExistErrorWhenCreate(CreateDayOffRequest request)
    {
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