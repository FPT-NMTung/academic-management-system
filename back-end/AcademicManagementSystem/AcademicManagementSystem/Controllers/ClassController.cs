using System.Text;
using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.CenterController;
using AcademicManagementSystem.Models.ClassController;
using AcademicManagementSystem.Models.ClassDaysController;
using AcademicManagementSystem.Models.ClassStatusController;
using AcademicManagementSystem.Models.CourseFamilyController;
using AcademicManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class ClassController : ControllerBase
{
    private readonly AmsContext _context;
    private readonly User _user;
    private const int NotScheduleYet = 5;

    public ClassController(AmsContext context, IUserService userService)
    {
        _context = context;
        var userId = Convert.ToInt32(userService.GetUserId());
        _user = _context.Users.FirstOrDefault(u => u.Id == userId)!;
    }

    // get all classes
    [HttpGet]
    [Route("api/classes")]
    [Authorize(Roles = "sro")]
    public IActionResult GetClassesByCurrentSroCenter()
    {
        var classes = GetAllClassesInThisCenterByContext().ToList();
        return Ok(CustomResponse.Ok("Classes retrieved successfully", classes));
    }

    // get class by id
    [HttpGet]
    [Route("api/classes/{id:int}")]
    [Authorize(Roles = "sro")]
    public IActionResult GetClassById(int id)
    {
        var classResponse = GetAllClassesInThisCenterByContext().FirstOrDefault(c => c.Id == id);
        if (classResponse == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found in this center"));
        }

        return Ok(CustomResponse.Ok("Get class by id successfully", classResponse));
    }

    /*
     * sroName is firstName
     */
    [HttpGet]
    [Route("api/classes/search")]
    [Authorize(Roles = "sro")]
    public IActionResult SearchClasses([FromQuery] int? classDaysId, [FromQuery] int? classStatusId,
        [FromQuery] string? className, [FromQuery] string? courseFamilyCode, [FromQuery] string? sroName)
    {
        var sClassName = className == null ? string.Empty : RemoveDiacritics(className.Trim().ToLower());
        var sCourseFamilyCode = courseFamilyCode == null
            ? string.Empty
            : RemoveDiacritics(courseFamilyCode.Trim().ToLower());
        var sSroName = sroName == null ? string.Empty : RemoveDiacritics(sroName.Trim().ToLower());

        var allClasses = GetAllClassesInThisCenterByContext();

        //if user didn't input any search condition, return all classes
        if (classDaysId == null && classStatusId == null && sClassName == string.Empty
            && sCourseFamilyCode == string.Empty && sSroName == string.Empty)
        {
            return Ok(CustomResponse.Ok("Search classes successfully", allClasses));
        }

        var classesResponse = new List<ClassResponse>();
        foreach (var c in allClasses)
        {
            var s1 = RemoveDiacritics(c.Name!.ToLower());
            var s2 = RemoveDiacritics(c.CourseFamilyCode!.ToLower());
            var s3 = RemoveDiacritics(c.SroFirstName!.ToLower());
            var s4 = RemoveDiacritics(c.SroLastName!.ToLower());

            var fullName = s3 + " " + s4;

            if (s1.Contains(sClassName)
                && s2.Contains(sCourseFamilyCode)
                && fullName.Contains(sSroName)
                && (classDaysId == null || c.ClassDaysId == classDaysId)
                && (classStatusId == null || c.ClassStatusId == classStatusId))
            {
                classesResponse.Add(c);
            }
        }

        return Ok(CustomResponse.Ok("Search classes successfully", classesResponse));
    }

    // create new class
    [HttpPost]
    [Route("api/classes")]
    [Authorize(Roles = "sro")]
    public IActionResult CreateClass([FromBody] CreateClassRequest request)
    {
        request.Name = Regex.Replace(request.Name, StringConstant.RegexWhiteSpaces, " ").Trim();

        var errorCode = GetCodeIfOccuredErrorWhenCreate(request);

        if (errorCode != null)
        {
            var error = ErrorDescription.Error[errorCode];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var newClass = new Class()
        {
            CenterId = _user.CenterId,
            CourseFamilyCode = request.CourseFamilyCode,
            ClassDaysId = request.ClassDaysId,
            ClassStatusId = NotScheduleYet,
            SroId = _user.Id,
            Name = request.Name,
            StartDate = request.StartDate,
            CompletionDate = request.CompletionDate,
            GraduationDate = request.GraduationDate,
            ClassHourStart = request.ClassHourStart,
            ClassHourEnd = request.ClassHourEnd,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Classes.Add(newClass);
        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0071"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }
        catch (Exception e)
        {
            return BadRequest(CustomResponse.BadRequest(e.Message, e.GetType().FullName!));
        }

        // get new class by id
        var classResponse = GetAllClassesInThisCenterByContext().FirstOrDefault(c => c.Id == newClass.Id);

        if (classResponse == null)
        {
            return BadRequest(CustomResponse.BadRequest("Cannot find created class", "error-not-found"));
        }

        return Ok(CustomResponse.Ok("Create class successfully", classResponse));
    }

    // update class
    [HttpPut]
    [Route("api/classes/{classId:int}")]
    [Authorize(Roles = "sro")]
    public IActionResult UpdateClass(int classId, [FromBody] UpdateClassRequest request)
    {
        request.Name = Regex.Replace(request.Name, StringConstant.RegexWhiteSpaces, " ").Trim();

        var errorCode = GetCodeIfOccuredErrorWhenUpdate(classId, request);

        if (errorCode != null)
        {
            var error = ErrorDescription.Error[errorCode];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var classToUpdate = _context.Classes.FirstOrDefault(c => c.Id == classId && c.CenterId == _user.CenterId);

        if (classToUpdate == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found in this center"));
        }

        classToUpdate.CourseFamilyCode = request.CourseFamilyCode;
        classToUpdate.ClassDaysId = request.ClassDaysId;
        classToUpdate.ClassStatusId = request.ClassStatusId;
        classToUpdate.Name = request.Name;
        classToUpdate.StartDate = request.StartDate;
        classToUpdate.CompletionDate = request.CompletionDate;
        classToUpdate.GraduationDate = request.GraduationDate;
        classToUpdate.ClassHourStart = request.ClassHourStart;
        classToUpdate.ClassHourEnd = request.ClassHourEnd;
        classToUpdate.UpdatedAt = DateTime.Now;

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0071"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }
        catch (Exception e)
        {
            return BadRequest(CustomResponse.BadRequest(e.GetType().FullName!, e.Message));
        }

        // get updated class by id
        var classResponse = GetAllClassesInThisCenterByContext().FirstOrDefault(c => c.Id == classToUpdate.Id);

        if (classResponse == null)
        {
            return BadRequest(CustomResponse.BadRequest("Cannot find updated class", "error-not-found"));
        }

        return Ok(CustomResponse.Ok("Update class successfully", classResponse));
    }

    private string? GetCodeIfOccuredErrorWhenCreate(CreateClassRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return "E0068";
        }

        // allow special characters: ()-_
        if (Regex.IsMatch(request.Name, StringConstant.RegexSpecialCharactersNotAllowForClassName))
        {
            return "E0069";
        }

        if (request.ClassHourStart >= request.ClassHourEnd)
        {
            return "E0072";
        }

        return IsClassExist(request.Name, _user.CenterId, false, 0) ? "E0070" : null;
    }

    private string? GetCodeIfOccuredErrorWhenUpdate(int classId, UpdateClassRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return "E0068";
        }

        // allow special characters: ()-_
        if (Regex.IsMatch(request.Name, StringConstant.RegexSpecialCharactersNotAllowForClassName))
        {
            return "E0069";
        }

        if (request.ClassHourStart >= request.ClassHourEnd)
        {
            return "E0072";
        }

        return IsClassExist(request.Name, _user.CenterId, true, classId) ? "E0070" : null;
    }

    private bool IsClassExist(string className, int centerId, bool isUpdate, int classId)
    {
        return isUpdate
            ? _context.Classes.Any(c =>
                c.Name.ToLower().Equals(className.ToLower()) && c.CenterId == centerId && c.Id != classId)
            : _context.Classes.Any(c => c.Name.ToLower().Equals(className.ToLower()) && c.CenterId == centerId);
    }

    private IQueryable<ClassResponse> GetAllClassesInThisCenterByContext()
    {
        return _context.Classes.Include(c => c.Center)
            .Include(c => c.ClassDays)
            .Include(c => c.ClassStatus)
            .Include(c => c.Center.Province)
            .Include(c => c.Center.District)
            .Include(c => c.Center.Ward)
            .Include(c => c.CourseFamily)
            .Include(c => c.Sro)
            .ThenInclude(s => s.User)
            .Select(c => new ClassResponse()
            {
                Id = c.Id,
                Name = c.Name,
                CenterId = c.CenterId,
                CourseFamilyCode = c.CourseFamilyCode,
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
                CourseFamily = new CourseFamilyResponse()
                {
                    Code = c.CourseFamily.Code,
                    Name = c.CourseFamily.Name,
                    IsActive = c.CourseFamily.IsActive,
                    PublishedYear = c.CourseFamily.PublishedYear,
                    CreatedAt = c.CourseFamily.CreatedAt,
                    UpdatedAt = c.CourseFamily.UpdatedAt
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
            }).Where(c => c.CenterId == _user.CenterId);
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