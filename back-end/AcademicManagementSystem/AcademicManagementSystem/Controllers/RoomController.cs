using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.RoomController.RoomModel;
using AcademicManagementSystem.Models.RoomController.RoomTypeModel;
using AcademicManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class RoomController : ControllerBase
{
    private readonly AmsContext _context;
    private readonly IUserService _userService;

    public RoomController(AmsContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpGet]
    [Route("api/rooms")]
    [Authorize(Roles = "admin")]
    public IActionResult GetRoomsByCenterId([FromQuery] int? centerId)
    {
        if (centerId != null && !IsCenterExists(centerId))
        {
            var error = ErrorDescription.Error["E0001"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var roomsResponses = _context.Rooms
            .Include(r => r.Center)
            .Include(r => r.RoomType)
            .ToList();

        if (centerId != null)
        {
            roomsResponses = roomsResponses.Where(r => r.CenterId == centerId).ToList();
        }

        var responses = roomsResponses.Select(r => new RoomResponse()
        {
            Id = r.Id,
            CenterId = r.CenterId,
            CenterName = r.Center.Name,
            RoomType = new RoomTypeResponse()
            {
                Id = r.RoomTypeId,
                Value = r.RoomType.Value
            },
            Name = r.Name,
            Capacity = r.Capacity,
            IsActive = r.IsActive
        });

        return Ok(CustomResponse.Ok("Get all rooms by centerId successfully", responses));
    }
    
    [HttpGet]
    [Route("api/rooms/get-by-sro")]
    [Authorize(Roles = "sro")]
    public IActionResult GetRoomsBySro()
    {
        var userId = Int32.Parse(_userService.GetUserId());
        var user = _context.Users.FirstOrDefault(u => u.Id == userId)!;

        var roomsResponses = _context.Rooms
            .Include(r => r.Center)
            .Include(r => r.RoomType)
            .ToList();
        
        roomsResponses = roomsResponses.Where(r => r.CenterId == user.CenterId).ToList();

        var responses = roomsResponses.Select(r => new RoomResponse()
        {
            Id = r.Id,
            CenterId = r.CenterId,
            CenterName = r.Center.Name,
            RoomType = new RoomTypeResponse()
            {
                Id = r.RoomTypeId,
                Value = r.RoomType.Value
            },
            Name = r.Name,
            Capacity = r.Capacity,
            IsActive = r.IsActive
        });

        return Ok(CustomResponse.Ok("Get all rooms by sro successfully", responses));
    }

    //create new room
    [HttpPost]
    [Route("api/rooms")]
    [Authorize(Roles = "admin")]
    public IActionResult CreateRoom([FromBody] CreateRoomRequest createRoomRequest)
    {
        var roomCreate = new Room()
        {
            CenterId = createRoomRequest.CenterId,
            RoomTypeId = createRoomRequest.RoomTypeId,
            Name = createRoomRequest.Name.Trim(),
            Capacity = createRoomRequest.Capacity
        };

        roomCreate.Name = Regex.Replace(roomCreate.Name, StringConstant.RegexWhiteSpaces, " ");

        if (string.IsNullOrWhiteSpace(createRoomRequest.Name))
        {
            var error = ErrorDescription.Error["E0007"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (Regex.IsMatch(roomCreate.Name, StringConstant.RegexSpecialCharactersNotAllowForRoomName))
        {
            var error = ErrorDescription.Error["E0008"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsRoomExists(roomCreate, false, 0))
        {
            var error = ErrorDescription.Error["E0003"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (createRoomRequest.Capacity < 20 || createRoomRequest.Capacity > 100)
        {
            var error = ErrorDescription.Error["E0006"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        _context.Rooms.Add(roomCreate);

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0005"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var roomResponse = GetRoomResponse(roomCreate.Id);

        return Ok(CustomResponse.Ok("Create room successfully", roomResponse));
    }

    //Update room
    [HttpPut("api/rooms/{roomId:int}")]
    [Authorize(Roles = "admin")]
    public IActionResult UpdateRoom(int roomId, [FromBody] UpdateRoomRequest updateRoomRequest)
    {
        var roomToUpdate = _context.Rooms.FirstOrDefault(r => r.Id == roomId);
        if (roomToUpdate == null)
        {
            var error = ErrorDescription.Error["E0009"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (string.IsNullOrWhiteSpace(updateRoomRequest.Name))
        {
            var error = ErrorDescription.Error["E0010"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (Regex.IsMatch(updateRoomRequest.Name.Trim(), StringConstant.RegexSpecialCharactersNotAllowForRoomName))
        {
            var error = ErrorDescription.Error["E0011"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (updateRoomRequest.Capacity < 20 || updateRoomRequest.Capacity > 100)
        {
            var error = ErrorDescription.Error["E0014"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        roomToUpdate.RoomTypeId = updateRoomRequest.RoomTypeId;
        roomToUpdate.Name = Regex.Replace(updateRoomRequest.Name.Trim(), StringConstant.RegexWhiteSpaces, " ");
        roomToUpdate.Capacity = updateRoomRequest.Capacity;

        if (IsRoomExists(roomToUpdate, true, roomId))
        {
            var error = ErrorDescription.Error["E0015"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0005"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var roomResponse = GetRoomResponse(roomToUpdate.Id);
        return Ok(CustomResponse.Ok("Update room successfully", roomResponse));
    }

    //Change status of room
    [HttpPatch("api/rooms/{roomId:int}/change-status")]
    [Authorize(Roles = "admin")]
    public IActionResult ChangeStatusRoom(int roomId)
    {
        var roomToUpdate = _context.Rooms.FirstOrDefault(r => r.Id == roomId);
        if (roomToUpdate == null)
        {
            var error = ErrorDescription.Error["E2059"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        roomToUpdate.IsActive = !roomToUpdate.IsActive;

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E2061"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var roomResponse = GetRoomResponse(roomToUpdate.Id);
        return Ok(CustomResponse.Ok("Change status room successfully", roomResponse));
    }

    //Check can delete room
    [HttpGet("api/rooms/{roomId:int}/can-delete")]
    [Authorize(Roles = "admin")]
    public IActionResult CanDeleteRoom(int roomId)
    {
        var room = _context.Rooms
            .Include(r => r.ClassSchedulesExamRoom)
            .Include(r => r.ClassSchedulesLabRoom)
            .Include(r => r.ClassSchedulesTheoryRoom)
            .FirstOrDefault(r => r.Id == roomId);

        if (room == null)
        {
            var error = ErrorDescription.Error["E2059"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var canDelete = room.ClassSchedulesExamRoom.Any()
                        || room.ClassSchedulesLabRoom.Any()
                        || room.ClassSchedulesTheoryRoom.Any();

        return Ok(CustomResponse.Ok("Check can delete room successfully", new CheckRoomCanDeleteResponse()
        {
            CanDelete = canDelete
        }));
    }

    //Delete room
    [HttpDelete("api/rooms/{roomId:int}")]
    [Authorize(Roles = "admin")]
    public IActionResult DeleteRoom(int roomId)
    {
        var roomToDelete = _context.Rooms.FirstOrDefault(r => r.Id == roomId);
        if (roomToDelete == null)
        {
            var error = ErrorDescription.Error["E2059"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        _context.Rooms.Remove(roomToDelete);

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E2060"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Delete room successfully", null!));
    }

    private bool IsRoomExists(Room room, bool isUpdate, int idUpdate)
    {
        if (isUpdate)
        {
            return _context.Rooms.Any(r =>
                r.CenterId == room.CenterId &&
                r.Name == room.Name &&
                r.Id != idUpdate);
        }

        return _context.Rooms.Any(r =>
            r.CenterId == room.CenterId &&
            r.Name == room.Name);
    }

    private bool IsCenterExists(int? centerId)
    {
        return _context.Centers.Any(e => e.Id == centerId);
    }

    private IQueryable<RoomResponse> GetRoomResponse(int roomId)
    {
        var roomResponse = _context.Rooms
            .Include(r => r.Center)
            .Include(r => r.RoomType)
            .Where(r => r.Id == roomId)
            .Select(r => new RoomResponse()
            {
                Id = r.Id,
                CenterId = r.CenterId,
                CenterName = r.Center.Name,
                RoomType = new RoomTypeResponse()
                {
                    Id = r.RoomTypeId,
                    Value = r.RoomType.Value
                },
                Name = r.Name,
                Capacity = r.Capacity
            });
        return roomResponse;
    }
}