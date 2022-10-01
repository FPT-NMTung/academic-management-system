using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Extension;
using AcademicManagementSystem.Models.AddressController;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.CenterController;
using Microsoft.AspNetCore.Mvc;

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
    public IActionResult GetCenters()
    {
        var centers = _context.Centers.ToList()
            .Select(c => new CenterResponse()
            {
                Id = c.Id, ProvinceId = c.ProvinceId, DistrictId = c.DistrictId, WardId = c.WardId,
                Name = c.Name, CreatedAt = c.CreatedAt, UpdatedAt = c.UpdatedAt
            });
        if (!centers.Any())
        {
            var error = ErrorDescription.Error["E0020"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }
        return Ok(CustomResponse.Ok("Get all centers success", centers));
    }
    
    // get address by center id
    [HttpGet]
    [Route("api/centers/{id:int}/address")]
    public IActionResult GetAddressByCenterId(int id)
    {
        var center = _context.Centers.FirstOrDefault(c => c.Id == id);
        if (center == null)
        {
            var error = ErrorDescription.Error["E0018"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }
        var province = _context.Provinces.FirstOrDefault(p => p.Id == center.ProvinceId);
        var district = _context.Districts.FirstOrDefault(d => d.Id == center.DistrictId);
        var ward = _context.Wards.FirstOrDefault(w => w.Id == center.WardId);
        if (province == null || district == null || ward == null)
        {
            var error = ErrorDescription.Error["E0019"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }
        var centerAddress = new AddressResponse()
        {
            Province = new ProvinceResponse() { Id = province.Id, Code = province.Code, Name = province.Name },
            District = new DistrictResponse() { Id = district.Id, Name = district.Name, Prefix = district.Prefix },
            Ward = new WardResponse() { Id = ward.Id, Name = ward.Name, Prefix = ward.Prefix }
        };
        return Ok(CustomResponse.Ok("Get address success", centerAddress));
    }
    
    // create center
    [HttpPost]
    [Route("api/centers")]
    public IActionResult CreateCenter([FromBody] CreateCenterRequest request)
    {
        var center = new Center()
        {
            ProvinceId = request.ProvinceId, DistrictId = request.DistrictId, WardId = request.WardId,
            Name = request.Name!.Trim(), CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now
        };

        if (IsCenterExists(request))
        {
            var error = ErrorDescription.Error["E0015"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }
        
        if (string.IsNullOrWhiteSpace(request.Name.Trim()))
        {
            var error = ErrorDescription.Error["E0016"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }
        
        // if (!Regex.IsMatch(request.Name, StringConstant.RegexCenterName))
        // {
        //     var error = ErrorDescription.Error["E0017"];
        //     return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        // }
        
        _context.Centers.Add(center);
        _context.SaveChanges();
        return Ok(CustomResponse.Ok("Create center success", center));
    }
    
    // is center exists
    private bool IsCenterExists(CreateCenterRequest request)
    {
        return _context.Centers.Any(c => c.ProvinceId == request.ProvinceId 
                                         && c.DistrictId == request.DistrictId 
                                         && c.WardId == request.WardId 
                                         && c.Name == request.Name);
    }
}