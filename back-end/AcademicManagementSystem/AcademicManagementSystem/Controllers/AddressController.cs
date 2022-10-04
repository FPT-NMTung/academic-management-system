using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.AddressController;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[Route("api/address")]
[ApiController]
[Authorize]
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
        return Ok(CustomResponse.Ok("Get list provinces success", listProvince));
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
                    Id = d.Id, Name = d.Name, Prefix = d.Prefix
                });
        // empty list
        if (!listDistrict.Any())
        {
            return NotFound(CustomResponse.NotFound("Not found districts"));
        }
        return Ok(CustomResponse.Ok("Get list districts success", listDistrict));
    }
    
    // get list wards by provinceId and districtId
    [HttpGet("provinces/{provinceId:int}/districts/{districtId:int}/wards")]
    public IActionResult GetWards(int provinceId, int districtId)
    {
        var listWard = _context.Wards.ToList()
            .Where(w => w.ProvinceId == provinceId && w.DistrictId == districtId)
            .Select(w => new WardResponse() { Id = w.Id, Name = w.Name, Prefix = w.Prefix });
        // empty list
        if (!listWard.Any())
        {
            return NotFound(CustomResponse.NotFound("Not found wards"));
        }
        return Ok(CustomResponse.Ok("Get list wards success", listWard));
    }
    
    //get address by specify provinceId, districtId, wardId
    [HttpGet("provinces/{provinceId:int}/districts/{districtId:int}/wards/{wardId:int}")]
    public IActionResult GetAddress(int provinceId, int districtId, int wardId)
    {
        var province = _context.Provinces.FirstOrDefault(p => p.Id == provinceId);
        var district = _context.Districts.FirstOrDefault(d => d.ProvinceId == provinceId && d.Id == districtId);
        var ward = _context.Wards.FirstOrDefault(w => w.DistrictId == districtId && w.ProvinceId == provinceId && w.Id == wardId);
        // not found
        if (province == null || district == null || ward == null)
        {
            return NotFound(CustomResponse.NotFound("Not found address"));
        }
        
        var address = new AddressResponse()
        {
            Province = new ProvinceResponse() { Id = province.Id, Code = province.Code, Name = province.Name },
            District = new DistrictResponse() { Id = district.Id, Name = district.Name, Prefix = district.Prefix },
            Ward = new WardResponse() { Id = ward.Id, Name = ward.Name, Prefix = ward.Prefix }
        };
        return Ok(CustomResponse.Ok("Get address success", address));
    }
}