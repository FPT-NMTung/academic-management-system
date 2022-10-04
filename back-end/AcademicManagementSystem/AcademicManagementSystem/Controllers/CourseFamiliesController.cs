using AcademicManagementSystem.Context;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class CourseFamiliesController
{
    private readonly AmsContext _context;
    
    public CourseFamiliesController(AmsContext context)
    {
        _context = context;
    }
    
    // // get all course families
    // [HttpGet]
    // [Route("api/course-families")]
    // public IActionResult GetCourseFamilies()
    // {
    //     var courseFamilies = _context.CourseFamilies.ToList();
    //     return Ok(courseFamilies);
    // }
}