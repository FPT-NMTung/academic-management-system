using System.Security.Claims;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Controllers;
using AcademicManagementSystem.Services;
using AcademicManagementSystemTest.MockData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace AcademicManagementSystemTest.System.Controller;

public class TestTestController
{
    private readonly TestController _controller;
    private readonly AmsContext _context;
    private readonly IUserService _userService;

    public TestTestController()
    {
        var options = new DbContextOptionsBuilder<AmsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _userService = new UserService(new HttpContextAccessor()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("uid", "1"),
                    new Claim("role", "student"),
                    new Claim("unique_string", RandomString())
                }))
            }
        });

        _context = new AmsContext(options);
        _controller = new TestController(_userService);
        Init();
    }

    private void Init()
    {
        _context.RoomTypes.AddRange(RoomTypeMockData.RoomTypes);
        _context.SaveChanges();
    }
    
    private static string RandomString()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 32)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }
    
    [Fact(DisplayName = "TestController__Auth__Valid")]
    public void TestControllerAuthValid()
    {
        var result = _controller.Auth();
        
        // check ok code
        Assert.IsType<OkObjectResult>(result as OkObjectResult);
    }
}