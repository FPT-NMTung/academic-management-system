using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.CourseController;
using AcademicManagementSystem.Models.CourseFamilyController;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "admin,sro")]
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

    // get course by code
    [HttpGet]
    [Route("api/courses/{code}")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult GetCourseByCode(string code)
    {
        var course = _context.Courses.Include(c => c.CourseFamily)
            .FirstOrDefault(c => c.Code == code.Trim());
        if (course == null)
        {
            return NotFound(CustomResponse.NotFound("Not Found Course"));
        }

        var courseResponse = new CourseResponse()
        {
            Code = course.Code, CourseFamilyCode = course.CourseFamilyCode, Name = course.Name,
            SemesterCount = course.SemesterCount, IsActive = course.IsActive, CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt,
            CourseFamily = new CourseFamilyResponse()
            {
                Code = course.CourseFamily.Code, Name = course.CourseFamily.Name,
                PublishedYear = course.CourseFamily.PublishedYear, IsActive = course.CourseFamily.IsActive,
                CreatedAt = course.CourseFamily.CreatedAt, UpdatedAt = course.CourseFamily.UpdatedAt
            }
        };
        return Ok(courseResponse);
    }
}