using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[Route("api/address")]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly AmsContext _context;

    public AddressController(AmsContext context)
    {
        _context = context;
    }
    
    //get list provinces
    [HttpGet("provinces")]
    public IActionResult GetProvinces()
    {
        var listProvince = _context.Provinces.ToList()
            .Select(p => new ProvinceResponse() { Id = p.Id, Code = p.Code, Name = p.Name });
        // empty list
        if (!listProvince.Any())
        {
            return BadRequest(CustomResponse.BadRequest("Province not found", "address-error-000001"));
        }
        return Ok(CustomResponse.Ok("Get list province success", listProvince));
    }
    
    // get list districts by provinceId
    [HttpGet("provinces/{provinceId:int}/districts")]
    public IActionResult GetDistricts(int provinceId)
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
                .BadRequest("District with provinceId not found", "address-error-000002"));
        }
        return Ok(CustomResponse.Ok("Get list district success", listDistrict));
    }
    
    // get list wards by provinceId and districtId
    [HttpGet("provinces/{provinceId:int}/districts/{districtId:int}/wards")]
    public IActionResult GetWards(int provinceId, int districtId)
    {
        var listWard = _context.Wards.ToList()
            .Where(w => w.ProvinceId == provinceId && w.DistrictId == districtId)
            .Select(w => new WardResponse() { WardId = w.Id, Name = w.Name, Prefix = w.Prefix });
        // empty list
        if (!listWard.Any())
        {
            return BadRequest(CustomResponse
                .BadRequest("Ward with provinceId and districtId not found", "address-error-000003"));
        }
        return Ok(CustomResponse.Ok("Get list ward success", listWard));
    }
}