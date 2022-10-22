﻿using System.Text;
using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.GenderController;
using AcademicManagementSystem.Models.RoleController;
using AcademicManagementSystem.Models.TeacherSkillController.Skill;
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

        var teachers = GetAllUserRoleTeacher();

        // if user didn't input any search condition, return all teachers
        if (sFirstName == string.Empty && sLastName == string.Empty
                                       && sNickname == string.Empty && sMobilePhone == string.Empty
                                       && sEmail == string.Empty && sEmailOrganization == string.Empty)
        {
            return Ok(CustomResponse.Ok("Search teachers successfully", teachers));
        }

        var teacherResponses = new List<TeacherResponse>();
        foreach (var t in teachers)
        {
            var s1 = RemoveDiacritics(t.FirstName!.ToLower());
            var s2 = RemoveDiacritics(t.LastName!.ToLower());
            t.Nickname ??= string.Empty;
            var s3 = RemoveDiacritics(t.Nickname.ToLower());
            var s4 = RemoveDiacritics(t.MobilePhone!.ToLower());
            var s5 = RemoveDiacritics(t.Email!.ToLower());
            var s6 = RemoveDiacritics(t.EmailOrganization!.ToLower());

            if (s1.Contains(sFirstName)
                && s2.Contains(sLastName)
                && s3.Contains(sNickname)
                && s4.Contains(sMobilePhone)
                && s5.Contains(sEmail)
                && s6.Contains(sEmailOrganization))
            {
                teacherResponses.Add(t);
            }
        }

        return Ok(CustomResponse.Ok("Search Teacher successfully", teacherResponses));
    }

    // create teacher
    [HttpPost]
    [Route("api/teachers")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult CreateTeacher([FromBody] CreateTeacherRequest request)
    {
        if (CheckTeacherNameForCreate(request, out var badRequest)) return badRequest;

        if (CheckMobilePhoneForCreate(request, out var badRequestObjectResult)) return badRequestObjectResult;

        if (CheckEmailAndEmailOrganizeForCreate(request, out var teacher)) return teacher;

        if (IsCitizenIdentityCardNoExists(request.CitizenIdentityCardNo, false, 0))
        {
            var error = ErrorDescription.Error["E0053"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.CitizenIdentityCardNo, StringConstant.RegexCitizenIdCardNo))
        {
            var error = ErrorDescription.Error["E0045"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsTaxCodeExists(request.TaxCode, false, 0))
        {
            var error = ErrorDescription.Error["E0041"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.TaxCode, StringConstant.RegexTenDigits))
        {
            var error = ErrorDescription.Error["E0054"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var user = new User()
        {
            ProvinceId = request.ProvinceId,
            DistrictId = request.DistrictId,
            WardId = request.WardId,
            CenterId = request.CenterId,
            GenderId = request.GenderId,
            RoleId = RoleIdTeacher,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Avatar = request.Avatar,
            MobilePhone = request.MobilePhone,
            Email = request.Email,
            EmailOrganization = request.EmailOrganization,
            Birthday = request.Birthday,
            CitizenIdentityCardNo = request.CitizenIdentityCardNo,
            CitizenIdentityCardPublishedDate = request.CitizenIdentityCardPublishedDate,
            CitizenIdentityCardPublishedPlace = request.CitizenIdentityCardPublishedPlace,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Teacher = new Teacher()
            {
                TeacherTypeId = request.TeacherTypeId,
                WorkingTimeId = request.WorkingTimeId,
                Nickname = request.Nickname,
                CompanyAddress = request.CompanyAddress,
                StartWorkingDate = request.StartWorkingDate,
                Salary = request.Salary,
                TaxCode = request.TaxCode
            }
        };

        _context.Users.Add(user);

        if (request.Skills != null)
        {
            foreach (var sk in request.Skills)
            {
                sk.Name = Regex.Replace(sk.Name, StringConstant.RegexWhiteSpaces, " ");
                sk.Name = sk.Name.Replace(" ' ", "'").Trim();

                var existedSkill = _context.Skills.FirstOrDefault(s => s.Name.ToLower().Equals(sk.Name.ToLower()));

                if (existedSkill == null)
                {
                    var newSkill = new Skill() { Name = sk.Name };
                    // add both teacher to this skill and skill to this teacher
                    newSkill.Teachers.Add(user.Teacher);
                    _context.Skills.Add(newSkill);
                    user.Teacher.Skills.Add(newSkill);
                }
                else
                {
                    // add existed skill for this new teacher
                    user.Teacher.Skills.Add(existedSkill);
                }

                _context.Teachers.Add(user.Teacher);
            }
        }

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0049"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var sroResponse = GetAllUserRoleTeacher().FirstOrDefault(s => s.UserId == user.Id);
        return Ok(CustomResponse.Ok("Create Teacher successfully", sroResponse!));
    }

    //update teacher 
    [HttpPut]
    [Route("api/teachers/{id:int}")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult UpdateTeacher(int id, [FromBody] UpdateTeacherRequest request)
    {
        if (IsExistUserTeacher(id, out var user, out var teacher, out var actionResult)) return actionResult;

        if (CheckTeacherNameForUpdate(request, out var badRequest)) return badRequest;

        if (CheckMobilePhoneForUpdate(id, request, out var updateTeacher1)) return updateTeacher1;

        if (CheckEmailAndEmailOrganizationForUpdate(id, request, out var badRequestObjectResult))
            return badRequestObjectResult;

        if (IsCitizenIdentityCardNoExists(request.CitizenIdentityCardNo, true, id))
        {
            var error = ErrorDescription.Error["E0053"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.CitizenIdentityCardNo, StringConstant.RegexCitizenIdCardNo))
        {
            var error = ErrorDescription.Error["E0045"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsTaxCodeExists(request.TaxCode, true, id))
        {
            var error = ErrorDescription.Error["E0041"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.TaxCode, StringConstant.RegexTenDigits))
        {
            var error = ErrorDescription.Error["E0054"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (user != null)
        {
            user.ProvinceId = request.ProvinceId;
            user.DistrictId = request.DistrictId;
            user.WardId = request.WardId;
            user.GenderId = request.GenderId;
            user.RoleId = RoleIdTeacher;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.MobilePhone = request.MobilePhone;
            user.Email = request.Email;
            user.EmailOrganization = request.EmailOrganization;
            user.Birthday = request.Birthday;
            user.CitizenIdentityCardNo = request.CitizenIdentityCardNo;
            user.CitizenIdentityCardPublishedDate = request.CitizenIdentityCardPublishedDate;
            user.CitizenIdentityCardPublishedPlace = request.CitizenIdentityCardPublishedPlace;
            user.Teacher.TeacherTypeId = request.TeacherTypeId;
            user.Teacher.WorkingTimeId = request.WorkingTimeId;
            user.Teacher.Nickname = request.Nickname;
            user.Teacher.CompanyAddress = request.CompanyAddress;
            user.Teacher.StartWorkingDate = request.StartWorkingDate;
            user.Teacher.Salary = request.Salary;
            user.Teacher.TaxCode = request.TaxCode;
            user.UpdatedAt = DateTime.Now;

            if (request.Skills != null)
            {
                foreach (var sk in request.Skills)
                {
                    sk.Name = Regex.Replace(sk.Name, StringConstant.RegexWhiteSpaces, " ");
                    sk.Name = sk.Name.Replace(" ' ", "'").Trim();

                    var existedSkill = _context.Skills.FirstOrDefault(s => s.Name.ToLower().Equals(sk.Name.ToLower()));

                    if (existedSkill == null)
                    {
                        var newSkill = new Skill() { Name = sk.Name };
                        // add both teacher to this skill and skill to this teacher
                        newSkill.Teachers.Add(user.Teacher);
                        _context.Skills.Add(newSkill);
                        user.Teacher.Skills.Add(newSkill);
                    }
                }
            }
            else
            {
                
            }
        }

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0049"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var teacherResponse =
            GetAllUserRoleTeacher().FirstOrDefault(s => teacher != null && s.UserId == teacher.UserId);
        return Ok(CustomResponse.Ok("Update SRO successfully", teacherResponse!));
    }

    private bool IsMobilePhoneExists(string? mobilePhone, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e => e.MobilePhone == mobilePhone && e.Id != userId)
            : _context.Users.Any(e => e.MobilePhone == mobilePhone);
    }

    private bool IsEmailExists(string? email, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e => e.Email == email && e.Id != userId)
            : _context.Users.Any(e => e.Email == email);
    }

    private bool IsEmailOrganizationExists(string? emailOrganization, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e => e.EmailOrganization == emailOrganization && e.Id != userId)
            : _context.Users.Any(e => e.EmailOrganization == emailOrganization);
    }

    private bool IsCitizenIdentityCardNoExists(string? citizenIdentityCardNo, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e => e.CitizenIdentityCardNo == citizenIdentityCardNo && e.Id != userId)
            : _context.Users.Any(e => e.CitizenIdentityCardNo == citizenIdentityCardNo);
    }

    private bool IsTaxCodeExists(string? taxCode, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Teachers.Any(t => t.TaxCode == taxCode && t.UserId != userId)
            : _context.Teachers.Any(t => t.TaxCode == taxCode);
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
                Skills = u.Teacher.Skills.Select(s => new SkillResponse()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Teachers = new List<TeacherSkillInformation>()
                    {
                        new TeacherSkillInformation()
                        {
                            Id = u.Teacher.UserId,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                        }
                    }
                }).ToList(),
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

    private bool CheckMobilePhoneForCreate(CreateTeacherRequest request, out IActionResult badRequestObjectResult)
    {
        if (IsMobilePhoneExists(request.MobilePhone, false, 0))
        {
            var error = ErrorDescription.Error["E0050"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (!Regex.IsMatch(request.MobilePhone, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E0042"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        badRequestObjectResult = null!;
        return false;
    }

    private bool CheckEmailAndEmailOrganizeForCreate(CreateTeacherRequest request, out IActionResult teacher)
    {
        if (IsEmailExists(request.Email, false, 0))
        {
            var error = ErrorDescription.Error["E0051"];
            teacher = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (IsEmailExists(request.EmailOrganization, false, 0))
        {
            var error = ErrorDescription.Error["E0052_1"];
            teacher = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // if (!Regex.IsMatch(request.Email!, StringConstant.RegexEmail))
        // {
        //     var error = ErrorDescription.Error["E0043"];
        //     teacher = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        //     return true;
        // }

        if (IsEmailOrganizationExists(request.EmailOrganization, false, 0))
        {
            var error = ErrorDescription.Error["E0052"];
            teacher = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (IsEmailOrganizationExists(request.Email, false, 0))
        {
            var error = ErrorDescription.Error["E0051_1"];
            teacher = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (request.Email == request.EmailOrganization)
        {
            var error = ErrorDescription.Error["E0052_2"];
            teacher = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // if (!Regex.IsMatch(request.EmailOrganization!, StringConstant.RegexEmail))
        // {
        //     var error = ErrorDescription.Error["E0044"];
        //     teacher = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        //     return true;
        // }

        teacher = null!;
        return false;
    }

    private bool CheckTeacherNameForCreate(CreateTeacherRequest request, out IActionResult badRequest)
    {
        if (request.FirstName.Trim().Equals(string.Empty) || request.LastName.Trim().Equals(string.Empty))
        {
            badRequest =
                BadRequest(CustomResponse.BadRequest("firstName, lastName cannot be empty", "error-teacher-01"));
            return true;
        }

        request.FirstName = Regex.Replace(request.FirstName, StringConstant.RegexWhiteSpaces, " ");
        // function replace string ex: H ' Hen Nie => H'Hen Nie
        request.FirstName = request.FirstName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.FirstName, RegexSpecialCharacters))
        {
            var error = ErrorDescription.Error["E0046"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        request.LastName = Regex.Replace(request.LastName, StringConstant.RegexWhiteSpaces, " ");
        request.LastName = request.LastName.Replace(" ' ", "'").Trim();

        if (Regex.IsMatch(request.LastName, RegexSpecialCharacters))
        {
            var error = ErrorDescription.Error["E0047"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        badRequest = null!;
        return false;
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


    private bool CheckMobilePhoneForUpdate(int id, UpdateTeacherRequest request, out IActionResult updateTeacher1)
    {
        if (IsMobilePhoneExists(request.MobilePhone, true, id))
        {
            var error = ErrorDescription.Error["E0050"];
            updateTeacher1 = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (!Regex.IsMatch(request.MobilePhone, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E0042"];
            updateTeacher1 = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        updateTeacher1 = null!;
        return false;
    }

    private bool CheckEmailAndEmailOrganizationForUpdate(int id, UpdateTeacherRequest request,
        out IActionResult badRequestObjectResult)
    {
        if (IsEmailExists(request.Email, true, id))
        {
            var error = ErrorDescription.Error["E0051"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (IsEmailExists(request.EmailOrganization, true, id))
        {
            var error = ErrorDescription.Error["E0052_1"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // if (!Regex.IsMatch(request.Email!, StringConstant.RegexEmail))
        // {
        //     var error = ErrorDescription.Error["E0043"];
        //     badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        //     return true;
        // }

        if (IsEmailOrganizationExists(request.EmailOrganization, true, id))
        {
            var error = ErrorDescription.Error["E0052"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (IsEmailOrganizationExists(request.Email, true, id))
        {
            var error = ErrorDescription.Error["E0051_1"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (request.Email == request.EmailOrganization)
        {
            var error = ErrorDescription.Error["E0052_2"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // if (!Regex.IsMatch(request.EmailOrganization!, StringConstant.RegexEmail))
        // {
        //     var error = ErrorDescription.Error["E0044"];
        //     badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        //     return true;
        // }
        badRequestObjectResult = null!;
        return false;
    }

    private bool CheckTeacherNameForUpdate(UpdateTeacherRequest request, out IActionResult badRequest)
    {
        if (request.FirstName.Trim().Equals(string.Empty) || request.LastName.Trim().Equals(string.Empty))
        {
            badRequest =
                BadRequest(CustomResponse.BadRequest("firstName, lastName cannot be empty", "error-teacher-01"));
            return true;
        }

        request.FirstName = Regex.Replace(request.FirstName, StringConstant.RegexWhiteSpaces, " ");
        // function replace string ex: H ' Hen Nie => H'Hen Nie
        request.FirstName = request.FirstName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.FirstName, RegexSpecialCharacters))
        {
            var error = ErrorDescription.Error["E0046"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        request.LastName = Regex.Replace(request.LastName, StringConstant.RegexWhiteSpaces, " ");
        request.LastName = request.LastName.Replace(" ' ", "'").Trim();

        if (Regex.IsMatch(request.LastName, RegexSpecialCharacters))
        {
            var error = ErrorDescription.Error["E0047"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        badRequest = null!;
        return false;
    }

    private bool IsExistUserTeacher(int id, out User? user, out Teacher? teacher, out IActionResult actionResult)
    {
        user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            var error = ErrorDescription.Error["E0048"];
            actionResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            teacher = null;
            return true;
        }

        teacher = _context.Teachers.FirstOrDefault(s => s.UserId == id);
        if (teacher == null)
        {
            var error = ErrorDescription.Error["E0055"];
            actionResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        actionResult = null!;
        return false;
    }
}