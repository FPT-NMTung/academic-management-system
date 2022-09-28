using AcademicManagementSystem.Context;
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
        var listProvince = _context.Districts.ToList();
        return Ok(CustomResponse.Ok("Get list province success", listProvince));
    }
    
    
}