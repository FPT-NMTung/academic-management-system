using System.Security.Claims;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Controllers;
using AcademicManagementSystem.Models.UserController.TeacherController;
using AcademicManagementSystem.Services;
using AcademicManagementSystemTest.Helper;
using AcademicManagementSystemTest.MockData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace AcademicManagementSystemTest.System.Controller;

/*
 * some test cases like duplicated data will compare with mock data  
 */
public class TestTeacherController
{
    private readonly AmsContext _context;
    private readonly TeacherController _controller;
    private readonly TestOutputHelper _testOutputHelper;

    public TestTeacherController(ITestOutputHelper output)
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

        _controller = new TeacherController(_context, userService);
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
        _context.SaveChanges();
    }

    [Fact]
    public void CreateTeacher_ValidRequest_ReturnOK()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540000",
            Email = "success-teacher@fpt.personal.edu.vn",
            EmailOrganization = "success-teacher@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974565",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591122584"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);
        
        // assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_FirstNameEmpty_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "  ",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_FirstNameContainSpecialCharacter_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy**",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_FirstNameContainNumber_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy 0",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_LastNameEmpty_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_LastNameContainSpecialCharacter_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen %",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_LastNameContainNumber_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen 8",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_BirthDayEqualToday_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = DateTime.Today,
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_BirthDayAfterToday_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = DateTime.Today.AddDays(2),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_MobilePhoneNumberExisted_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen",
            Avatar = null,
            MobilePhone = "0985563542", // belong to another user
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_MobilePhoneNumberNotStartWith0_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "9986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_MobilePhoneNumberLessThan10_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "09865403",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_MobilePhoneNumberMoreThan10_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "098654033848945",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_MobilePhoneNumberContainLetter_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986W40338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_EmailExisted_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.personal.edu.vn", // belong to another user
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_EmailOrganizationExisted_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.personal.edu.vn", // belong to another user
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_EmailNotContainDomain_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_EmailOrganizationNotContainDomain_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_EmailAndEmailOrganizationAreSame_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.same.edu.vn",
            EmailOrganization = "THANHNMHE141011@fpt.new.same.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_CitizenIdCardNoExisted_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456783", // belong to another user
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_CitizenIdCardLessThan9_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "15897",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_CitizenIdCardMoreThan9AndLessThan12_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "15897456234",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_CitizenIdCardMoreThan12_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562345612314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_CitizenIdCardPublishedDateEqualToday_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = DateTime.Today,
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_CitizenIdCardPublishedDateAfterToday_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(10),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_SalarySmallerThan0_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = -1,
            TaxCode = "2591126844"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_TaxCodeExisted_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "0000000001" // belong to another teacher
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_TaxCodeLessThan10_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "2591126"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateTeacher_TaxCodeMoreThan10_ReturnBadRequest()
    {
        // arrange
        var request = new CreateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            Avatar = null,
            MobilePhone = "0986540338",
            Email = "thanhnmhe141011@fpt.new.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.new.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "158974562314",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "25911265516982165"
        };

        // act 
        var result = _controller.CreateTeacher(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_ValidRequest_ReturnOK()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy ",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);
        
        // assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_FirstNameEmpty_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = " ",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_FirstNameContainSpecialCharacter_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "&& ",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_FirstNameContainNumber_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy 1",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_LastNameEmpty_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "         ",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_LastNameContainSpecialCharacter_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen *",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_LastNameContainNumber_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher 9",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_BirthDayEqualToday_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = DateTime.Today,
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_BirthDayAfterToday_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = DateTime.Today.AddDays(5),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_MobilePhoneNumberExisted_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563540",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_MobilePhoneNumberNotStartWith0_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "9855635421",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_MobilePhoneNumberLessThan10_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_MobilePhoneNumberMoreThan10_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "05649865985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_MobilePhoneNumberContainLetter_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "098-5563-542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_EmailExisted_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnm136@gmail.personal.com", // belong to another user
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_EmailOrganizationExisted_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnm136@gmail.personal.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_EmailNotContainDomain_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_EmailOrganizationNotContainDomain_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_EmailAndEmailOrganizationAreSame_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.org.edu.vn",
            EmailOrganization = "ThanhNMhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_CitizenIdCardNoExisted_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781", // belong to another user
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_CitizenIdCardLessThan9_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123452",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_CitizenIdCardMoreThan9AndLessThan12_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "1234567828",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_CitizenIdCardMoreThan12_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456751956182",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_CitizenIdCardPublishedDateEqualToday_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = DateTime.Today,
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_CitizenIdCardPublishedDateAfterToday_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(30),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_SalarySmallerThan0_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = -5862,
            TaxCode = "0000000001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_TaxCodeExisted_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0000000002"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_TaxCodeLessThan10_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "0001"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateTeacher_TaxCodeMoreThan10_ReturnBadRequest()
    {
        const int teacherId = 4;

        // arrange
        var request = new UpdateTeacherRequest()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            FirstName = "Tommy",
            LastName = "Nguyen Teacher",
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.personal.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.org.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 5000,
            TaxCode = "000000000156584"
        };

        // act 
        var result = _controller.UpdateTeacher(teacherId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}