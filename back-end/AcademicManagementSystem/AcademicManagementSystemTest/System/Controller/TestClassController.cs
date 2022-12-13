using System.Security.Claims;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Controllers;
using AcademicManagementSystem.Models.ClassController;
using AcademicManagementSystem.Services;
using AcademicManagementSystemTest.Helper;
using AcademicManagementSystemTest.MockData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace AcademicManagementSystemTest.System.Controller;

public class TestClassController
{
    private readonly TestOutputHelper _testOutputHelper;
    private readonly AmsContext _context;
    private readonly ClassController _controller;

    public TestClassController(ITestOutputHelper output)
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

        _controller = new ClassController(_context, userService);
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
        _context.Courses.AddRange(CourseMockData.Courses);
        _context.Users.AddRange(UserMockData.Users);
        _context.Students.AddRange(StudentMockData.Students);
        _context.Classes.AddRange(ClassMockData.Classes);
        _context.StudentsClasses.AddRange(StudentClassMockData.StudentsClasses);
        _context.SaveChanges();
    }

    [Fact]
    public void AddStudentManual_ValidRequest_ReturnOK()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0972222222",
            Email = "nguyenvana@gmail.com",
            EmailOrganization = "nguyenvana_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099922228272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            EnrollNumber = "HENguyenVanA",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố",
            ContactAddress = "Hà Nội",
            ParentalPhone = "0974222222",
            ApplicationDate = new DateTime(2021, 01, 01),
            ApplicationDocument = null,
            HighSchool = null,
            University = null,
            FacebookUrl = null,
            PortfolioUrl = null,
            WorkingCompany = null,
            CompanySalary = null,
            CompanyPosition = null,
            CompanyAddress = null,
            FeePlan = 5000,
            Promotion = 20, // %
        };

        // act 
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<OkObjectResult>(result);
    }
}