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

    // get all course families
    [HttpGet]
    [Route("api/course-families")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult GetCourseFamilies()
    {
        var courseFamilies = _context.CourseFamilies
            .Include(cf => cf.Courses)
            .Select(cf => new CourseFamilyResponse()
            {
                Code = cf.Code, Name = cf.Name, PublishedYear = cf.PublishedYear, IsActive = cf.IsActive,
                CreatedAt = cf.CreatedAt, UpdatedAt = cf.UpdatedAt
            }).ToList();
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
    [Authorize(Roles = "admin")]
    public IActionResult CreateCourseFamily([FromBody] CreateCourseFamilyRequest request)
    {
        request.Code = request.Code.ToUpper().Trim();
        request.Name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(request.Name.Trim()) || string.IsNullOrWhiteSpace(request.Code))
        {
            var error = ErrorDescription.Error["E1007"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // code format
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

        if (IsCourseFamilyCodeExists(request.Code))
        {
            var error = ErrorDescription.Error["E1013"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsCourseFamilyNameExists(request.Name))
        {
            var error = ErrorDescription.Error["E1055"];
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
    [Authorize(Roles = "admin")]
    public IActionResult UpdateCourseFamily(string code, [FromBody] UpdateCourseFamilyRequest request)
    {
        request.Name = request.Name?.Trim();

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

        if (IsCourseFamilyNameWithDifferentCodeExists(code.Trim(), request.Name))
        {
            var error = ErrorDescription.Error["E1056"];
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

    [HttpGet]
    [Route("api/course-families/{code}/can-delete")]
    [Authorize(Roles = "admin")]
    public IActionResult CanDeleteCourseFamily(string code)
    {
        var courseFamily = _context.CourseFamilies.FirstOrDefault(cf => cf.Code == code.ToUpper().Trim());
        if (courseFamily == null)
        {
            return NotFound(CustomResponse.NotFound("Course family not found"));
        }

        var canDelete = CanDelete(code);

        return Ok(CustomResponse.Ok("Can delete course family", new CheckCourseFamilyCanDeleteResponse()
        {
            CanDelete = canDelete
        }));
    }

    // change active status
    [HttpPatch]
    [Route("api/course-families/{code}")]
    [Authorize(Roles = "admin")]
    public IActionResult ChangeActiveStatusCourseFamily(string code)
    {
        var selectedCourseFamily = _context.CourseFamilies.FirstOrDefault(cf => cf.Code == code.Trim());

        if (selectedCourseFamily == null)
            return NotFound(CustomResponse.NotFound("Course family not found"));

        selectedCourseFamily.IsActive = !selectedCourseFamily.IsActive;
        selectedCourseFamily.UpdatedAt = DateTime.Now;

        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E2063"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var courseFamilyResponse = GetCourseFamilyResponse(selectedCourseFamily);
        return Ok(CustomResponse.Ok("Change active status course family success", courseFamilyResponse));
    }

    // delete course family
    [HttpDelete]
    [Route("api/course-families/{code}")]
    [Authorize(Roles = "admin")]
    public IActionResult DeleteCourseFamily(string code)
    {
        try
        {
            var courseFamily = _context.CourseFamilies.FirstOrDefault(cf => cf.Code == code.Trim());
            if (courseFamily == null)
            {
                return NotFound(CustomResponse.NotFound("Not Found Course Family"));
            }

            _context.CourseFamilies.Remove(courseFamily);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1121"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Course family deleted successful", null!));
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

    private bool CanDelete(string code)
    {
        var selectCourseFamily = _context.CourseFamilies
            .Include(cf => cf.Courses)
            .Include(cf => cf.Classes)
            .FirstOrDefault(cf => cf.Code == code);

        if (selectCourseFamily == null)
        {
            return false;
        }

        if (selectCourseFamily.Courses.Count > 0 || selectCourseFamily.Classes.Count > 0)
        {
            return false;
        }

        return true;
    }

    // is course family exists
    private bool IsCourseFamilyCodeExists(string code)
    {
        return _context.CourseFamilies.Any(cf => cf.Code == code);
    }

    // is course family name exists
    private bool IsCourseFamilyNameExists(string name)
    {
        return _context.CourseFamilies.Any(cf => cf.Name == name);
    }

    // is course family name with different code existed
    private bool IsCourseFamilyNameWithDifferentCodeExists(string code, string name)
    {
        return _context.CourseFamilies.Any(cf => cf.Code != code && cf.Name == name);
    }
}