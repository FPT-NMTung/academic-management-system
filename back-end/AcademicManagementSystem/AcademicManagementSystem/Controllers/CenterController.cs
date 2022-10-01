using AcademicManagementSystem.Context;
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
            return BadRequest(CustomResponse.BadRequest("Center not found", "center-error-000001"));
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
            return BadRequest(CustomResponse.BadRequest("Center not found", "center-error-000002"));
        }
        var province = _context.Provinces.FirstOrDefault(p => p.Id == center.ProvinceId);
        var district = _context.Districts.FirstOrDefault(d => d.Id == center.DistrictId);
        var ward = _context.Wards.FirstOrDefault(w => w.Id == center.WardId);
        if (province == null || district == null || ward == null)
        {
            return BadRequest(CustomResponse.BadRequest("Center address not found", "center-error-000003"));
        }
        var centerAddress = new AddressResponse()
        {
            Province = new ProvinceResponse() { Id = province.Id, Code = province.Code, Name = province.Name },
            District = new DistrictResponse() { Id = district.Id, Name = district.Name, Prefix = district.Prefix },
            Ward = new WardResponse() { Id = ward.Id, Name = ward.Name, Prefix = ward.Prefix }
        };
        return Ok(CustomResponse.Ok("Get address success", centerAddress));
    }
}