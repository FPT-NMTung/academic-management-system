using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Models.CourseFamilyController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class CourseFamilyController : ControllerBase
{
    private readonly AmsContext _context;

    public CourseFamilyController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/course-families")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult GetCourseFamilies()
    {
        var courseFamilies = _context.CourseFamilies.ToList()
            .Select(cf => new CourseFamilyResponse()
            {
                Code = cf.Code, Name = cf.Name, PublishedYear = cf.PublishedYear, IsActive = cf.IsActive,
                CreatedAt = cf.CreatedAt, UpdatedAt = cf.UpdatedAt
            });
        return Ok(CustomResponse.Ok("Get course families success", courseFamilies));
    }
}