using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.CourseFamilyController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        var courseFamilies = _context.CourseFamilies.Include(cf => cf.Courses).ToList()
            .Select(cf => new CourseFamilyResponse()
            {
                Code = cf.Code, Name = cf.Name, PublishedYear = cf.PublishedYear, IsActive = cf.IsActive,
                CreatedAt = cf.CreatedAt, UpdatedAt = cf.UpdatedAt
            });
        return Ok(CustomResponse.Ok("Get course families success", courseFamilies));
    }

    // get course family by code
    [HttpGet]
    [Route("api/course-families/{code}")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult GetCourseFamilyByCode(string code)
    {
        var courseFamily = _context.CourseFamilies.FirstOrDefault(cf => cf.Code == code.ToUpper().Trim());
        if (courseFamily == null)
        {
            return NotFound(CustomResponse.NotFound("Course family not found"));
        }

        var courseFamilyResponse = GetCourseFamilyResponse(courseFamily);
        return Ok(CustomResponse.Ok("Get course family success", courseFamilyResponse));
    }

    // create course family
    [HttpPost]
    [Route("api/course-families")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult CreateCourseFamily([FromBody] CreateCourseFamilyRequest request)
    {
        request.Code = request.Code.ToUpper().Trim();
        request.Name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(request.Name.Trim()) || string.IsNullOrWhiteSpace(request.Code))
        {
            var error = ErrorDescription.Error["E1007"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // name format
        request.Name = Regex.Replace(request.Name, StringConstant.RegexWhiteSpaces, " ");
        request.Name = request.Name.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.Name, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1008"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Name.Length > 255)
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

        request.Code = Regex.Replace(request.Code, StringConstant.RegexWhiteSpaces, " ");
        request.Code = request.Code.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.Code, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1010"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Code.Length > 100)
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
            Code = request.Code, Name = request.Name, PublishedYear = request.PublishedYear,
            IsActive = request.IsActive,
            CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now
        };
        try
        {
            _context.CourseFamilies.Add(courseFamily);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            return BadRequest(CustomResponse.BadRequest(e.Message, e.GetType().ToString()));
        }

        var courseFamilyResponse = GetCourseFamilyResponse(courseFamily);
        return Ok(CustomResponse.Ok("Create course family success", courseFamilyResponse));
    }

    // update course family
    [HttpPut]
    [Route("api/course-families/{code}")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult UpdateCourseFamily(string code, [FromBody] UpdateCourseFamilyRequest request)
    {
        request.Name = request.Name.Trim();

        var courseFamily = _context.CourseFamilies.FirstOrDefault(cf => cf.Code == code.Trim());
        if (courseFamily == null)
        {
            return NotFound(CustomResponse.NotFound("Course family not found"));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            var error = ErrorDescription.Error["E1007"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // name format
        request.Name = Regex.Replace(request.Name, StringConstant.RegexWhiteSpaces, " ");
        request.Name = request.Name.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.Name, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1008"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Name.Length > 255)
        {
            var error = ErrorDescription.Error["E1009"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.PublishedYear <= 0)
        {
            var error = ErrorDescription.Error["E1012"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        try
        {
            courseFamily.Name = request.Name;
            courseFamily.PublishedYear = request.PublishedYear;
            courseFamily.IsActive = request.IsActive;
            courseFamily.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            return BadRequest(CustomResponse.BadRequest(e.Message, e.GetType().ToString()));
        }

        var courseFamilyResponse = GetCourseFamilyResponse(courseFamily);
        return Ok(CustomResponse.Ok("Update course family success", courseFamilyResponse));
    }

    // delete course family
    [HttpDelete]
    [Route("api/course-families/{code}")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult DeleteCourseFamily(string code)
    {
        try
        {
            var courseFamily = _context.CourseFamilies.FirstOrDefault(cf => cf.Code == code.Trim());
            if (courseFamily == null)
            {
                return NotFound(CustomResponse.NotFound("Course family not found"));
            }

            _context.CourseFamilies.Remove(courseFamily);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            return BadRequest(CustomResponse.BadRequest(e.Message, e.GetType().ToString()));
        }

        return Ok(CustomResponse.Ok("Delete course family success", null!));
    }

    private static CourseFamilyResponse GetCourseFamilyResponse(CourseFamily courseFamily)
    {
        var courseFamilyResponse = new CourseFamilyResponse()
        {
            Code = courseFamily.Code, Name = courseFamily.Name, PublishedYear = courseFamily.PublishedYear,
            IsActive = courseFamily.IsActive, CreatedAt = courseFamily.CreatedAt, UpdatedAt = courseFamily.UpdatedAt
        };
        return courseFamilyResponse;
    }

    // is course family exists
    private bool IsCourseFamilyExists(CreateCourseFamilyRequest request)
    {
        return _context.CourseFamilies.Any(cf => cf.Code == request.Code.ToUpper().Trim());
    }
}