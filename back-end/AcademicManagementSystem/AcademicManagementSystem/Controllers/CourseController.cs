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
        var courses = _context.Courses
            .Include(c => c.CourseFamily)
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
            }).ToList();
        return Ok(CustomResponse.Ok("Get Courses Successfully", courses));
    }

    // get course by code
    [HttpGet]
    [Route("api/courses/{code}")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult GetCourseByCode(string code)
    {
        var course = _context.Courses
            .Include(c => c.CourseFamily)
            .FirstOrDefault(c => c.Code == code.ToUpper().Trim());
        if (course == null)
        {
            return NotFound(CustomResponse.NotFound("Not Found Course"));
        }

        var courseResponse = GetCourseResponse(course);
        return Ok(CustomResponse.Ok("Get Course Successfully", courseResponse));
    }

    // create course
    [HttpPost]
    [Route("api/courses")]
    [Authorize(Roles = "admin")]
    public IActionResult CreateCourse([FromBody] CreateCourseRequest request)
    {
        request.Name = request.Name.Trim();
        request.Code = request.Code.ToUpper().Trim();
        request.CourseFamilyCode = request.CourseFamilyCode.ToUpper().Trim();

        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Code) ||
            string.IsNullOrWhiteSpace(request.CourseFamilyCode))
        {
            var error = ErrorDescription.Error["E1007"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // name format
        request.Name = Regex.Replace(request.Name, StringConstant.RegexWhiteSpaces, " ");
        request.Name = request.Name.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.Name, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
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
        request.Code = Regex.Replace(request.Code, StringConstant.RegexWhiteSpaces, " ");
        request.Code = request.Code.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.Code, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1018"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Code.Length > 100)
        {
            var error = ErrorDescription.Error["E1019"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsCourseCodeExists(request.Code))
        {
            var error = ErrorDescription.Error["E1014"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsCourseNameExists(request.Name))
        {
            var error = ErrorDescription.Error["E1057"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.SemesterCount is < 1 or > 4)
        {
            var error = ErrorDescription.Error["E1020"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var courseFamily =
            _context.CourseFamilies.FirstOrDefault(cf => cf.Code == request.CourseFamilyCode);
        if (courseFamily == null)
        {
            var error = ErrorDescription.Error["E1015"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var course = new Course()
        {
            Code = request.Code, CourseFamilyCode = request.CourseFamilyCode, Name = request.Name,
            SemesterCount = request.SemesterCount, IsActive = request.IsActive, CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        try
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            return BadRequest(CustomResponse.BadRequest(e.Message, e.GetType().ToString()));
        }

        var courseResponse = GetCourseResponse(course);
        return Ok(CustomResponse.Ok("Course Created Successfully", courseResponse));
    }

    // update course
    [HttpPut]
    [Route("api/courses/{code}")]
    [Authorize(Roles = "admin")]
    public IActionResult UpdateCourse(string code, [FromBody] UpdateCourseRequest request)
    {
        request.Name = request.Name.Trim();
        request.CourseFamilyCode = request.CourseFamilyCode.ToUpper().Trim();

        var course = _context.Courses.FirstOrDefault(c => c.Code == code.Trim());
        if (course == null)
        {
            return NotFound(CustomResponse.NotFound("Not Found Course"));
        }

        // name format
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.CourseFamilyCode))
        {
            var error = ErrorDescription.Error["E1007"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.Name = Regex.Replace(request.Name, StringConstant.RegexWhiteSpaces, " ");
        request.Name = request.Name.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.Name, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1016"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsCourseNameWithDifferentCodeExists(request.Name, code))
        {
            var error = ErrorDescription.Error["E1058"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Name.Length > 255)
        {
            var error = ErrorDescription.Error["E1017"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var courseFamilyCode =
            _context.CourseFamilies.FirstOrDefault(cf => cf.Code == request.CourseFamilyCode);
        if (courseFamilyCode == null)
        {
            var error = ErrorDescription.Error["E1015"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.SemesterCount is < 1 or > 4)
        {
            var error = ErrorDescription.Error["E1020"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        try
        {
            course.CourseFamilyCode = request.CourseFamilyCode;
            course.Name = request.Name;
            course.SemesterCount = request.SemesterCount;
            course.IsActive = request.IsActive;
            course.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
        }
        catch (Exception e)
        {
            return BadRequest(CustomResponse.BadRequest(e.Message, e.GetType().ToString()));
        }

        var courseResponse = GetCourseResponse(course);
        return Ok(CustomResponse.Ok("Course Updated Successfully", courseResponse));
    }

    // change active status
    [HttpPatch]
    [Route("api/courses/{code}")]
    [Authorize(Roles = "admin")]
    public IActionResult ChangeActiveStatusCourse(string code)
    {
        var selectedCourse = _context.Courses
            .Include(c => c.CourseFamily)
            .FirstOrDefault(c => c.Code == code.Trim());

        if (selectedCourse == null)
            return NotFound(CustomResponse.NotFound("Course not found"));

        selectedCourse.IsActive = !selectedCourse.IsActive;
        selectedCourse.UpdatedAt = DateTime.Now;

        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1117"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var courseResponse = GetCourseResponse(selectedCourse);
        return Ok(CustomResponse.Ok("Active status course changed successfully", courseResponse));
    }

    // Can delete course
    [HttpGet]
    [Route("api/courses/{code}/can-delete")]
    [Authorize(Roles = "admin")]
    public IActionResult CanDeleteCourse(string code)
    {
        var course = _context.Courses.FirstOrDefault(c => c.Code == code.ToUpper().Trim());
        if (course == null)
        {
            return NotFound(CustomResponse.NotFound("Not Found Course"));
        }

        var canDelete = CanDelete(code);

        return Ok(CustomResponse.Ok("Can delete course", new CheckCourseCanDeleteResponse()
        {
            CanDelete = canDelete
        }));
    }

    // delete course
    [HttpDelete]
    [Route("api/courses/{code}")]
    [Authorize(Roles = "admin")]
    public IActionResult DeleteCourse(string code)
    {
        try
        {
            var course = _context.Courses.FirstOrDefault(c => c.Code == code.Trim());
            if (course == null)
            {
                return NotFound(CustomResponse.NotFound("Not Found Course"));
            }

            _context.Courses.Remove(course);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1120"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Course Deleted Successfully", null!));
    }

    private static CourseResponse GetCourseResponse(Course course)
    {
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
        return courseResponse;
    }

    private bool CanDelete(string code)
    {
        var selectCourse = _context.Courses
            .Include(c => c.CoursesModulesSemesters)
            .Include(c => c.Students)
            .FirstOrDefault(c => c.Code == code);

        if (selectCourse == null)
        {
            return false;
        }

        return selectCourse.CoursesModulesSemesters.Count <= 0 && selectCourse.Students.Count <= 0;
    }

    // is course code exists
    private bool IsCourseCodeExists(string code)
    {
        return _context.Courses.Any(c => c.Code == code.ToUpper().Trim());
    }

    // is course name exists
    private bool IsCourseNameExists(string name)
    {
        return _context.Courses.Any(c => c.Name == name.Trim());
    }

    // is course name with different code exists
    private bool IsCourseNameWithDifferentCodeExists(string name, string code)
    {
        return _context.Courses.Any(c => c.Name == name.Trim() && c.Code != code.ToUpper().Trim());
    }
}