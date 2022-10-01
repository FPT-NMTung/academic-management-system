using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Models.CenterController;
using AcademicManagementSystem.Models.RoomController.RoomModel;
using DocumentFormat.OpenXml.Office2010.Excel;
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
    /*[HttpGet]
    [Route("api/centers/{id:int}/address")]
    public IActionResult GetAddressByCenterId(int id)
    {
        var center = _context.Centers.FirstOrDefault(c => c.Id == id);
        var province = _context.Provinces.FirstOrDefault(p => p.Id == center.ProvinceId);
        var district = _context.Districts.FirstOrDefault(d => d.Id == center.DistrictId);
        var ward = _context.Wards.FirstOrDefault(w => w.Id == center.WardId);
        
        if (center == null || province == null || district == null || ward == null) {}
        {
            return BadRequest(CustomResponse.BadRequest("Center not found", "center-error-000002"));
        }
        var address = _context.Addresses.FirstOrDefault(a => a.Id == center.AddressId);
        if (address == null)
            return BadRequest(CustomResponse.BadRequest("Address not found", "center-error-000002"));
        var addressResponse = new AddressResponse()
        {
            Id = address.Id, ProvinceId = address.ProvinceId, DistrictId = address.DistrictId,
            WardId = address.WardId, Address = address.Address, CreatedAt = address.CreatedAt,
            UpdatedAt = address.UpdatedAt
        };
        return Ok(CustomResponse.Ok("Get address by center id success", addressResponse));
    }*/
}