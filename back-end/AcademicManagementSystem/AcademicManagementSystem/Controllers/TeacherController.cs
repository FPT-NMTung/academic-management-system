using System.Text;
using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.GenderController;
using AcademicManagementSystem.Models.RoleController;
using AcademicManagementSystem.Models.TeacherTypeController;
using AcademicManagementSystem.Models.UserController.TeacherController;
using AcademicManagementSystem.Models.WorkingTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class TeacherController : ControllerBase
{
    private readonly AmsContext _context;
    private const int RoleIdTeacher = 3;
    private const string RegexSpecialCharacters = StringConstant.RegexSpecialCharsNotAllowForPersonName;

    public TeacherController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/teachers")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetAllTeachers()
    {
        var teachers = GetAllUserRoleTeacher();
        return Ok(CustomResponse.Ok("Get all teachers successfully", teachers));
    }

    // get teachers by center id
    [HttpGet]
    [Route("api/centers/{centerId}/teachers")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetTeachersByCenterId(int centerId)
    {
        var teachers = GetAllUserRoleTeacher().Where(t => t.CenterId == centerId);
        return Ok(CustomResponse.Ok("Get teachers by centerId successfully", teachers));
    }

    // get teacher by id
    [HttpGet]
    [Route("api/teachers/{teacherId}")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetTeacherById(int teacherId)
    {
        var teacher = GetAllUserRoleTeacher().FirstOrDefault(t => t.UserId == teacherId);
        if (teacher == null)
        {
            return NotFound(CustomResponse.NotFound("Teacher not found"));
        }

        return Ok(CustomResponse.Ok("Get teacher by id successfully", teacher));
    }

    // get teachers by teacher type id
    [HttpGet]
    [Route("api/teacher-types/teachers")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult FilterTeachersByTeacherTypeId([FromQuery] int teacherTypeId)
    {
        var teachers = GetAllUserRoleTeacher().Where(t => t.TeacherType.Id == teacherTypeId);
        return Ok(CustomResponse.Ok("get teachers by teacher type id successfully", teachers));
    }

    // get teachers by working time id
    [HttpGet]
    [Route("api/working-times/teachers")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetTeachersByWorkingTimeId([FromQuery] int workingTimeId)
    {
        var teachers = GetAllUserRoleTeacher().Where(t => t.WorkingTime.Id == workingTimeId);
        return Ok(CustomResponse.Ok("get teachers by working time id successfully", teachers));
    }

    // search teacher by by firstName, lastName, nickname, mobilePhone, email, emailOrganization
    [HttpGet]
    [Route("api/teachers/search")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult SearchTeachers([FromQuery] string? firstName, [FromQuery] string? lastName,
        [FromQuery] string? nickname, [FromQuery] string? mobilePhone, [FromQuery] string? email,
        [FromQuery] string? emailOrganization)
    {
        var sFirstName = firstName == null ? string.Empty : RemoveDiacritics(firstName.Trim().ToLower());
        var sLastName = lastName == null ? string.Empty : RemoveDiacritics(lastName.Trim().ToLower());
        var sNickname = nickname == null ? string.Empty : RemoveDiacritics(nickname.Trim().ToLower());
        var sMobilePhone = mobilePhone == null ? string.Empty : RemoveDiacritics(mobilePhone.Trim().ToLower());
        var sEmail = email == null ? string.Empty : RemoveDiacritics(email.Trim().ToLower());
        var sEmailOrganization = emailOrganization == null
            ? string.Empty
            : RemoveDiacritics(emailOrganization.Trim().ToLower());

        //check empty
        if (sFirstName == string.Empty && sLastName == string.Empty
                                       && sNickname == string.Empty && sMobilePhone == string.Empty
                                       && sEmail == string.Empty && sEmailOrganization == string.Empty)
        {
            var error = ErrorDescription.Error["E0039"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var listSro = GetAllUserRoleTeacher();

        var sroResponse = new List<TeacherResponse>();
        foreach (var s in listSro)
        {
            var s1 = RemoveDiacritics(s.FirstName!.ToLower());
            var s2 = RemoveDiacritics(s.LastName!.ToLower());
            s.Nickname ??= string.Empty;
            var s3 = RemoveDiacritics(s.Nickname.ToLower());
            var s4 = RemoveDiacritics(s.MobilePhone!.ToLower());
            var s5 = RemoveDiacritics(s.Email!.ToLower());
            var s6 = RemoveDiacritics(s.EmailOrganization!.ToLower());

            if (s1.Contains(sFirstName)
                && s2.Contains(sLastName)
                && s3.Contains(sNickname)
                && s4.Contains(sMobilePhone)
                && s5.Contains(sEmail)
                && s6.Contains(sEmailOrganization))
            {
                sroResponse.Add(s);
            }
        }

        return Ok(CustomResponse.Ok("Search Teacher successfully", sroResponse));
    }

    private IEnumerable<TeacherResponse> GetAllUserRoleTeacher()
    {
        var allUserRoleTeacher = _context.Users
            .Include(u => u.Province)
            .Include(u => u.District)
            .Include(u => u.Ward)
            .Include(u => u.Role)
            .Include(u => u.Gender)
            .Include(u => u.Center)
            .Include(u => u.Teacher)
            .ThenInclude(t => t.TeacherType)
            .Include(u => u.Teacher)
            .ThenInclude(t => t.WorkingTime)
            .Where(u => u.RoleId == RoleIdTeacher)
            .Select(u => new TeacherResponse()
            {
                UserId = u.Id,
                Role = new RoleResponse()
                {
                    Id = u.Role.Id,
                    Value = u.Role.Value
                },
                FirstName = u.FirstName,
                LastName = u.LastName,
                MobilePhone = u.MobilePhone,
                Email = u.Email,
                EmailOrganization = u.EmailOrganization,
                Avatar = u.Avatar,
                Province = new ProvinceResponse()
                {
                    Id = u.Province.Id,
                    Code = u.Province.Code,
                    Name = u.Province.Name
                },
                District = new DistrictResponse()
                {
                    Id = u.District.Id,
                    Name = u.District.Name,
                    Prefix = u.District.Prefix
                },
                Ward = new WardResponse()
                {
                    Id = u.Ward.Id,
                    Name = u.Ward.Name,
                    Prefix = u.Ward.Prefix
                },
                Gender = new GenderResponse()
                {
                    Id = u.Gender.Id,
                    Value = u.Gender.Value
                },
                Birthday = u.Birthday,
                CenterId = u.CenterId,
                CenterName = u.Center.Name,
                CitizenIdentityCardNo = u.CitizenIdentityCardNo,
                CitizenIdentityCardPublishedDate = u.CitizenIdentityCardPublishedDate,
                CitizenIdentityCardPublishedPlace = u.CitizenIdentityCardPublishedPlace,
                TeacherType = new TeacherTypeResponse()
                {
                    Id = u.Teacher.TeacherType.Id,
                    Value = u.Teacher.TeacherType.Value
                },
                WorkingTime = new WorkingTimeResponse()
                {
                    Id = u.Teacher.WorkingTime.Id,
                    Value = u.Teacher.WorkingTime.Value
                },
                Nickname = u.Teacher.Nickname,
                CompanyAddress = u.Teacher.CompanyAddress,
                StartWorkingDate = u.Teacher.StartWorkingDate,
                Salary = u.Teacher.Salary,
                TaxCode = u.Teacher.TaxCode,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            });
        return allUserRoleTeacher;
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