using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
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

    // create course
    [HttpPost]
    [Route("api/courses")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult CreateCourse([FromBody] CreateCourseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name.Trim()) ||
            string.IsNullOrWhiteSpace(request.Code.ToUpper().Trim()) ||
            string.IsNullOrWhiteSpace(request.CourseFamilyCode.Trim()))
        {
            var error = ErrorDescription.Error["E1007"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check existed
        var course = _context.Courses
            .FirstOrDefault(c => c.Code == request.Code.ToUpper().Trim());
        if (course != null)
        {
            var error = ErrorDescription.Error["E1014"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var courseFamilyCode =
            _context.CourseFamilies.FirstOrDefault(cf => cf.Code == request.CourseFamilyCode.ToUpper().Trim());
        if (courseFamilyCode == null)
        {
            var error = ErrorDescription.Error["E1015"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // name format
        if (Regex.IsMatch(request.Name.Trim(), StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1016"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Name.Trim().Length > 255)
        {
            var error = ErrorDescription.Error["E1017"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // code format
        if (Regex.IsMatch(request.Code.ToUpper().Trim(), StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1018"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Code.ToUpper().Trim().Length > 100)
        {
            var error = ErrorDescription.Error["E1019"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.SemesterCount is < 1 or > 10)
        {
            var error = ErrorDescription.Error["E1020"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        course = new Course()
        {
            Code = request.Code.ToUpper().Trim(), CourseFamilyCode = request.CourseFamilyCode.ToUpper().Trim(),
            Name = request.Name.Trim(),
            SemesterCount = request.SemesterCount, IsActive = request.IsActive, CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Courses.Add(course);
        _context.SaveChanges();

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
        return Ok(CustomResponse.Ok("Course Created Successfully", courseResponse));
    }

    // update course
    [HttpPut]
    [Route("api/courses/{code}")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult UpdateCourse(string code, [FromBody] UpdateCourseRequest request)
    {
        var course = _context.Courses.FirstOrDefault(c => c.Code == code.ToUpper().Trim());
        if(course == null)
        {
            return NotFound(CustomResponse.NotFound("Not Found Course"));
        }
        
        if (string.IsNullOrWhiteSpace(request.Name.Trim()) ||
            string.IsNullOrWhiteSpace(request.CourseFamilyCode.ToUpper().Trim()))
        {
            var error = ErrorDescription.Error["E1007"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var courseFamilyCode =
            _context.CourseFamilies.FirstOrDefault(cf => cf.Code == request.CourseFamilyCode.ToUpper().Trim());
        if (courseFamilyCode == null)
        {
            var error = ErrorDescription.Error["E1015"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // name format
        if (Regex.IsMatch(request.Name.Trim(), StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1016"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Name.Trim().Length > 255)
        {
            var error = ErrorDescription.Error["E1017"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.SemesterCount is < 1 or > 10)
        {
            var error = ErrorDescription.Error["E1020"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        course.CourseFamilyCode = request.CourseFamilyCode.ToUpper().Trim();
        course.Name = request.Name.Trim();
        course.SemesterCount = request.SemesterCount;
        course.IsActive = request.IsActive;
        course.UpdatedAt = DateTime.Now;

        _context.SaveChanges();

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
        return Ok(CustomResponse.Ok("Course Updated Successfully", courseResponse));
    }
}
// update course