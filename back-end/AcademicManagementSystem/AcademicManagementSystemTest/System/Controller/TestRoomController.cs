using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Controllers;
using AcademicManagementSystem.Models.RoomController.RoomModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AcademicManagementSystemTest.System.Controller;

public class TestRoomController
{
    private readonly RoomController _controllerServer;

    private readonly AmsContext _contextInMemoryDb;
    private readonly RoomController _controllerInMemoryDb;

    public TestRoomController()
    {
        var builder = WebApplication.CreateBuilder();

        var connectionString = builder.Configuration.GetConnectionString("AmsConnection");

        var options = new DbContextOptionsBuilder<AmsContext>()
            .UseSqlServer(connectionString)
            .Options;

        var optionsInMemoryDb = new DbContextOptionsBuilder<AmsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var contextServer = new AmsContext(options);
        _controllerServer = new RoomController(contextServer);

        _contextInMemoryDb = new AmsContext(optionsInMemoryDb);
        _controllerInMemoryDb = new RoomController(_contextInMemoryDb);

        Init();
    }

    private void Init()
    {
        _contextInMemoryDb.Rooms.Add(new Room()
        {
            CenterId = 1,
            RoomTypeId = 1,
            Name = "Room 1",
            Capacity = 20
        });

        _contextInMemoryDb.Rooms.Add(new Room()
        {
            CenterId = 2,
            RoomTypeId = 2,
            Name = "Room 2",
            Capacity = 30
        });
        
        _contextInMemoryDb.Rooms.Add(new Room()
        {
            CenterId = 1,
            RoomTypeId = 1,
            Name = "Room 3",
            Capacity = 20
        });

        _contextInMemoryDb.SaveChanges();
    }

    [Fact(DisplayName = "Return Bad Request Center Not Found")]
    public void ReturnBadRequestCenterNotFound()
    {
        // arrange
        const int centerId = -1;

        // act
        var result = _controllerServer.GetRoomsByCenterId(centerId);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact(DisplayName = "Return OK Find Rooms By Center")]
    public void ReturnOkResult_FindByCenterId()
    {
        // arrange
        const int centerId = 2;

        // act
        var result = _controllerServer.GetRoomsByCenterId(centerId);

        // assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact(DisplayName = "Add Invalid Format RoomName Return Bad Request")]
    public void AddInvalidData_RoomName_ReturnBadRequest()
    {
        // arrange
        var request = new CreateRoomRequest()
        {
            CenterId = 1,
            RoomTypeId = 1,
            Name = "_Room 1",
            Capacity = 30
        };

        // act
        var result = _controllerInMemoryDb.CreateRoom(request);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact(DisplayName = "Add Invalid RoomName Empty Return Bad Request")]
    public void AddInvalidData_RoomName_Empty_ReturnBadRequest()
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
        var result = _controllerInMemoryDb.CreateRoom(request);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact(DisplayName = "Add Invalid RoomCapacity OutRange Return Bad Request")]
    public void AddInvalidData_RoomCapacity_ReturnBadRequest()
    {
        // arrange
        var request = new CreateRoomRequest()
        {
            CenterId = 1,
            RoomTypeId = 1,
            Name = "Room 1",
            Capacity = 101
        };

        // act
        var result = _controllerInMemoryDb.CreateRoom(request);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact(DisplayName = "Add Valid Data But Existed Room Return Bad Request")]
    public void AddValidDataButExistedRoom_ReturnBadRequest()
    {
        // arrange
        var request = new CreateRoomRequest()
        {
            CenterId = 1,
            RoomTypeId = 1,
            Name = "Room 1",
            Capacity = 30
        };

        // act
        var result = _controllerInMemoryDb.CreateRoom(request);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact(DisplayName = "Add Valid Data Return Ok Object Created")]
    public void AddValidData_ReturnOk()
    {
        // arrange
        var request = new CreateRoomRequest()
        {
            CenterId = 1,
            RoomTypeId = 1,
            Name = "New Room",
            Capacity = 30
        };
        // act
        var result = _controllerInMemoryDb.CreateRoom(request);

        // assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact(DisplayName = "Update Not Found Room Return Bad Request")]
    public void UpdateNotFoundRoom_ReturnBadRequest()
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
        var result = _controllerInMemoryDb.UpdateRoom(roomId, request);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact(DisplayName = "Update Invalid Data RoomName Empty Return Bad Request")]
    public void UpdateInvalidData_RoomName_Empty_ReturnBadRequest()
    {
        // arrange
        const int roomId = 1;

        var request = new UpdateRoomRequest()
        {
            RoomTypeId = 1,
            Name = "",
            Capacity = 20
        };

        // act
        var result = _controllerInMemoryDb.UpdateRoom(roomId, request);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact(DisplayName = "Update Invalid RoomCapacity OutRange Return Bad Request")]
    public void UpdateInvalidData_RoomCapacity_ReturnBadRequest()
    {
        // arrange
        const int roomId = 1;

        var request = new UpdateRoomRequest()
        {
            RoomTypeId = 1,
            Name = "Updated Room",
            Capacity = -1
        };

        // act
        var result = _controllerInMemoryDb.UpdateRoom(roomId, request);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact(DisplayName = "Update Valid Data But Existed Room Return Bad Request")]
    public void UpdateValidDataButExistedRoom_ReturnBadRequest()
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
        var result = _controllerInMemoryDb.UpdateRoom(roomId, request);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact(DisplayName = "Update Valid Data Return Ok Object Created")]
    public void UpdateValidData_ReturnOk()
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
        var result = _controllerInMemoryDb.UpdateRoom(roomId, request);

        // assert
        Assert.IsType<OkObjectResult>(result);
    }
}