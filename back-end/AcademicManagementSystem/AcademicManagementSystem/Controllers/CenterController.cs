using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.CenterController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class CenterController : ControllerBase
{
    private readonly AmsContext _context;

    public CenterController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/centers")]
    [Authorize(Roles = "admin")]
    public IActionResult GetCenters()
    {
        var centers = _context.Centers.Include(e => e.Province)
            .Include(e => e.District)
            .Include(e => e.Ward)
            .Select(c => new CenterResponse()
            {
                Id = c.Id,
                Name = c.Name, CreatedAt = c.CreatedAt, UpdatedAt = c.UpdatedAt,
                IsActive = c.IsActive,
                Province = new ProvinceResponse()
                {
                    Id = c.Province.Id,
                    Name = c.Province.Name,
                    Code = c.Province.Code
                },
                District = new DistrictResponse()
                {
                    Id = c.District.Id,
                    Name = c.District.Name,
                    Prefix = c.District.Prefix,
                },
                Ward = new WardResponse()
                {
                    Id = c.Ward.Id,
                    Name = c.Ward.Name,
                    Prefix = c.Ward.Prefix,
                }
            }).ToList();
        return Ok(CustomResponse.Ok("Get all centers success", centers));
    }

    //get center by id
    [HttpGet]
    [Route("api/centers/{id:int}")]
    [Authorize(Roles = "admin")]
    public IActionResult GetCenterById(int id)
    {
        var center = _context.Centers.Include(e => e.Province)
            .Include(e => e.District)
            .Include(e => e.Ward)
            .FirstOrDefault(c => c.Id == id);
        if (center == null)
        {
            var error = ErrorDescription.Error["E1001"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var centerResponse = GetCenterResponse(center);
        return Ok(CustomResponse.Ok("Get center by id success", centerResponse));
    }

    private static CenterResponse GetCenterResponse(Center center)
    {
        var centerResponse = new CenterResponse()
        {
            Id = center.Id,
            Name = center.Name,
            CreatedAt = center.CreatedAt,
            UpdatedAt = center.UpdatedAt,
            IsActive = center.IsActive,
            Province = new ProvinceResponse()
            {
                Id = center.Province.Id,
                Name = center.Province.Name,
                Code = center.Province.Code
            },
            District = new DistrictResponse()
            {
                Id = center.District.Id,
                Name = center.District.Name,
                Prefix = center.District.Prefix,
            },
            Ward = new WardResponse()
            {
                Id = center.Ward.Id,
                Name = center.Ward.Name,
                Prefix = center.Ward.Prefix,
            }
        };
        return centerResponse;
    }

    // create center
    [HttpPost]
    [Route("api/centers")]
    [Authorize(Roles = "admin")]
    public IActionResult CreateCenter([FromBody] CreateCenterRequest request)
    {
        request.Name = request.Name.Trim();

        // is null or white space input
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            var error = ErrorDescription.Error["E1002"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // name format
        request.Name = Regex.Replace(request.Name, StringConstant.RegexWhiteSpaces, " ");
        request.Name = request.Name.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.Name, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1003"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Name.Length > 100)
        {
            var error = ErrorDescription.Error["E1004"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.ProvinceId is < 0 or > 63 || request.DistrictId is < 0 or > 709 ||
            request.WardId is < 0 or > 11283)
        {
            var error = ErrorDescription.Error["E1006"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsCenterNameExists(request))
        {
            var error = ErrorDescription.Error["E1047"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsCenterAddressExists(request))
        {
            var error = ErrorDescription.Error["E1005"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var center = new Center()
        {
            ProvinceId = request.ProvinceId, DistrictId = request.DistrictId, WardId = request.WardId,
            Name = request.Name, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now
        };
        try
        {
            _context.Centers.Add(center);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1050"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Center created successfully", center));
    }

    // update center
    [HttpPut]
    [Route("api/centers/{id:int}")]
    [Authorize(Roles = "admin")]
    public IActionResult UpdateCenter(int id, [FromBody] UpdateCenterRequest request)
    {
        request.Name = request.Name.Trim();

        var center = _context.Centers.FirstOrDefault(c => c.Id == id);
        if (center == null)
        {
            var error = ErrorDescription.Error["E1001"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // name format
        request.Name = Regex.Replace(request.Name, StringConstant.RegexWhiteSpaces, " ");
        request.Name = request.Name.Replace(" ' ", "'").Trim();
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            var error = ErrorDescription.Error["E1002"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (Regex.IsMatch(request.Name, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1003"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Name.Length > 100)
        {
            var error = ErrorDescription.Error["E1004"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsCenterNameWithDifferentIdExists(id, request.Name))
        {
            var error = ErrorDescription.Error["E1048"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsCenterAddressWithDifferentIdExists(id, request))
        {
            var error = ErrorDescription.Error["E1049"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.ProvinceId is < 0 or > 63 || request.DistrictId is < 0 or > 709 ||
            request.WardId is < 0 or > 11283)
        {
            var error = ErrorDescription.Error["E1006"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        try
        {
            center.ProvinceId = request.ProvinceId;
            center.DistrictId = request.DistrictId;
            center.WardId = request.WardId;
            center.Name = request.Name;
            center.UpdatedAt = DateTime.Now;
            _context.Centers.Update(center);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1051"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Center updated successfully", center));
    }

    [HttpGet]
    [Route("api/centers/{id:int}/can-delete")]
    [Authorize(Roles = "admin")]
    public IActionResult CheckCanDeleteCenter(int id)
    {
        var center = _context.Centers
            .Include(c => c.Classes)
            .Include(c => c.Rooms)
            .Include(c => c.Modules)
            .Include(c => c.Users)
            .FirstOrDefault(c => c.Id == id);
        if (center == null)
        {
            var error = ErrorDescription.Error["E1001"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var canDelete = center.Classes.Count == 0 && center.Modules.Count == 0 && center.Users.Count == 0 &&
                        center.Rooms.Count == 0;

        return Ok(CustomResponse.Ok("Check successfully", new CenterCheckDeleteResponse()
        {
            CanDelete = canDelete
        }));
    }

    // change activate center
    [HttpPatch]
    [Route("api/centers/{id:int}/change-activate")]
    [Authorize(Roles = "admin")]
    public IActionResult ChangeActivateCenter(int id)
    {
        try
        {
            var center = _context.Centers.FirstOrDefault(c => c.Id == id);

            if (center == null)
            {
                var error = ErrorDescription.Error["E1001"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            center.IsActive = !center.IsActive;
            center.UpdatedAt = DateTime.Now;
            _context.Centers.Update(center);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1051"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Center updated successfully", null!));
    }

    // delete center
    [HttpDelete]
    [Route("api/centers/{id:int}")]
    [Authorize(Roles = "admin")]
    public IActionResult DeleteCenter(int id)
    {
        try
        {
            var center = _context.Centers.FirstOrDefault(c => c.Id == id);
            if (center == null)
            {
                var error = ErrorDescription.Error["E1001"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            _context.Centers.Remove(center);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1052"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Center deleted successfully", null!));
    }

    // is center name exists
    private bool IsCenterNameExists(CreateCenterRequest request)
    {
        return _context.Centers.Any(c => string.Equals(c.Name.ToLower(), request.Name.Trim().ToLower()));
    }

    // is center address exists
    private bool IsCenterAddressExists(CreateCenterRequest request)
    {
        return _context.Centers.Any(c => c.ProvinceId == request.ProvinceId
                                         && c.DistrictId == request.DistrictId
                                         && c.WardId == request.WardId);
    }

    // is center name with different id exists
    private bool IsCenterNameWithDifferentIdExists(int id, string name)
    {
        return _context.Centers.Any(c => c.Id != id
                                         && string.Equals(c.Name.ToLower(), name.Trim().ToLower()));
    }

    // is center address with different id exists
    private bool IsCenterAddressWithDifferentIdExists(int id, UpdateCenterRequest request)
    {
        return _context.Centers.Any(c => c.Id != id
                                         && c.ProvinceId == request.ProvinceId
                                         && c.DistrictId == request.DistrictId
                                         && c.WardId == request.WardId);
    }
}