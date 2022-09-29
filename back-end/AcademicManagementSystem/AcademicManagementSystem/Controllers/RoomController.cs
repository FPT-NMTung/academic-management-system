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

    //GET: api/rooms/center/{roomTypeId}
    //get all rooms by roomType Id
    [HttpGet("center/{centerId}")]
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
    [HttpGet("room-type/{roomTypeId}")]
    public IActionResult GetRoomsByRoomTypeId(int roomTypeId)
    {
        var rooms = _context.Rooms.ToList()
            .Where(r => r.RoomTypeId == roomTypeId)
            .Select(r => new RoomResponse()
            {
                RoomCode = r.RoomCode,
                Name = r.Name,
                Capacity = r.Capacity
            });

        if (!rooms.Any())
        {
            return BadRequest(CustomResponse.BadRequest("No rooms found", "room-error-000002"));
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
        _context.SaveChanges();
        return Ok(CustomResponse.Ok("Create room success", room));
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
                CustomResponse.BadRequest("Room with code " + roomCode + " not found", "room-error-000004"));
        }

        roomToUpdate.Name = room.Name;
        roomToUpdate.Capacity = room.Capacity;

        _context.SaveChanges();
        return Ok(CustomResponse.Ok("Room updated successfully", roomToUpdate));
    }

    //DELETE: api/rooms/roomCode
    //Delete room
    [HttpDelete("{roomCode}")]
    public IActionResult DeleteRoom(string roomCode)
    {
        var roomToDelete = _context.Rooms.FirstOrDefault(r => r.RoomCode == roomCode);
        if (roomToDelete == null)
        {
            return BadRequest(
                CustomResponse.BadRequest("Room with code " + roomCode + " not found", "room-error-000005"));
        }

        _context.Rooms.Remove(roomToDelete);
        _context.SaveChanges();
        return Ok(CustomResponse.Ok("Room deleted successfully", roomToDelete));
    }
    
    private bool IsRoomExists(string roomCode)
    {
        return _context.Rooms.Any(e => e.RoomCode == roomCode);
    }
}