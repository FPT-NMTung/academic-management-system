using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Models.RoomController.RoomModel;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class RoomController : ControllerBase
{
    private readonly AmsContext _context;

    private const string RegexRoomName =
        "^[a-zA-Z0-9](?!.*--)(?!.*  )(?!.*__)(?!.*-_)(?!.*_-)(?!.* _)(?!.*_ )(?!.*- )(?!.* -)[a-zA-Z0-9-_ ]*[a-zA-Z0-9]$";

    public RoomController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/center/{centerId:int}/rooms")]
    public IActionResult GetRoomsByCenterId(int centerId)
    {
        if (!IsCenterExists(centerId))
        {
            return BadRequest(CustomResponse.BadRequest("Center not found", "room-error-000004"));
        }

        var rooms = _context.Rooms.ToList()
            .Where(r => r.CenterId == centerId)
            .Select(r => new RoomResponse()
            {
                Id = r.Id,
                CenterId = r.CenterId,
                CenterName = _context.Centers.FirstOrDefault(c => c.Id == r.CenterId)!.Name,
                RoomTypeId = r.RoomTypeId,
                RoomTypeValue = _context.RoomTypes.FirstOrDefault(rt => rt.Id == r.RoomTypeId)!.Value,
                Name = r.Name,
                Capacity = r.Capacity
            });

        if (!rooms.Any())
        {
            return BadRequest(CustomResponse.BadRequest("Room Not Found", "room-error-000001"));
        }

        return Ok(CustomResponse.Ok("Get all rooms by room type id success", rooms));
    }

    //create new room
    [HttpPost]
    [Route("api/room/create")]
    public IActionResult CreateRoom([FromBody] RoomRequest roomRequest)
    {
        var room = new Room()
        {
            CenterId = roomRequest.CenterId,
            RoomTypeId = roomRequest.RoomTypeId,
            Name = roomRequest.Name,
            Capacity = roomRequest.Capacity
        };

        if (IsRoomExists(roomRequest))
        {
            return BadRequest(CustomResponse.BadRequest("This room already exists", "room-error-000003"));
        }

        if (!IsCenterExists(roomRequest.CenterId))
        {
            return BadRequest(CustomResponse.BadRequest("Center not found", "room-error-000004"));
        }

        if (!IsRoomTypeExists(roomRequest.RoomTypeId))
        {
            return BadRequest(CustomResponse.BadRequest("Room type not found", "room-error-000005"));
        }

        if (roomRequest.Capacity < 20 || roomRequest.Capacity > 100)
        {
            return BadRequest(CustomResponse.BadRequest("Capacity must be between 20 and 100", "room-error-000006"));
        }

        if (string.IsNullOrWhiteSpace(roomRequest.Name))
        {
            return BadRequest(CustomResponse.BadRequest("Name cannot be empty", "room-error-000007"));
        }

        if (!Regex.IsMatch(room.Name, RegexRoomName))
        {
            return BadRequest(CustomResponse.BadRequest("Name must match format", "room-error-000010"));
        }

        _context.Rooms.Add(room);
        _context.SaveChanges();
        return Ok(CustomResponse.Ok("Create room success", roomRequest));
    }

    //Update room
    [HttpPut("api/room/update/{roomId:int}")]
    public IActionResult UpdateRoom(int roomId, [FromBody] RoomRequest roomRequest)
    {
        var roomToUpdate = _context.Rooms.FirstOrDefault(r => r.Id == roomId);
        if (roomToUpdate == null)
        {
            return BadRequest(
                CustomResponse.BadRequest("Room with this id not found", "room-error-000008"));
        }

        if (string.IsNullOrWhiteSpace(roomRequest.Name))
        {
            return BadRequest(CustomResponse.BadRequest("Name cannot be empty", "room-error-000009"));
        }

        if (!Regex.IsMatch(roomRequest.Name, RegexRoomName))
        {
            return BadRequest(CustomResponse.BadRequest("Name must match format", "room-error-000010"));
        }

        if (!IsCenterExists(roomRequest.CenterId))
        {
            return BadRequest(CustomResponse.BadRequest("Center not found", "room-error-000004"));
        }

        if (!IsRoomTypeExists(roomRequest.RoomTypeId))
        {
            return BadRequest(CustomResponse.BadRequest("Room type not found", "room-error-000005"));
        }

        if (roomRequest.Capacity < 20 || roomRequest.Capacity > 100)
        {
            return BadRequest(CustomResponse.BadRequest("Capacity must be between 20 and 100", "room-error-000006"));
        }

        roomToUpdate.RoomTypeId = roomRequest.RoomTypeId;
        roomToUpdate.Name = roomRequest.Name;
        roomToUpdate.Capacity = roomRequest.Capacity;

        _context.SaveChanges();
        return Ok(CustomResponse.Ok("Room updated successfully", roomToUpdate));
    }

    private bool IsRoomExists(RoomRequest roomRequest)
    {
        return _context.Rooms.Any(r =>
            r.CenterId == roomRequest.CenterId &&
            r.RoomTypeId == roomRequest.RoomTypeId &&
            r.Name == roomRequest.Name);
    }

    private bool IsRoomTypeExists(int roomTypeId)
    {
        return _context.RoomTypes.Any(e => e.Id == roomTypeId);
    }

    private bool IsCenterExists(int centerId)
    {
        return _context.Centers.Any(e => e.Id == centerId);
    }
}