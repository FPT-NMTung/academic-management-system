using System.Security.Claims;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Controllers;
using AcademicManagementSystem.Models.UserController.StudentController;
using AcademicManagementSystem.Services;
using AcademicManagementSystemTest.Helper;
using AcademicManagementSystemTest.MockData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        _context.Courses.AddRange(CourseMockData.Courses);
        _context.CourseFamilies.AddRange(CourseFamilyMockData.CourseFamilies);
        _context.Users.AddRange(UserMockData.Users);
        _context.Students.AddRange(StudentMockData.Students);
        _context.Classes.AddRange(ClassMockData.Classes);
        _context.StudentsClasses.AddRange(StudentClassMockData.StudentsClasses);
        _context.SaveChanges();
    }

    [Fact]
    public void UpdateStudent_UserNotExists_ReturnNotFound()
    {
        // arrange
        var userId = 9999;
        var request = new UpdateStudentRequest()
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_FirstNameIsEmpty_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_FirstNameIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "    ",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_LastNameIsEmpty_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyễn  ",
            LastName = "",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_LastNameIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyễn  ",
            LastName = "     ",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_EmailIsEmpty_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyễn  ",
            LastName = "   Văn A  ",
            MobilePhone = "0972222222",
            Email = "",
            EmailOrganization = "nguyenvana_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099922228272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_EmailIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyễn  ",
            LastName = "   Văn A  ",
            MobilePhone = "0972222222",
            Email = "      ",
            EmailOrganization = "nguyenvana_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099922228272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_EmailOrganizationIsEmpty_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyễn  ",
            LastName = "   Văn A  ",
            MobilePhone = "0972222222",
            Email = "nguyenvana@gmail.com",
            EmailOrganization = "",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099922228272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_EmailOrganizationIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyễn  ",
            LastName = "   Văn A  ",
            MobilePhone = "0972222222",
            Email = "nguyenvana@gmail.com",
            EmailOrganization = "      ",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099922228272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_CourseCodeIsEmpty_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyễn  ",
            LastName = "Van A",
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
            CourseCode = "",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_CourseCodeIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyễn  ",
            LastName = "   Van A  ",
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
            CourseCode = "      ",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ParentalNameIsEmpty_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyen",
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
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ParentalNameIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "    ",
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
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "      ",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ParentalRelationshipIsEmpty_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyen",
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
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ParentalRelationshipIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = " Nguyen   ",
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
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "      ",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ContactAddressIsEmpty_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyen",
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
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố",
            ContactAddress = "",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ContactAddressIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyen  ",
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
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố",
            ContactAddress = "      ",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_CitizenIdentityCardPublishedPlaceIsEmpty_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyen",
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
            CitizenIdentityCardPublishedPlace = "",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_CitizenIdentityCardPublishedPlaceIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = " Nguyen   ",
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
            CitizenIdentityCardPublishedPlace = "          ",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_CourseCodeIsNotExist_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyen  ",
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
            CourseCode = "COURSE CODE 10000",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_CourseCodeIsNotActive_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyen  ",
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
            CourseCode = "COURSE CODE 555",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_FirstNameIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyen@,&^  ",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_LastNameIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyen  ",
            LastName = "Văn A@,&^#",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ParentalNameIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyen  ",
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
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn @,&^#",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ParentalRelationshipIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyen  ",
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
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố @,&^#",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ContactAddressIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyen  ",
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
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố",
            ContactAddress = "Hà Nội @,&^#",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_HighSchoolIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyen  ",
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
            HighSchool = " Truowng Tieu hoc @,&^#",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_UniversityIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyen  ",
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
            University = " Truowng Dai hoc @,&^#",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_WorkingCompanyIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyen  ",
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
            HighSchool = " Truowng Tieu hoc ABC",
            University = null,
            FacebookUrl = null,
            PortfolioUrl = null,
            WorkingCompany = " Cong ty ^*&#@",
            CompanySalary = null,
            CompanyPosition = null,
            CompanyAddress = null,
            FeePlan = 5000,
            Promotion = 20, // %
        };

        // act 
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_CompanyPositionIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyen  ",
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
            University = " Truong Dai hoc",
            FacebookUrl = null,
            PortfolioUrl = null,
            WorkingCompany = null,
            CompanySalary = null,
            CompanyPosition = "Truong phong ^*&#@",
            CompanyAddress = null,
            FeePlan = 5000,
            Promotion = 20, // %
        };

        // act 
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_CompanyAddressIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "  Nguyen  ",
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
            HighSchool = " Truowng Tieu hoc ABC",
            University = null,
            FacebookUrl = null,
            PortfolioUrl = null,
            WorkingCompany = " Cong ty ABC",
            CompanySalary = null,
            CompanyPosition = null,
            CompanyAddress = "HA NOI ^*&#@",
            FeePlan = 5000,
            Promotion = 20, // %
        };

        // act 
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_MobilePhoneIsExisted_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248896",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_MobilePhoneContainsText_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248896a",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_MobilePhoneHasMoreNumber_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248896473",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_MobilePhoneHasLessNumber_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ContactPhoneContainsText_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222b",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ContactPhoneHasMoreNumber_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "09722222224",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ContactPhoneHasLessNumber_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "09722222",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_HomePhoneContainsText_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222c",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_HomePhoneHasMoreNumber_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "024222222232",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_HomePhoneHasLessNumber_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "02422222",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ParentalPhoneContainsText_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố",
            ContactAddress = "Hà Nội",
            ParentalPhone = "0974222222q",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ParentalPhoneHasMoreNumber_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố",
            ContactAddress = "Hà Nội",
            ParentalPhone = "097422222322",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ParentalPhoneHasLessNumber_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố",
            ContactAddress = "Hà Nội",
            ParentalPhone = "0974222",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_EmailIsExisted_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenht_student@fpt.personal.edu.vn",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố",
            ContactAddress = "Hà Nội",
            ParentalPhone = "0974222432",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_EmailOrganizationBelongToAnotherEmail_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenht_student@fpt.personal.edu.vn",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố",
            ContactAddress = "Hà Nội",
            ParentalPhone = "0974222111",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_EmailOrganizationIsExisted_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenht_student@fpt.org.edu.vn",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố",
            ContactAddress = "Hà Nội",
            ParentalPhone = "0974222432",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_EmailBelongToAnotherEmailOrganization_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenht_student@fpt.org.edu.vn",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố",
            ContactAddress = "Hà Nội",
            ParentalPhone = "0974222111",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_EmailIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenht_student@fp",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố",
            ContactAddress = "Hà Nội",
            ParentalPhone = "0974222432",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_EmailOrganizationIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenht_student@fpt.o",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố",
            ContactAddress = "Hà Nội",
            ParentalPhone = "0974222432",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_EmailSameAsEmailOrganization_ReturnBadRequest()
    {
        // arrange
        var userId = 100;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0373625172",
            Email = "nguyenvana3_organization@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố",
            ContactAddress = "Hà Nội",
            ParentalPhone = "0974222432",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_IdentityCardIsExisted_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248236",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "022200004321",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_IdentityCardPublishedPlaceIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội$#@%%^",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_IdentityCardContainsText_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272av",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_IdentityCardHasMoreNumber_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "09996462827256456456",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_IdentityCardHasLessNumber_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "09996",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ProvinceNotExists_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 99,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_ProvinceIdIsNegative_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = -2,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_DistrictNotExists_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 99,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_DistrictIdIsNegative_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = -1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_WardNotExists_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 99,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_WardIdIsNegative_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = -1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_AddressNotExists_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 327,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_GenderIsNotExist_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 10,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_StatusIdIsLessThan1_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = -5,
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_StatusIdIsMoreThan7_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CourseCode = "COURSE CODE 1",
            Status = 8,
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_FeePlanIsNegative_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
            FeePlan = -5,
            Promotion = 20, // %
        };

        // act 
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_PromotionIsNegative_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
            Promotion = -5, // %
        };

        // act 
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_CompanySalaryIsNegative_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
            CompanySalary = -7,
            CompanyPosition = null,
            CompanyAddress = null,
            FeePlan = 5000,
            Promotion = 5, // %
        };

        // act 
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateStudent_BirthdateIsLargerThanNow_ReturnBadRequest()
    {
        // arrange
        var userId = 101;
        var request = new UpdateStudentRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "0973248436",
            Email = "nguyenvana3@gmail.com",
            EmailOrganization = "nguyenvana3_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2030, 01, 01),
            CitizenIdentityCardNo = "099964628272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
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
        var result = _controller.UpdateStudent(userId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}