using System.Security.Claims;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Controllers;
using AcademicManagementSystem.Models.ClassScheduleController.ClassScheduleModel;
using AcademicManagementSystem.Services;
using AcademicManagementSystemTest.Helper;
using AcademicManagementSystemTest.MockData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace AcademicManagementSystemTest.System.Controller;

public class TestClassScheduleController
{
    private readonly TestOutputHelper _testOutputHelper;
    private readonly AmsContext _context;
    private readonly ClassScheduleController _controller;

    public TestClassScheduleController(ITestOutputHelper output)
    {
        _testOutputHelper = new TestOutputHelper(output);

        var optionsInMemoryDb = new DbContextOptionsBuilder<AmsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AmsContext(optionsInMemoryDb);

        IUserService userService = new UserService(new HttpContextAccessor()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("uid", "3"),
                    new Claim("role", "sro"),
                }))
            }
        });

        _controller = new ClassScheduleController(_context, userService);
        Init();
    }

    private void Init()
    {
        _context.ClassStatuses.AddRange(ClassStatusMockData.ClassStatuses);
        _context.ClassDays.AddRange(ClassDayMockData.ClassDays);
        _context.Provinces.AddRange(ProvinceMockData.Provinces);
        _context.Districts.AddRange(DistrictMockData.Districts);
        _context.Wards.AddRange(WardMockData.Wards);
        _context.Roles.AddRange(RoleMockData.Roles);
        _context.Genders.AddRange(GenderMockData.Genders);
        _context.Centers.AddRange(CenterMockData.Centers);
        _context.Users.AddRange(UserMockData.Users);
        _context.Sros.AddRange(SroMockData.Sros);
        _context.Teachers.AddRange(TeacherMockData.Teachers);
        _context.Students.AddRange(StudentMockData.Students);
        _context.Classes.AddRange(ClassMockData.Classes);
        _context.Rooms.AddRange(RoomMockData.Rooms);
        _context.StudentsClasses.AddRange(StudentClassMockData.StudentsClasses);
        _context.CourseFamilies.AddRange(CourseFamilyMockData.CourseFamilies);
        _context.Courses.AddRange(CourseMockData.Courses);
        _context.Modules.AddRange(ModuleMockData.Modules);
        _context.CoursesModulesSemesters.AddRange(CourseModuleSemesterMockData.CoursesModulesSemesters);
        _context.ClassSchedules.AddRange(ClassScheduleMockData.ClassSchedules);
        _context.Sessions.AddRange(SessionMockData.Sessions);

        _context.SaveChanges();
    }


    [Fact]
    public void CreateClassSchedule_ValidRequest_ReturnOK()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 3,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddYears(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_ClassNotFound_ReturnNotFound()
    {
        const int classId = -2;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_ClassDontHasActiveStudent_ReturnBadRequest()
    {
        const int classId = 3;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_ModuleNotForThisClass_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = -2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_TeacherNotFoundInThisCenter_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 50561,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_TeacherNotActive_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 5,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_ModuleAlreadyScheduledForClass_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 1,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_DurationSmallerThan0_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = -1,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_DurationEqual0_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 0,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_DurationBiggerThan50_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 51,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_StartDateSmallerThanToday_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddDays(-1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_StartDateEqualToday_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today,
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

/*
 * case working time = 1 -> learning time must 8h - 12h
 */
    [Fact]
    public void CreateClassSchedule_StartTimeNotMatchWithWorkingTimeMorning_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(7, 0, 0), // fail
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

/*
* case working time = 1 -> learning time must 08h - 12h
*/
    [Fact]
    public void CreateClassSchedule_EndTimeNotMatchWithWorkingTimeMorning_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(13, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

/*
* case working time = 2 -> learning time must 13h - 17h
*/
    [Fact]
    public void CreateClassSchedule_StartTimeNotMatchWithWorkingTimeAfternoon_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 2,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(12, 0, 0),
            ClassHourEnd = new TimeSpan(15, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

/*
* case working time = 2 -> learning time must 13h - 17h
*/
    [Fact]
    public void CreateClassSchedule_EndTimeNotMatchWithWorkingTimeAfternoon_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 2,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(15, 0, 0),
            ClassHourEnd = new TimeSpan(19, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

/*
* case working time = 3 -> learning time must 18h - 22h
*/
    [Fact]
    public void CreateClassSchedule_StartTimeNotMatchWithWorkingTimeEvening_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 3,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(17, 0, 0),
            ClassHourEnd = new TimeSpan(21, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

/*
* case working time = 3 -> learning time must 18h - 22h
*/
    [Fact]
    public void CreateClassSchedule_EndTimeNotMatchWithWorkingTimeEvening_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 3,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(19, 0, 0),
            ClassHourEnd = new TimeSpan(22, 30, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_PracticeSessionNumber0NotInSessionNumber_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 0, 1, 2, 3, 4 }, // don't have session 0
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_PracticeSessionNumber6NotInSessionNumber_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 3, 6, 5 }, // don't have session 6
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_NotTheoryRoom_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 3, // not theory room
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 3, 5 },
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 00, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_NotLabRoom_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 1, // not lab room
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 3, 5 },
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 00, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_ClassAlreadyHaveModuleLearningThisTime_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 3, 5 },
            StartDate = DateTime.Today.AddYears(1), // class already have module learning this time
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 00, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_TeacherBusyInAnotherClassAtThisTime_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 9,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 3, 5, 1 },
            StartDate = DateTime.Today.AddYears(3).AddDays(1), // teacher busy in another class at this time
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 00, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_RoomBusyInAnotherClassAtThisTime_ReturnBadRequest()
    {
        const int classId = 2;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 9,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 3, 5, 1 },
            StartDate = DateTime.Today.AddYears(1), // room busy in another class at this time (case create success) 
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 00, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateClassSchedule_ValidRequest_ReturnOK()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(2).AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public void UpdateClassSchedule_ScheduleNotFound_ReturnBadRequest()
    {
        const int classScheduleId = -5;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(2).AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public void UpdateClassSchedule_TeacherNotInThisCenter_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        
        // arrange

        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 11,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(3).AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public void UpdateClassSchedule_TeacherNotActive_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        
        // arrange

        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 5,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(3).AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public void UpdateClassSchedule_DurationSmallerThan0_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = -5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(2).AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateClassSchedule_DurationEqual0_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 0,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(2).AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateClassSchedule_DurationBiggerThan50_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 51,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(2).AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateClassSchedule_StartDateSmallerThanToday_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddDays(-1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateClassSchedule_StartDateEqualToday_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today,
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

/*
 * case working time = 1 -> learning time must 8h - 12h
 */
    [Fact]
    public void UpdateClassSchedule_StartTimeNotMatchWithWorkingTimeMorning_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(2).AddDays(1),
            ClassHourStart = new TimeSpan(7, 0, 0), // fail
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

/*
* case working time = 1 -> learning time must 08h - 12h
*/
    [Fact]
    public void UpdateClassSchedule_EndTimeNotMatchWithWorkingTimeMorning_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(2).AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(13, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

/*
* case working time = 2 -> learning time must 13h - 17h
*/
    [Fact]
    public void UpdateClassSchedule_StartTimeNotMatchWithWorkingTimeAfternoon_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(2).AddDays(1),
            ClassHourStart = new TimeSpan(12, 0, 0),
            ClassHourEnd = new TimeSpan(15, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

/*
* case working time = 2 -> learning time must 13h - 17h
*/
    [Fact]
    public void UpdateClassSchedule_EndTimeNotMatchWithWorkingTimeAfternoon_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(2).AddDays(1),
            ClassHourStart = new TimeSpan(15, 0, 0),
            ClassHourEnd = new TimeSpan(19, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

/*
* case working time = 3 -> learning time must 18h - 22h
*/
    [Fact]
    public void UpdateClassSchedule_StartTimeNotMatchWithWorkingTimeEvening_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(2).AddDays(1),
            ClassHourStart = new TimeSpan(17, 0, 0),
            ClassHourEnd = new TimeSpan(21, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

/*
* case working time = 3 -> learning time must 18h - 22h
*/
    [Fact]
    public void UpdateClassSchedule_EndTimeNotMatchWithWorkingTimeEvening_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(2).AddDays(1),
            ClassHourStart = new TimeSpan(19, 0, 0),
            ClassHourEnd = new TimeSpan(22, 30, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateClassSchedule_PracticeSessionNumber0NotInSessionNumber_ReturnBadRequest()
    {
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 2,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 0, 1, 2 }, // don't have session 0
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateClassSchedule_PracticeSessionNumber65NotInSessionNumber_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 65 },
            StartDate = DateTime.Today.AddYears(2).AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateClassSchedule_NotTheoryRoom_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 3,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(2).AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateClassSchedule_NotLabRoom_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 1,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(2).AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public void UpdateClassSchedule_ModuleIsStartedLearning_ReturnBadRequest()
    {
        const int classScheduleId = 1;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(2).AddDays(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public void UpdateClassSchedule_TeacherBusyInAnotherClassAtThisTime_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateClassSchedule_RoomBusyInAnotherClassAtThisTime_ReturnBadRequest()
    {
        const int classScheduleId = 2;
        // arrange
        var request = new UpdateClassScheduleRequest()
        {
            TeacherId = 9,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = 3,
            ExamRoomId = 1,
            Duration = 5,
            PracticeSession = new List<int>() { 4, 5 },
            StartDate = DateTime.Today.AddYears(1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.UpdateClassScheduleById(classScheduleId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}