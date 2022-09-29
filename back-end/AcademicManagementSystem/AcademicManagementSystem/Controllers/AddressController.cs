using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Models.AddressController;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class AddressController : ControllerBase
{
    private readonly AmsContext _context;

    public AddressController(AmsContext context)
    {
        _context = context;
    }
    
    //get list province
    [HttpGet]
    [Route("api/address/province")]
    public IActionResult GetProvince()
    {
        var listProvince = _context.Provinces.ToList()
            .Select(p => new ProvinceResponse() { Id = p.Id, Code = p.Code, Name = p.Name });
        // empty list
        if (listProvince.Any())
        {
            return BadRequest(CustomResponse.BadRequest("Province not found", "address-error-000001"));
        }
        return Ok(CustomResponse.Ok("Get list province success", listProvince));
    }
    
    // get list district by province id
    [HttpGet]
    [Route("api/address/district/{provinceId}")]
    public IActionResult GetDistrict(int provinceId)
    {
        var listDistrict = _context.Districts.ToList()
            .Where(d => d.ProvinceId == provinceId)
            .Select(d =>
                new DistrictResponse()
                {
                    DistrictId = d.Id, Name = d.Name, Prefix = d.Prefix
                });
        
        // empty list
        if (!listDistrict.Any())
        {
            return BadRequest(CustomResponse
                .BadRequest("District with this ProvinceId not found", "address-error-000002"));
        }
        return Ok(CustomResponse.Ok("Get list district success", listDistrict));
    }
    
    // get list ward by district id
    [HttpGet]
    [Route("api/address/ward/{districtId}")]
    public IActionResult GetWard(int districtId)
    {
        var listWard = _context.Wards.ToList()
            .Where(w => w.DistrictId == districtId)
            .Select(w =>
                new WardResponse()
                {
                    WardId = w.Id, Name = w.Name, Prefix = w.Prefix
                });
        // empty list
        if (listWard.Any())
        { 
            return BadRequest(CustomResponse
                .BadRequest("Ward with this DistrictId not found", "address-error-000003"));
        }
        
        return Ok(CustomResponse.Ok("Get list ward success", listWard));
    }
}