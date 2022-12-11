using System.Security.Claims;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Controllers;
using AcademicManagementSystem.Services;
using AcademicManagementSystemTest.Helper;
using AcademicManagementSystemTest.MockData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace AcademicManagementSystemTest.System.Controller;

public class TestStudentController
{
    private readonly TestOutputHelper _testOutputHelper;
    private readonly AmsContext _context;
    private readonly StudentController _controller;
    
    public TestStudentController(ITestOutputHelper output)
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

        _controller = new StudentController(_context, userService);
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
        _context.TeacherTypes.AddRange(TeacherTypeMockData.TeacherTypes);
        _context.WorkingTimes.AddRange(WorkingTimeMockData.WorkingTimes);
        _context.Users.AddRange(UserMockData.Users);
        _context.Teachers.AddRange(TeacherMockData.Teachers);
        _context.Students.AddRange(StudentMockData.Students);
        _context.Classes.AddRange(ClassMockData.Classes);
        _context.StudentsClasses.AddRange(StudentClassMockData.StudentsClasses);
        _context.SaveChanges();
    }
}