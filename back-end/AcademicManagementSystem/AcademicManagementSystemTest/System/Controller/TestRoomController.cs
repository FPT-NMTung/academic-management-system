using System.Security.Claims;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Controllers;
using AcademicManagementSystem.Models.RoomController.RoomModel;
using AcademicManagementSystem.Services;
using AcademicManagementSystemTest.Helper;
using AcademicManagementSystemTest.MockData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace AcademicManagementSystemTest.System.Controller;

public class TestRoomController
{
    private readonly TestOutputHelper _testOutputHelper;
    private readonly AmsContext _context;
    private readonly RoomController _controller;

    public TestRoomController(ITestOutputHelper output)
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
        
        _controller = new RoomController(_context, userService);
        Init();
    }

    private void Init()
    {
        _context.Centers.AddRangeAsync(CenterMockData.Centers);
        _context.RoomTypes.AddRangeAsync(RoomTypeMockData.RoomTypes);
        _context.Rooms.AddRangeAsync(RoomMockData.Rooms);
        _context.Users.AddRangeAsync(UserMockData.Users);

        _context.SaveChangesAsync();
    }
    
    [Fact]
    public void GetRoomsBySro_GetRoomsSuccess_ReturnOK()
    {
        // act
        var result = _controller.GetRoomsBySro();
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetRoomsByCenterId_CenterNotFound_ReturnBadRequest()
    {
        // arrange
        const int centerId = -1;

        // act
        var result = _controller.GetRoomsByCenterId(centerId);
        _testOutputHelper.PrintMessage(result);
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetRoomsByCenterId_CenterFound_ReturnOK()
    {
        // arrange
        const int centerId = 2;

        // act
        var result = _controller.GetRoomsByCenterId(centerId);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void CreateRoom_RoomNameInvalid_ReturnBadRequest()
    {
        // arrange
        var request = new CreateRoomRequest()
        {
            CenterId = 1,
            RoomTypeId = 1,
            Name = "Room @",
            Capacity = 30
        };

        // act
        var result = _controller.CreateRoom(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateRoom_RoomNameIsEmpty_ReturnBadRequest()
    {
        // arrange
        var request = new CreateRoomRequest()
        {
            CenterId = 1,
            RoomTypeId = 1,
            Name = "        ",
            Capacity = 30
        };

        // act
        var result = _controller.CreateRoom(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateRoom_CapacityOver100_ReturnBadRequest()
    {
        // arrange
        var request = new CreateRoomRequest()
        {
            CenterId = 1,
            RoomTypeId = 1,
            Name = "New Room",
            Capacity = 101
        };

        // act
        var result = _controller.CreateRoom(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateRoom_CapacityUnder20_ReturnBadRequest()
    {
        // arrange
        var request = new CreateRoomRequest()
        {
            CenterId = 1,
            RoomTypeId = 1,
            Name = "New Room",
            Capacity = 19
        };

        // act
        var result = _controller.CreateRoom(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateRoom_RoomExisted_ReturnBadRequest()
    {
        // arrange
        var request = new CreateRoomRequest()
        {
            CenterId = 2,
            RoomTypeId = 2,
            Name = "Room 2",
            Capacity = 99
        };

        // act
        var result = _controller.CreateRoom(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateRoom_ValidData_ReturnOk()
    {
        // arrange
        var request = new CreateRoomRequest()
        {
            CenterId = 1,
            RoomTypeId = 1,
            Name = "New Room OK",
            Capacity = 30
        };
        
        // act
        var result = _controller.CreateRoom(request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void UpdateRoom_RoomNotFound_ReturnBadRequest()
    {
        // arrange
        const int roomId = -1;

        var request = new UpdateRoomRequest()
        {
            RoomTypeId = 1,
            Name = "Room 1",
            Capacity = 30
        };

        // act
        var result = _controller.UpdateRoom(roomId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateRoom_RoomNameInvalid_ReturnBadRequest()
    {
        // arrange
        const int roomId = 1;

        var request = new UpdateRoomRequest()
        {
            RoomTypeId = 1,
            Name = " Room $",
            Capacity = 20
        };

        // act
        var result = _controller.UpdateRoom(roomId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }


    [Fact]
    public void UpdateRoom_RoomNameIsEmpty_ReturnBadRequest()
    {
        // arrange
        const int roomId = 1;

        var request = new UpdateRoomRequest()
        {
            RoomTypeId = 1,
            Name = " ",
            Capacity = 20
        };

        // act
        var result = _controller.UpdateRoom(roomId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateRoom_CapacityOver100_ReturnBadRequest()
    {
        // arrange
        const int roomId = 1;

        var request = new UpdateRoomRequest()
        {
            RoomTypeId = 1,
            Name = "Updated Room",
            Capacity = 101
        };

        // act
        var result = _controller.UpdateRoom(roomId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateRoom_CapacityUnder20_ReturnBadRequest()
    {
        // arrange
        const int roomId = 1;

        var request = new UpdateRoomRequest()
        {
            RoomTypeId = 1,
            Name = "Updated Room",
            Capacity = 19
        };

        // act
        var result = _controller.UpdateRoom(roomId, request);
        _testOutputHelper.PrintMessage(result);
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateRoom_RoomWithNameExistedInCenter_ReturnBadRequest()
    {
        // arrange
        const int roomId = 1;

        var request = new UpdateRoomRequest()
        {
            RoomTypeId = 1,
            Name = "Room 3",
            Capacity = 20
        };

        // act
        var result = _controller.UpdateRoom(roomId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateRoom_RoomUpdateValid_ReturnOK()
    {
        // arrange
        const int roomId = 1;

        var request = new UpdateRoomRequest()
        {
            RoomTypeId = 1,
            Name = "Updated Room1",
            Capacity = 99
        };

        // act
        var result = _controller.UpdateRoom(roomId, request);
        _testOutputHelper.PrintMessage(result);

        // assert
        Assert.IsType<OkObjectResult>(result);
    }
}