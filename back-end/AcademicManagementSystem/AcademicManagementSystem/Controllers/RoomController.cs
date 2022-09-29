using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Models.RoomController.RoomModel;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[Route("api/rooms")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly AmsContext _context;

    public RoomController(AmsContext context)
    {
        _context = context;
    }

    //GET: api/rooms/center/{centerId}
    [HttpGet]
    [Route("center/{centerId:int}")]
    public IActionResult GetRoomsByCenterId(int centerId)
    {
        var rooms = _context.Rooms.ToList()
            .Where(r => r.CenterId == centerId)
            .Select(r => new RoomResponse()
            {
                RoomCode = r.RoomCode,
                Name = r.Name,
                Capacity = r.Capacity
            });

        if (!rooms.Any())
        {
            return BadRequest(CustomResponse.BadRequest("No rooms found", "room-error-000001"));
        }

        return Ok(CustomResponse.Ok("Get all rooms by room type id success", rooms));
    }

    //GET: api/rooms/room-type/{roomTypeId}
    //get all rooms by roomType Id
    [HttpGet("room-type/{roomTypeId:int}")]
    public IActionResult GetRoomsByRoomTypeId(int roomTypeId)
    {
        var list = _context.Rooms.ToList();
        var rooms = list
            .Where(r => r.RoomTypeId == roomTypeId)
            .Select(r => new RoomResponse()
            {
                RoomCode = r.RoomCode,
                Name = r.Name,
                Capacity = r.Capacity
            });

        if (!rooms.Any())
        {
            return BadRequest(CustomResponse.BadRequest("No room found", "room-error-000002"));
        }

        return Ok(CustomResponse.Ok("Get all rooms by room type id success", rooms));
    }

    //POST: api/rooms
    //create new room
    [HttpPost]
    public IActionResult CreateRoom([FromBody] RoomRequest roomRequest)
    {
        var room = new Room()
        {
            RoomCode = roomRequest.RoomCode,
            CenterId = roomRequest.CenterId,
            RoomTypeId = roomRequest.RoomTypeId,
            Name = roomRequest.Name,
            Capacity = roomRequest.Capacity
        };
        _context.Rooms.Add(room);

        if (IsRoomExists(roomRequest.RoomCode))
        {
            return BadRequest(CustomResponse.BadRequest("Room code already exists", "room-error-000003"));
        }

        if (!IsCenterExists(roomRequest.CenterId))
        {
            return BadRequest(CustomResponse.BadRequest("Center not found", "room-error-000004"));
        }
        
        if(!IsRoomTypeExists(roomRequest.RoomTypeId))
        {
            return BadRequest(CustomResponse.BadRequest("Room type not found", "room-error-000005"));
        }
        
        if(roomRequest.Capacity <= 0)
        {
            return BadRequest(CustomResponse.BadRequest("Capacity must be greater than 0", "room-error-000006"));
        }
        
        if(string.IsNullOrWhiteSpace(roomRequest.Name))
        {
            return BadRequest(CustomResponse.BadRequest("Name cannot be empty", "room-error-000007"));
        }
        
        _context.SaveChanges();
        return Ok(CustomResponse.Ok("Create room success", roomRequest));
    }

    //PUT: api/rooms/roomCode
    //Update room
    [HttpPut("{roomCode}")]
    public IActionResult UpdateRoom(string roomCode, [FromBody] RoomRequest room)
    {
        var roomToUpdate = _context.Rooms.FirstOrDefault(r => r.RoomCode == roomCode);
        if (roomToUpdate == null)
        {
            return BadRequest(
                CustomResponse.BadRequest("Room with code " + roomCode + " not found", "room-error-000008"));
        }
        
        if(string. IsNullOrWhiteSpace(room.Name))
        {
            return BadRequest(CustomResponse.BadRequest("Name cannot be empty", "room-error-000009"));
        }
        
        if(room.Capacity <= 0)
        {
            return BadRequest(CustomResponse.BadRequest("Capacity must be greater than 0", "room-error-000010"));
        }

        roomToUpdate.Name = room.Name;
        roomToUpdate.Capacity = room.Capacity;

        _context.SaveChanges();
        return Ok(CustomResponse.Ok("Room updated successfully", roomToUpdate));
    }

    //DELETE: api/rooms/roomCode
    [HttpDelete("{roomCode}")]
    public IActionResult DeleteRoom(string roomCode)
    {
        var roomToDelete = _context.Rooms.FirstOrDefault(r => r.RoomCode == roomCode);
        if (roomToDelete == null)
        {
            return BadRequest(
                CustomResponse.BadRequest("Room with code " + roomCode + " not found", "room-error-0000011"));
        }

        _context.Rooms.Remove(roomToDelete);
        _context.SaveChanges();
        return Ok(CustomResponse.Ok("Room deleted successfully", roomToDelete));
    }
    
    private bool IsRoomExists(string roomCode)
    {
        return _context.Rooms.Any(e => e.RoomCode == roomCode);
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