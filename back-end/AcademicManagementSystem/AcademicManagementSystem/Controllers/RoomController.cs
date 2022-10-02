using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Extension;
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
            var error = ErrorDescription.Error["E0001"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
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
            var error = ErrorDescription.Error["E0002"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Get all rooms by centerId successfully", rooms));
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
            var error = ErrorDescription.Error["E0003"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!IsCenterExists(roomRequest.CenterId))
        {
            var error = ErrorDescription.Error["E0004"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!IsRoomTypeExists(roomRequest.RoomTypeId))
        {
            var error = ErrorDescription.Error["E0005"];
            return BadRequest(CustomResponse.BadRequest( error.Message, error.Type));
        }

        if (roomRequest.Capacity < 20 || roomRequest.Capacity > 100)
        {
            var error = ErrorDescription.Error["E0006"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (string.IsNullOrWhiteSpace(roomRequest.Name))
        {
            var error = ErrorDescription.Error["E0007"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(room.Name, RegexRoomName))
        {
            var error = ErrorDescription.Error["E0008"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        _context.Rooms.Add(room);
        _context.SaveChanges();
        return Ok(CustomResponse.Ok("Create room successfully", roomRequest));
    }

    //Update room
    [HttpPut("api/room/update/{roomId:int}")]
    public IActionResult UpdateRoom(int roomId, [FromBody] RoomRequest roomRequest)
    {
        var roomToUpdate = _context.Rooms.FirstOrDefault(r => r.Id == roomId);
        if (roomToUpdate == null)
        {
            var error = ErrorDescription.Error["E0009"];
            return BadRequest(
                CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (string.IsNullOrWhiteSpace(roomRequest.Name))
        {
            var error = ErrorDescription.Error["E0010"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(roomRequest.Name, RegexRoomName))
        {
            var error = ErrorDescription.Error["E0011"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!IsCenterExists(roomRequest.CenterId))
        {
            var error = ErrorDescription.Error["E0012"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!IsRoomTypeExists(roomRequest.RoomTypeId))
        {
            var error = ErrorDescription.Error["E0013"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (roomRequest.Capacity < 20 || roomRequest.Capacity > 100)
        {
            var error = ErrorDescription.Error["E0014"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        roomToUpdate.RoomTypeId = roomRequest.RoomTypeId;
        roomToUpdate.Name = roomRequest.Name;
        roomToUpdate.Capacity = roomRequest.Capacity;

        _context.SaveChanges();
        return Ok(CustomResponse.Ok("Update room successfully", roomToUpdate));
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