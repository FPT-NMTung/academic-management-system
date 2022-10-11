using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.GradeCategoryController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class GradeCategoryController : ControllerBase
{
    private readonly AmsContext _context;

    public GradeCategoryController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/grade-categories")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetGradeCategories()
    {
        var gradeCategories =
            _context.GradeCategories.Select(gc => new GradeCategoryResponse()
            {
                Id = gc.Id,
                Name = gc.Name,
            });
        return Ok(CustomResponse.Ok("Get all grade categories successfully", gradeCategories));
    }
}