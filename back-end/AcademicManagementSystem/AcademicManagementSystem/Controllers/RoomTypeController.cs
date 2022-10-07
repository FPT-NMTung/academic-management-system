using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Models.RoomController.RoomModel;
using AcademicManagementSystem.Models.RoomController.RoomTypeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class RoomTypeController : ControllerBase
{
    private readonly AmsContext _context;

    public RoomTypeController(AmsContext context)
    {
        _context = context;
    }

    //get all room types
    [HttpGet]
    [Route("api/room-types")]
    [Authorize(Roles = "admin")]
    public IActionResult GetRoomTypes()
    {
        var roomTypes = _context.RoomTypes.Select(rt => new RoomTypeResponse()
        {
            Id = rt.Id,
            Value = rt.Value
        });

        return Ok(CustomResponse.Ok("Get all room type success", roomTypes));
    }
}