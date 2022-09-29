using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Models.AddressController;
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
    
    [HttpGet]
    [Route("api/address/province")]
    public IActionResult GetProvince()
    {
        //var listProvince = from _context.Districts.ToList();
        var listProvince = from p in _context.Provinces.ToList()
            select new ProvinceResponse()
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name
            };

        return Ok(CustomResponse.Ok("Get list province success", listProvince));
    }
    
    
}