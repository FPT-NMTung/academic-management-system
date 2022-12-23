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
        _context.CourseFamilies.AddRange(CourseFamilyMockData.CourseFamilies);
        _context.Provinces.AddRange(ProvinceMockData.Provinces);
        _context.Districts.AddRange(DistrictMockData.Districts);
        _context.Wards.AddRange(WardMockData.Wards);
        _context.Roles.AddRange(RoleMockData.Roles);
        _context.Genders.AddRange(GenderMockData.Genders);
        _context.Centers.AddRange(CenterMockData.Centers);
        _context.Courses.AddRange(CourseMockData.Courses);
        _context.Sros.AddRange(SroMockData.Sros);
        _context.Users.AddRange(UserMockData.Users);
        _context.Students.AddRange(StudentMockData.Students);
        _context.Classes.AddRange(ClassMockData.Classes);
        _context.ClassDays.AddRange(ClassDayMockData.ClassDays);
        _context.ClassSchedules.AddRange(ClassScheduleMockData.ClassSchedules);
        _context.StudentsClasses.AddRange(StudentClassMockData.StudentsClasses);
        _context.SaveChanges();
    }

    [Fact]
    public void AddStudentManual_ClassIsNotExisted_ReturnBadRequest()
    {
        // arrange
        var classId = 1000;
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ClassIsNotExistedInCenter_ReturnBadRequest()
    {
        // arrange
        var classId = 4;
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_FirstNameIsEmpty_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_FirstNameIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_LastNameIsEmpty_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_LastNameIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_EmailIsEmpty_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_EmailIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_EmailOrganizationIsEmpty_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_EmailOrganizationIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_CourseCodeIsEmpty_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_CourseCodeIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ParentalNameIsEmpty_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ParentalNameIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ParentalRelationshipIsEmpty_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ParentalRelationshipIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ContactAddressIsEmpty_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ContactAddressIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_CitizenIdentityCardPublishedPlaceIsEmpty_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_CitizenIdentityCardPublishedPlaceIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_EnrollNumberIsEmpty_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_EnrollNumberIsOnlySpace_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "        ",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_CourseCodeIsNotExist_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = " HENguyenVanA1 ",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_CourseCodeIsNotActive_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = " HENguyenVanA1 ",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_EnrollNumberIsExisted_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
        {
            FirstName = "  Nguyen  ",
            LastName = "Văn A",
            MobilePhone = "0972244222",
            Email = "nguyenvana12@gmail.com",
            EmailOrganization = "nguyenvana12_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099944228272",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            EnrollNumber = "HE103",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_FirstNameIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = " HENguyenVanA1 ",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_LastNameIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = " HENguyenVanA1 ",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ParentalRelationshipIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = " HENguyenVanA1 ",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ParentalNameIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = " HENguyenVanA1 ",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ContactAddressIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = " HENguyenVanA1 ",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_HighSchoolIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = " HENguyenVanA1 ",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_UniversityIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = " HENguyenVanA1 ",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_WorkingCompanyIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = " HENguyenVanA1 ",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_CompanyPositionIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = " HENguyenVanA1 ",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_CompanyAddressIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = " HENguyenVanA1 ",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_MobilePhoneIsExisted_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_MobilePhoneContainsText_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_MobilePhoneHasMoreNumber_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_MobilePhoneHasLessNumber_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
        {
            FirstName = "Nguyễn",
            LastName = "Văn A",
            MobilePhone = "09732",
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ContactPhoneContainsText_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ContactPhoneHasLessNumber_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ContactPhoneHasMoreNumber_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_HomePhoneContainsText_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_HomePhoneHasMoreNumber_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_HomePhoneHasLessNumber_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ParentalPhoneContainsText_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ParentalPhoneHasMoreNumber_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ParentalPhoneHasLessNumber_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_EmailIsExisted_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_EmailOrganizationBelongToAnotherEmail_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_EmailOrganizationIsExisted_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_EmailBelongToAnotherEmailOrganization_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_EmailIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_EmailOrganizationIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_EmailSameAsEmailOrganization_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_IdentityCardIsExisted_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_IdentityCardPublishedPlaceIsNotMatchWithFormat_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_IdentityCardContainsText_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_IdentityCardHasMoreNumber_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_IdentityCardHasLessNumber_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ProvinceNotExists_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_ProvinceIdIsNegative_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_DistrictNotExists_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_DistrictIdIsNegative_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_WardNotExists_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_WardIdIsNegative_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_AddressNotExists_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_GenderIsNotExist_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_FeePlanIsNegative_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_PromotionIsNegative_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_CompanySalaryIsNegative_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        var result = _controller.AddStudentToClass(classId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_BirthdateIsLargerThanNow_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddStudentManual_IdentityCardPublishedDateIsLargerThanNow_ReturnBadRequest()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
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
            CitizenIdentityCardPublishedDate = new DateTime(2030, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            EnrollNumber = "HENguyenVanA1",
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
        Assert.IsType<BadRequestObjectResult>(result);
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

    [Fact]
    public void AddStudentManual_FirstNameRemoveUnUsedSpace_ReturnOk()
    {
        // arrange
        var classId = 100;
        var request = new AddStudentToClassRequest()
        {
            FirstName = "   Trần   ",
            LastName = "Văn A",
            MobilePhone = "0972222223",
            Email = "tranvana1@gmail.com",
            EmailOrganization = "tranvana2_organization@gmail.com",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "099922428273",
            CitizenIdentityCardPublishedDate = new DateTime(2010, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            EnrollNumber = "HETranVanA",
            CourseCode = "COURSE CODE 1",
            Status = 1,
            HomePhone = "0242222222",
            ContactPhone = "0972222222",
            ParentalName = "Nguyễn Văn Toàn",
            ParentalRelationship = "Bố",
            ContactAddress = "Hà Nội",
            ParentalPhone = "0974222224",
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

    [Fact]
    public void MergeClass_FirstClassNotExists_ReturnBadRequest()
    {
        // arrange
        var request = new MergeClassRequest2()
        {
            FirstClassId = 1000,
            SecondClassId = 2,
        };

        // act 
        var result = _controller.MergeClass2(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void MergeClass_SecondClassNotExists_ReturnBadRequest()
    {
        // arrange
        var request = new MergeClassRequest2()
        {
            FirstClassId = 100,
            SecondClassId = 1000,
        };

        // act 
        var result = _controller.MergeClass2(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetAllClass_ValidRequest_ReturnOK()
    {
        // act 
        var result = _controller.GetClassesByCurrentSroCenter();
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<OkObjectResult>(result);
    }
}