using System.Text;
using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.CenterController;
using AcademicManagementSystem.Models.ClassController;
using AcademicManagementSystem.Models.ClassDaysController;
using AcademicManagementSystem.Models.ClassStatusController;
using AcademicManagementSystem.Models.CourseController;
using AcademicManagementSystem.Models.CourseFamilyController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class ClassController : ControllerBase
{
    private readonly AmsContext _context;

    public ClassController(AmsContext context)
    {
        _context = context;
    }

    // get all classes
    [HttpGet]
    [Route("api/classes")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult GetClasses()
    {
        var classes = GetAllClassesByContext().ToList();
        return Ok(CustomResponse.Ok("Classes retrieved successfully", classes));
    }

    // get class by id
    [HttpGet]
    [Route("api/classes/{id:int}")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult GetClassById(int id)
    {
        var classResponse = GetAllClassesByContext().FirstOrDefault(c => c.Id == id);
        if (classResponse == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }

        return Ok(CustomResponse.Ok("Get Class by id successfully", classResponse));
    }

    /*
     * sroName is firstName
     */
    [HttpGet]
    [Route("api/classes/search")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult SearchClasses([FromQuery] int? classDaysId, [FromQuery] int? classStatusId,
        [FromQuery] string? className, [FromQuery] string? courseCode, [FromQuery] int? centerId,
        [FromQuery] string? sroName)
    {
        var sClassName = className == null ? string.Empty : RemoveDiacritics(className.Trim().ToLower());
        var sCourseCode = courseCode == null ? string.Empty : RemoveDiacritics(courseCode.Trim().ToLower());
        var sSroName = sroName == null ? string.Empty : RemoveDiacritics(sroName.Trim().ToLower());

        var allClasses = GetAllClassesByContext();

        //if user didn't input any search condition, return all classes
        if (classDaysId == null && classStatusId == null && sClassName == string.Empty && sCourseCode == string.Empty
            && centerId == null && sSroName == string.Empty)
        {
            return Ok(CustomResponse.Ok("Search classes successfully", allClasses));
        }

        var classesResponse = new List<ClassResponse>();
        foreach (var c in allClasses)
        {
            var s1 = RemoveDiacritics(c.Name!.ToLower());
            var s2 = RemoveDiacritics(c.CourseCode!.ToLower());
            var s3 = RemoveDiacritics(c.SroFirstName!.ToLower());

            if (s1.Contains(sClassName)
                && s2.Contains(sCourseCode)
                && s3.Contains(sSroName)
                && (classDaysId == null || c.ClassDaysId == classDaysId)
                && (classStatusId == null || c.ClassStatusId == classStatusId)
                && (centerId == null || c.CenterId == centerId))
            {
                classesResponse.Add(c);
            }
        }

        return Ok(CustomResponse.Ok("Search classes successfully", classesResponse));
    }

    private IQueryable<ClassResponse> GetAllClassesByContext()
    {
        return _context.Classes.Include(c => c.Center)
            .Include(c => c.Course)
            .Include(c => c.ClassDays)
            .Include(c => c.ClassStatus)
            .Include(c => c.Center.Province)
            .Include(c => c.Center.District)
            .Include(c => c.Center.Ward)
            .Include(c => c.Course.CourseFamily)
            .Include(c => c.Sro)
            .ThenInclude(s => s.User)
            .Select(c => new ClassResponse()
            {
                Id = c.Id,
                Name = c.Name,
                CenterId = c.CenterId,
                CourseCode = c.CourseCode,
                ClassDaysId = c.ClassDaysId,
                ClassStatusId = c.ClassStatusId,
                StartDate = c.StartDate,
                CompletionDate = c.CompletionDate,
                GraduationDate = c.GraduationDate,
                ClassHourStart = c.ClassHourStart,
                ClassHourEnd = c.ClassHourEnd,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                Center = new CenterResponse()
                {
                    Id = c.Center.Id,
                    Name = c.Center.Name,
                    CreatedAt = c.Center.CreatedAt,
                    UpdatedAt = c.Center.UpdatedAt,
                    Province = new ProvinceResponse()
                    {
                        Id = c.Center.Province.Id,
                        Code = c.Center.Province.Code,
                        Name = c.Center.Province.Name,
                    },
                    District = new DistrictResponse()
                    {
                        Id = c.Center.District.Id,
                        Name = c.Center.District.Name,
                        Prefix = c.Center.District.Prefix
                    },
                    Ward = new WardResponse()
                    {
                        Id = c.Center.Ward.Id,
                        Name = c.Center.Ward.Name,
                        Prefix = c.Center.Ward.Prefix
                    }
                },
                Course = new CourseResponse()
                {
                    Code = c.Course.Code,
                    Name = c.Course.Name,
                    IsActive = c.Course.IsActive,
                    SemesterCount = c.Course.SemesterCount,
                    CourseFamilyCode = c.Course.CourseFamilyCode,
                    CreatedAt = c.Course.CreatedAt,
                    UpdatedAt = c.Course.UpdatedAt,
                    CourseFamily = new CourseFamilyResponse()
                    {
                        Code = c.Course.CourseFamily.Code,
                        Name = c.Course.CourseFamily.Name,
                        IsActive = c.Course.CourseFamily.IsActive,
                        PublishedYear = c.Course.CourseFamily.PublishedYear,
                        CreatedAt = c.Course.CourseFamily.CreatedAt,
                        UpdatedAt = c.Course.CourseFamily.UpdatedAt
                    }
                },
                ClassDays = new ClassDaysResponse()
                {
                    Id = c.ClassDays.Id,
                    Value = c.ClassDays.Value
                },
                ClassStatus = new ClassStatusResponse()
                {
                    Id = c.ClassStatus.Id,
                    Value = c.ClassStatus.Value
                },
                SroId = c.Sro.UserId,
                SroFirstName = c.Sro.User.FirstName,
                SroLastName = c.Sro.User.LastName
            });
    }

    private static string RemoveDiacritics(string text)
    {
        //regex pattern to remove diacritics
        const string pattern = "\\p{IsCombiningDiacriticalMarks}+";

        var normalizedStr = text.Normalize(NormalizationForm.FormD);

        return Regex.Replace(normalizedStr, pattern, string.Empty)
            .Replace('\u0111', 'd')
            .Replace('\u0110', 'D');
    }
}