using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Controllers;
using AcademicManagementSystem.Models.RoomController.RoomTypeModel;
using AcademicManagementSystemTest.MockData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystemTest.System.Controller;

public class RoomTypeControllerTest
{
    private readonly RoomTypeController _controller;
    private readonly AmsContext _context;

    public RoomTypeControllerTest()
    {
        var options = new DbContextOptionsBuilder<AmsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AmsContext(options);
        _controller = new RoomTypeController(_context);
        Init();
    }

    private void Init()
    {
        _context.RoomTypes.AddRange(RoomTypeMockData.RoomTypes);
        _context.SaveChanges();
    }

    [Fact(DisplayName = "Get All Room Type Success")]
    public void Get_WhenCalled_ReturnsOkResult()
    {
        // Arrange
        // Act
        var okResult = _controller.GetRoomTypes();

        // Assert
        Assert.IsType<OkObjectResult>(okResult as OkObjectResult);
    }

    [Fact(DisplayName = "Get All Data Room Type")]
    public void Get_WhenCalled_ReturnsAllItems()
    {
        // Arrange

        // Act
        var result = _controller.GetRoomTypes() as OkObjectResult;

        // Assert
        var items = Assert.IsType<ResponseCustom>(result!.Value);
        var roomTypes = Assert.IsType<List<RoomTypeResponse>>(items.Data);
        Assert.Equal(2, roomTypes.Count);
    }
}