using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
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

    // get all families
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

    // get family by code
    [HttpGet]
    [Route("api/course-families/{code}")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult GetCourseFamilyByCode(string code)
    {
        var courseFamily = _context.CourseFamilies.FirstOrDefault(cf => cf.Code == code);
        if (courseFamily == null)
        {
            return NotFound(CustomResponse.NotFound("Course family not found"));
        }

        return Ok(CustomResponse.Ok("Get course family success", new CourseFamilyResponse()
        {
            Code = courseFamily.Code, Name = courseFamily.Name, PublishedYear = courseFamily.PublishedYear,
            IsActive = courseFamily.IsActive, CreatedAt = courseFamily.CreatedAt, UpdatedAt = courseFamily.UpdatedAt
        }));
    }

    // create course family
    [HttpPost]
    [Route("api/course-families")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult CreateCourseFamily([FromBody] CreateCourseFamilyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name.Trim()) || string.IsNullOrWhiteSpace(request.Code.Trim()))
        {
            var error = ErrorDescription.Error["E1007"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // name format
        if (Regex.IsMatch(request.Name.Trim(), StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1008"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Name.Trim().Length > 255)
        {
            var error = ErrorDescription.Error["E1009"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // code format
        if (IsCourseFamilyExists(request))
        {
            var error = ErrorDescription.Error["E1013"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }
        
        if (Regex.IsMatch(request.Code.Trim(), StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1010"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Code.Trim().Length > 100)
        {
            var error = ErrorDescription.Error["E1011"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.PublishedYear <= 0)
        {
            var error = ErrorDescription.Error["E1012"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var courseFamily = new CourseFamily()
        {
            Code = request.Code.Trim(), Name = request.Name.Trim(), PublishedYear = request.PublishedYear,
            IsActive = request.IsActive,
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now
        };
        _context.CourseFamilies.Add(courseFamily);
        _context.SaveChanges();
        
        return Ok(CustomResponse.Ok("Create course family success", courseFamily));
    }
    
    // update course family
    [HttpPut]
    [Route("api/course-families/{code}")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult UpdateCourseFamily(string code, [FromBody] UpdateCourseFamilyRequest request)
    {
        var courseFamily = _context.CourseFamilies.FirstOrDefault(cf => cf.Code == code);
        if (courseFamily == null)
        {
            return NotFound(CustomResponse.NotFound("Course family not found"));
        }

        if (string.IsNullOrWhiteSpace(request.Name.Trim()))
        {
            var error = ErrorDescription.Error["E1007"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // name format
        if (Regex.IsMatch(request.Name.Trim(), StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1008"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Name.Trim().Length > 255)
        {
            var error = ErrorDescription.Error["E1009"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.PublishedYear <= 0)
        {
            var error = ErrorDescription.Error["E1012"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }
        
        courseFamily.Name = request.Name.Trim();
        courseFamily.PublishedYear = request.PublishedYear;
        courseFamily.IsActive = request.IsActive;
        courseFamily.UpdatedAt = DateTime.Now;
        _context.SaveChanges();
        
        return Ok(CustomResponse.Ok("Update course family success", courseFamily));
    }
    
    // is course family exist
    private bool IsCourseFamilyExists(CreateCourseFamilyRequest request)
    {
        return _context.CourseFamilies.Any(cf => cf.Code == request.Code.Trim());
    }
}