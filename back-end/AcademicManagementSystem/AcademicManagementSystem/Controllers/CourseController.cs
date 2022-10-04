using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.CourseController;
using AcademicManagementSystem.Models.CourseFamilyController;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class CourseController : ControllerBase
{
    private readonly AmsContext _context;

    public CourseController(AmsContext context)
    {
        _context = context;
    }

    // get all courses
    [HttpGet]
    [Route("api/courses")]
    public IActionResult GetCourses()
    {
        var courses = _context.Courses.Include(c => c.CourseFamily)
            .ToList()
            .Select(c => new CourseResponse()
            {
                Code = c.Code, CourseFamilyCode = c.CourseFamilyCode, Name = c.Name, SemesterCount = c.SemesterCount,
                IsActive = c.IsActive, CreatedAt = c.CreatedAt, UpdatedAt = c.UpdatedAt,
                CourseFamily = new CourseFamilyResponse()
                {
                    Code = c.CourseFamily.Code, Name = c.CourseFamily.Name,
                    PublishedYear = c.CourseFamily.PublishedYear, IsActive = c.CourseFamily.IsActive,
                    CreatedAt = c.CourseFamily.CreatedAt, UpdatedAt = c.CourseFamily.UpdatedAt
                }
            });
        return Ok(courses);
    }
}