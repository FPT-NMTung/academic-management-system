using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.GenderController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class GenderController : ControllerBase
{
    private readonly AmsContext _context;

    public GenderController(AmsContext context)
    {
        _context = context;
    }

    // get all gender
    [HttpGet]
    [Route("api/genders")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetGenders()
    {
        var genderResponses = _context.Genders.Select(g => new GenderResponse()
        {
            Id = g.Id,
            Value = g.Value
        });

        return Ok(CustomResponse.Ok("Get all genders successfully", genderResponses));
    }
}