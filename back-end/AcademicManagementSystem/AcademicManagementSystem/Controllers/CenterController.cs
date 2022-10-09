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
[Authorize(Roles = "admin")]
public class CenterController : ControllerBase
{
    private readonly AmsContext _context;

    public CenterController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/centers")]
    public IActionResult GetCenters()
    {
        var centers = _context.Centers.Include(e => e.Province)
            .Include(e => e.District)
            .Include(e => e.Ward)
            .Select(c => new CenterResponse()
            {
                Id = c.Id,
                Name = c.Name, CreatedAt = c.CreatedAt, UpdatedAt = c.UpdatedAt,
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

        var centerResponse = new CenterResponse()
        {
            Id = center.Id,
            Name = center.Name,
            CreatedAt = center.CreatedAt,
            UpdatedAt = center.UpdatedAt,
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
        return Ok(CustomResponse.Ok("Get center by id success", centerResponse));
    }

    // create center
    [HttpPost]
    [Route("api/centers")]
    public IActionResult CreateCenter([FromBody] CreateCenterRequest request)
    {
        request.Name = request.Name.Trim();

        // is null or white space input
        if (string.IsNullOrWhiteSpace(request.Name.Trim()))
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

        if (IsCenterExists(request))
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
            return BadRequest(CustomResponse.BadRequest(e.Message, e.GetType().ToString()));
        }

        return Ok(CustomResponse.Ok("Center created successfully", center));
    }

    // update center
    [HttpPut]
    [Route("api/centers/{id:int}")]
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
            return BadRequest(CustomResponse.BadRequest(e.Message, e.GetType().ToString()));
        }

        return Ok(CustomResponse.Ok("Center updated successfully", center));
    }

    // delete center
    [HttpDelete]
    [Route("api/centers/{id:int}")]
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
            return BadRequest(CustomResponse.BadRequest(e.Message, e.GetType().ToString()));
        }

        return Ok(CustomResponse.Ok("Center deleted successfully", null!));
    }

    // is center exists
    private bool IsCenterExists(CreateCenterRequest request)
    {
        return _context.Centers.Any(c => c.ProvinceId == request.ProvinceId
                                         && c.DistrictId == request.DistrictId
                                         && c.WardId == request.WardId
                                         && c.Name == request.Name.Trim());
    }
}