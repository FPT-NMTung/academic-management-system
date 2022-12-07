using System.Security.Claims;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Controllers;
using AcademicManagementSystem.Models.ClassScheduleController.ClassScheduleModel;
using AcademicManagementSystem.Services;
using AcademicManagementSystemTest.MockData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace AcademicManagementSystemTest.System.Controller;

public class TestClassScheduleController
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly AmsContext _context;
    private readonly ClassScheduleController _controller;

    public TestClassScheduleController(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

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
        _context.StudentsClasses.AddRange(StudentClassMockData.StudentsClasses);
        _context.CourseFamilies.AddRange(CourseFamilyMockData.CourseFamilies);
        _context.Courses.AddRange(CourseMockData.Courses);
        _context.Modules.AddRange(ModuleMockData.Modules);
        _context.CoursesModulesSemesters.AddRange(CourseModuleSemesterMockData.CoursesModulesSemesters);
        _context.ClassSchedules.AddRange(ClassScheduleMockData.ClassSchedules);

        _context.SaveChanges();
    }

    [Fact]
    public void CreateClassSchedule_ClassNotFound_ReturnNotFound()
    {
        // var firstOrDefault = _context.Users.FirstOrDefault(u => u.Id == 3);
        // _testOutputHelper.WriteLine("user " + firstOrDefault.FirstName);
        //
        const int classId = -2;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 1,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = null,
            ExamRoomId = 1,
            Duration = 1,
            StartDate = new DateTime(2022, 01, 03),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request) as NotFoundObjectResult;
        if (result != null)
        {
            var value = result.Value as ResponseCustom;
            var message = value?.Message;
            _testOutputHelper.WriteLine(message);
        }

        // assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_ClassDontHasActiveStudent_ReturnBadRequest()
    {
        const int classId = 2;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 1,
            TeacherId = 4,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = null,
            ExamRoomId = 1,
            Duration = 1,
            StartDate = new DateTime(2022, 01, 03),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request) as BadRequestObjectResult;
        if (result != null)
        {
            var value = result.Value as ResponseCustomBadRequest;
            var message = value?.Message;
            var type = value?.TypeError;
            _testOutputHelper.WriteLine(message);
            _testOutputHelper.WriteLine(type);
        }

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
            LabRoomId = null,
            ExamRoomId = 1,
            Duration = 1,
            StartDate = new DateTime(2022, 01, 03),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request) as BadRequestObjectResult;
        if (result != null)
        {
            var value = result.Value as ResponseCustomBadRequest;
            var message = value?.Message;
            var type = value?.TypeError;
            _testOutputHelper.WriteLine(message);
            _testOutputHelper.WriteLine(type);
        }

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
            ModuleId = 1,
            TeacherId = 50561,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = null,
            ExamRoomId = 1,
            Duration = 1,
            StartDate = new DateTime(2022, 01, 03),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request) as BadRequestObjectResult;
        if (result != null)
        {
            var value = result.Value as ResponseCustomBadRequest;
            var message = value?.Message;
            var type = value?.TypeError;
            _testOutputHelper.WriteLine(message);
            _testOutputHelper.WriteLine(type);
        }

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateClassSchedule_TeacherNotActive_ReturnBadRequest()
    {
        // var user = _context.Users.First(u => u.Id == 4);
        // user.IsActive = false;
        
        const int classId = 1;
        // arrange
        var request = new CreateClassScheduleRequest()
        {
            ModuleId = 1,
            TeacherId = 5,
            ClassDaysId = 1,
            WorkingTimeId = 1,
            TheoryRoomId = 1,
            LabRoomId = null,
            ExamRoomId = 1,
            Duration = 1,
            StartDate = new DateTime(2022, 01, 03),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request) as BadRequestObjectResult;
        if (result != null)
        {
            var value = result.Value as ResponseCustomBadRequest;
            var message = value?.Message;
            var type = value?.TypeError;
            _testOutputHelper.WriteLine(message);
            _testOutputHelper.WriteLine(type);
        }

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
            LabRoomId = null,
            ExamRoomId = 1,
            Duration = 1,
            StartDate = new DateTime(2022, 01, 03),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request) as BadRequestObjectResult;
        if (result != null)
        {
            var value = result.Value as ResponseCustomBadRequest;
            var message = value?.Message;
            var type = value?.TypeError;
            _testOutputHelper.WriteLine(message);
            _testOutputHelper.WriteLine(type);
        }

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
            LabRoomId = null,
            ExamRoomId = 1,
            Duration = -1,
            StartDate = new DateTime(2022, 01, 03),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request) as BadRequestObjectResult;
        if (result != null)
        {
            var value = result.Value as ResponseCustomBadRequest;
            var message = value?.Message;
            var type = value?.TypeError;
            _testOutputHelper.WriteLine(message);
            _testOutputHelper.WriteLine(type);
        }

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
            LabRoomId = null,
            ExamRoomId = 1,
            Duration = 0,
            StartDate = new DateTime(2022, 01, 03),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request) as BadRequestObjectResult;
        if (result != null)
        {
            var value = result.Value as ResponseCustomBadRequest;
            var message = value?.Message;
            var type = value?.TypeError;
            _testOutputHelper.WriteLine(message);
            _testOutputHelper.WriteLine(type);
        }

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
            LabRoomId = null,
            ExamRoomId = 1,
            Duration = 0,
            StartDate = new DateTime(2022, 01, 03),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request) as BadRequestObjectResult;
        if (result != null)
        {
            var value = result.Value as ResponseCustomBadRequest;
            var message = value?.Message;
            var type = value?.TypeError;
            _testOutputHelper.WriteLine(message);
            _testOutputHelper.WriteLine(type);
        }

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
            LabRoomId = null,
            ExamRoomId = 1,
            Duration = 10,
            StartDate = DateTime.Today.AddDays(-1),
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request) as BadRequestObjectResult;
        if (result != null)
        {
            var value = result.Value as ResponseCustomBadRequest;
            var message = value?.Message;
            var type = value?.TypeError;
            _testOutputHelper.WriteLine(message);
            _testOutputHelper.WriteLine(type);
        }

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
            LabRoomId = null,
            ExamRoomId = 1,
            Duration = 10,
            StartDate = DateTime.Today,
            ClassHourStart = new TimeSpan(8, 0, 0),
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request) as BadRequestObjectResult;
        if (result != null)
        {
            var value = result.Value as ResponseCustomBadRequest;
            var message = value?.Message;
            var type = value?.TypeError;
            _testOutputHelper.WriteLine(message);
            _testOutputHelper.WriteLine(type);
        }

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    /*
     * case working time = 1 -> 8h - 12h
     */
    [Fact]
    public void CreateClassSchedule_StartTimeNotMatchWithWorkingTime_ReturnBadRequest()
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
            LabRoomId = null,
            ExamRoomId = 1,
            Duration = 10,
            StartDate = DateTime.Today.AddDays(1),
            ClassHourStart = new TimeSpan(7, 0, 0), // fail
            ClassHourEnd = new TimeSpan(12, 0, 0),
            Note = null
        };

        // act
        var result = _controller.CreateClassSchedule(classId, request) as BadRequestObjectResult;
        if (result != null)
        {
            var value = result.Value as ResponseCustomBadRequest;
            var message = value?.Message;
            var type = value?.TypeError;
            _testOutputHelper.WriteLine(message);
            _testOutputHelper.WriteLine(type);
        }

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}