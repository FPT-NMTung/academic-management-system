using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.GpaController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class GpaController : ControllerBase
{
    private readonly AmsContext _context;

    public GpaController(AmsContext context)
    {
        _context = context;
    }

    // get all form
    [HttpGet]
    [Route("api/gpa/forms")]
    [Authorize(Roles = "admin, sro, student")]
    public IActionResult GetForms()
    {
        var forms = _context.Forms
            .Select(f => new FormResponse()
            {
                Id = f.Id, Title = f.Title, Description = f.Description
            }).ToList();
        return Ok(CustomResponse.Ok("Forms retrieved successfully", forms));
    }
}