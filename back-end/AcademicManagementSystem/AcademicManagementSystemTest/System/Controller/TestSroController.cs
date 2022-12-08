using AcademicManagementSystem.Context;
using AcademicManagementSystem.Controllers;
using AcademicManagementSystem.Models.UserController.SroController;
using AcademicManagementSystemTest.Helper;
using AcademicManagementSystemTest.MockData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using UpdateSroRequest = AcademicManagementSystem.Models.UserController.SroController.UpdateSroRequest;

namespace AcademicManagementSystemTest.System.Controller;

/*
 * some test cases like duplicated data will compare with mock data  
 */
public class TestSroController
{
    private readonly AmsContext _context;
    private readonly SroController _controller;
    private readonly TestOutputHelper _testOutputHelper;

    public TestSroController(ITestOutputHelper output)
    {
        _testOutputHelper = new TestOutputHelper(output);

        var optionsInMemoryDb = new DbContextOptionsBuilder<AmsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AmsContext(optionsInMemoryDb);

        _controller = new SroController(_context);
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

        _context.SaveChanges();
    }

    [Fact]
    public void CreateSRO_FirstNameEmpty_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "       ",
            LastName = "Nguyen",
            MobilePhone = "0987410258",
            Email = "tommy@email.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_FirstNameContainSpecialCharacter_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "T@mmy",
            LastName = "Nguyen",
            MobilePhone = "0987410258",
            Email = "tommy@email.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_FirstNameContainNumber_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy123",
            LastName = "Nguyen",
            MobilePhone = "0987410258",
            Email = "tommy@email.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_LastNameEmpty_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "",
            MobilePhone = "0987410258",
            Email = "tommy@email.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_LastNameContainSpecialCharacter_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen^",
            MobilePhone = "0987410258",
            Email = "tommy@email.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_LastNameContainNumber_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen69",
            MobilePhone = "0987410258",
            Email = "tommy@email.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_BirthDayEqualToday_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0987410258",
            Email = "tommy@email.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today,
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_BirthDayAfterToday_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0987410258",
            Email = "tommy@email.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(10),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_MobilePhoneNumberExisted_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0985563540",
            Email = "tommy@email.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_MobilePhoneNumberNotStartWith0_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "1234567890",
            Email = "tommy@email.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_MobilePhoneNumberLessThan10_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "012345678",
            Email = "tommy@email.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_MobilePhoneNumberMoreThan10_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0123456789856125",
            Email = "tommy@email.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_MobilePhoneNumberContainLetter_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "012345678a",
            Email = "tommy@email.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_EmailExisted_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0886547231",
            Email = "thanhnm_student@fpt.personal.edu.vn", // belong to another user
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_EmailOrganizationExisted_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0886547231",
            Email = "tommy@email.com",
            EmailOrganization = "thanhnmhe141011@fpt.personal.edu.vn", // belong to another user
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_EmailNotContainDomain_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0886547231",
            Email = "tommy@email",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_EmailOrganizationNotContainDomain_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0886547231",
            Email = "tommy@email.com",
            EmailOrganization = "tommy@email",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_EmailAndEmailOrganizationAreSame_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0886547231",
            Email = "tommy@email.org.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_CitizenIdCardNoExisted_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0886547231",
            Email = "tommy@email.personal.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "123456780",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_CitizenIdCardLessThan9_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0886547231",
            Email = "tommy@email.personal.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "00155",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_CitizenIdCardMoreThan9AndLessThan12_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0886547231",
            Email = "tommy@email.personal.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "12345678910",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_CitizenIdCardMoreThan12_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0886547231",
            Email = "tommy@email.personal.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "12345678910111213",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_CitizenIdCardPublishedDateEqualToday_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0886547231",
            Email = "tommy@email.personal.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today,
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_CitizenIdCardPublishedDateAfterToday_ReturnBadRequest()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0886547231",
            Email = "tommy@email.personal.com",
            EmailOrganization = "tommy@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-2),
            CitizenIdentityCardNo = "001122334455",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(9),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSRO_RequestValid_ReturnOK()
    {
        // arrange
        var request = new CreateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            MobilePhone = "0987458924",
            Email = "tommy-new@email.com",
            EmailOrganization = "tommy-new@email.org.com",
            Avatar = null,
            CenterId = 1,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            Birthday = DateTime.Today.AddDays(-1),
            CitizenIdentityCardNo = "001122334466",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(-1),
            CitizenIdentityCardPublishedPlace = "Ha Noi"
        };

        // act 
        var result = _controller.CreateSro(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_FirstNameEmpty_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "       ",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_FirstNameContainSpecialCharacter_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "T@mmy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_FirstNameContainNumber_ReturnBadRequest()
    {
        // arrange
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy123",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội"
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_LastNameEmpty_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_LastNameContainSpecialCharacter_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen~",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_LastNameContainNumber_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen58123",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_BirthDayEqualToday_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = DateTime.Today,
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_BirthDayAfterToday_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = DateTime.Today.AddDays(2),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_MobilePhoneNumberExisted_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563540", // belong to another user
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_MobilePhoneNumberNotStartWith0_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_MobilePhoneNumberLessThan10_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "09854682",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_MobilePhoneNumberMoreThan10_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "02131654562165",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_MobilePhoneNumberContainLetter_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "098556354a",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_EmailExisted_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnmhe141011@fpt.personal.edu.vn", // belong another user
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_EmailOrganizationExisted_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "nmthanh1306@gmail.personal.com", // belong to another user
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_EmailNotContainDomain_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_EmailOrganizationNotContainDomain_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_EmailAndEmailOrganizationAreSame_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.personal.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_CitizenIdCardNoExisted_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456782", // belong to another user
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_CitizenIdCardLessThan9_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_CitizenIdCardMoreThan9AndLessThan12_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "12345678910",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_CitizenIdCardMoreThan12_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "12345341326781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_CitizenIdCardPublishedDateEqualToday_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = DateTime.Today,
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_CitizenIdCardPublishedDateAfterToday_ReturnBadRequest()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = DateTime.Today.AddDays(99),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSRO_RequestValid_ReturnOK()
    {
        const int sroId = 3;

        // arrange
        var request = new UpdateSroRequest()
        {
            FirstName = "Tommy",
            LastName = "Nguyen",
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            GenderId = 1,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.personal.com",
            EmailOrganization = "thanhnm136@gmail.org.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "123456781",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
        };

        // act 
        var result = _controller.UpdateSro(sroId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<OkObjectResult>(result);
    }
}