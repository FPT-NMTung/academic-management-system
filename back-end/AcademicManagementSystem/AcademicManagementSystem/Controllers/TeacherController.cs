using System.Text;
using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.GenderController;
using AcademicManagementSystem.Models.RoleController;
using AcademicManagementSystem.Models.TeacherTypeController;
using AcademicManagementSystem.Models.UserController.TeacherController;
using AcademicManagementSystem.Models.WorkingTime;
using AcademicManagementSystem.Services;
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
    private const string Digits = StringConstant.RegexDigits;
    private readonly IUserService _userService;

    public TeacherController(AmsContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
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
    public IActionResult GetTeachersByTeacherTypeId([FromQuery] int teacherTypeId)
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
    [Authorize(Roles = "admin")]
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

    [HttpGet]
    [Route("api/teachers/search-by-sro")]
    [Authorize(Roles = "sro")]
    public IActionResult SearchTeachersBySro([FromQuery] string? firstName, [FromQuery] string? lastName,
        [FromQuery] string? nickname, [FromQuery] string? mobilePhone, [FromQuery] string? email,
        [FromQuery] string? emailOrganization)
    {
        var userId = Int32.Parse(_userService.GetUserId());
        var user = _context.Users.FirstOrDefault(u => u.Id == userId)!;

        var sFirstName = firstName == null ? string.Empty : RemoveDiacritics(firstName.Trim().ToLower());
        var sLastName = lastName == null ? string.Empty : RemoveDiacritics(lastName.Trim().ToLower());
        var sNickname = nickname == null ? string.Empty : RemoveDiacritics(nickname.Trim().ToLower());
        var sMobilePhone = mobilePhone == null ? string.Empty : RemoveDiacritics(mobilePhone.Trim().ToLower());
        var sEmail = email == null ? string.Empty : RemoveDiacritics(email.Trim().ToLower());
        var sEmailOrganization = emailOrganization == null
            ? string.Empty
            : RemoveDiacritics(emailOrganization.Trim().ToLower());

        var teachers = GetAllUserRoleTeacher().Where(t => t.CenterId == user.CenterId).ToList();

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

        return Ok(CustomResponse.Ok("Search Teachers by SRO successfully", teacherResponses));
    }

    // create teacher
    [HttpPost]
    [Route("api/teachers")]
    [Authorize(Roles = "admin")]
    public IActionResult CreateTeacher([FromBody] CreateTeacherRequest request)
    {
        if (request.FirstName.Trim().Equals(string.Empty) || request.LastName.Trim().Equals(string.Empty))
        {
            return BadRequest(CustomResponse.BadRequest("firstName, lastName cannot be empty", "error-teacher-01"));
        }

        request.FirstName = Regex.Replace(request.FirstName, StringConstant.RegexWhiteSpaces, " ");
        // function replace string ex: H ' Hen Nie => H'Hen Nie
        request.FirstName = request.FirstName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.FirstName, RegexSpecialCharacters) ||
            Regex.IsMatch(request.FirstName, Digits))
        {
            var error = ErrorDescription.Error["E0046"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.LastName = Regex.Replace(request.LastName, StringConstant.RegexWhiteSpaces, " ");
        request.LastName = request.LastName.Replace(" ' ", "'").Trim();

        if (Regex.IsMatch(request.LastName, RegexSpecialCharacters) ||
            Regex.IsMatch(request.LastName, Digits))
        {
            var error = ErrorDescription.Error["E0047"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Birthday.Date >= DateTime.Now.Date)
        {
            var error = ErrorDescription.Error["E0052_3"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsMobilePhoneExists(request.MobilePhone, false, 0))
        {
            var error = ErrorDescription.Error["E0050"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.MobilePhone, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E0042"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailExists(request.Email, false, 0))
        {
            var error = ErrorDescription.Error["E0051"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailExists(request.EmailOrganization, false, 0))
        {
            var error = ErrorDescription.Error["E0052_1"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.Email, StringConstant.RegexEmailCopilot))
        {
            var error = ErrorDescription.Error["E0043"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailOrganizationExists(request.EmailOrganization, false, 0))
        {
            var error = ErrorDescription.Error["E0052"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailOrganizationExists(request.Email, false, 0))
        {
            var error = ErrorDescription.Error["E0051_1"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Email.ToLower() == request.EmailOrganization.ToLower())
        {
            var error = ErrorDescription.Error["E0052_2"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.EmailOrganization, StringConstant.RegexEmailCopilot))
        {
            var error = ErrorDescription.Error["E0044"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

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

        if (request.CitizenIdentityCardPublishedDate >= DateTime.Now.Date)
        {
            var error = ErrorDescription.Error["E0052_5"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Salary < 0)
        {
            var error = ErrorDescription.Error["E0052_4"];
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
            IsActive = true,
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

        try
        {
            _context.Users.Add(user);
            _context.Teachers.Add(user.Teacher);
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0049"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var teacherResponse = GetAllUserRoleTeacher().FirstOrDefault(s => s.UserId == user.Id);
        if (teacherResponse == null)
        {
            var error = ErrorDescription.Error["E0055"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Create Teacher successfully", teacherResponse));
    }

    //update teacher 
    [HttpPut]
    [Route("api/teachers/{id:int}")]
    [Authorize(Roles = "admin")]
    public IActionResult UpdateTeacher(int id, [FromBody] UpdateTeacherRequest request)
    {
        if (request.FirstName.Trim().Equals(string.Empty) || request.LastName.Trim().Equals(string.Empty))
        {
            return BadRequest(CustomResponse.BadRequest("firstName, lastName cannot be empty", "error-teacher-01"));
        }

        var user = _context.Users.FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            var error = ErrorDescription.Error["E0048"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var teacher = _context.Teachers.FirstOrDefault(s => s.UserId == id);
        if (teacher == null)
        {
            var error = ErrorDescription.Error["E0055"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.FirstName = Regex.Replace(request.FirstName, StringConstant.RegexWhiteSpaces, " ");
        // function replace string ex: H ' Hen Nie => H'Hen Nie
        request.FirstName = request.FirstName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.FirstName, RegexSpecialCharacters)
            || Regex.IsMatch(request.FirstName, Digits))
        {
            var error = ErrorDescription.Error["E0046"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.LastName = Regex.Replace(request.LastName, StringConstant.RegexWhiteSpaces, " ");
        request.LastName = request.LastName.Replace(" ' ", "'").Trim();

        if (Regex.IsMatch(request.LastName, RegexSpecialCharacters) ||
            Regex.IsMatch(request.LastName, Digits))
        {
            var error = ErrorDescription.Error["E0047"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsMobilePhoneExists(request.MobilePhone, true, id))
        {
            var error = ErrorDescription.Error["E0050"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Birthday.Date >= DateTime.Now.Date)
        {
            var error = ErrorDescription.Error["E0052_3"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.MobilePhone, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E0042"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailExists(request.Email, true, id))
        {
            var error = ErrorDescription.Error["E0051"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailExists(request.EmailOrganization, true, id))
        {
            var error = ErrorDescription.Error["E0052_1"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.Email, StringConstant.RegexEmailCopilot))
        {
            var error = ErrorDescription.Error["E0043"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailOrganizationExists(request.EmailOrganization, true, id))
        {
            var error = ErrorDescription.Error["E0052"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailOrganizationExists(request.Email, true, id))
        {
            var error = ErrorDescription.Error["E0051_1"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Email.ToLower() == request.EmailOrganization.ToLower())
        {
            var error = ErrorDescription.Error["E0052_2"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.EmailOrganization, StringConstant.RegexEmailCopilot))
        {
            var error = ErrorDescription.Error["E0044"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

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

        if (request.CitizenIdentityCardPublishedDate >= DateTime.Now.Date)
        {
            var error = ErrorDescription.Error["E0052_5"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Salary < 0)
        {
            var error = ErrorDescription.Error["E0052_4"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

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

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0049"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var teacherResponse = GetAllUserRoleTeacher().FirstOrDefault(s => s.UserId == teacher.UserId);
        if (teacherResponse == null)
        {
            var error = ErrorDescription.Error["E0055"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Update Teacher successfully", teacherResponse));
    }

    // Can delete teacher
    [HttpGet]
    [Route("api/teachers/{id:int}/can-delete")]
    [Authorize(Roles = "admin")]
    public IActionResult CanDeleteTeacher(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id && u.RoleId == RoleIdTeacher);
        if (user == null)
        {
            return NotFound(CustomResponse.NotFound("Not Found Teacher"));
        }

        var canDelete = CanDelete(id);

        return Ok(CustomResponse.Ok("Can delete this teacher", new CheckTeacherCanDeleteResponse()
        {
            CanDelete = canDelete
        }));
    }

    // delete teacher
    [HttpDelete]
    [Route("api/teachers/{id:int}")]
    [Authorize(Roles = "admin")]
    public IActionResult DeleteTeacher(int id)
    {
        var user = _context.Users
            .Include(u => u.Teacher)
            .FirstOrDefault(u => u.Id == id && u.RoleId == RoleIdTeacher);
        if (user == null)
        {
            return NotFound(CustomResponse.NotFound("Not Found Teacher"));
        }

        try
        {
            _context.Teachers.Remove(user.Teacher);
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1118"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Teacher deleted successfully", null!));
    }

    // change active teacher
    [HttpPatch]
    [Route("api/teachers/{id:int}/change-active")]
    [Authorize(Roles = "admin")]
    public IActionResult ChangeActivateTeacher(int id)
    {
        var teacher = _context.Teachers.Include(t => t.User).FirstOrDefault(s => s.UserId == id);
        if (teacher == null)
        {
            var error = ErrorDescription.Error["E0055"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        try
        {
            teacher.User.IsActive = !teacher.User.IsActive;
            teacher.User.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
        }
        catch (Exception)
        {
            var error = ErrorDescription.Error["E2056"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Change active successfully", null!));
    }

    // get all teacher of center with SRO
    [HttpGet]
    [Route("api/teachers/get-by-sro")]
    [Authorize(Roles = "sro")]
    public IActionResult GetAllTeacherOfCenterWithSro()
    {
        var userId = Int32.Parse(_userService.GetUserId());
        var user = _context.Users.FirstOrDefault(u => u.Id == userId)!;

        var teachers = GetAllUserRoleTeacher().Where(t => t.CenterId == user.CenterId && t.IsActive).ToList();
        return Ok(CustomResponse.Ok("Get teachers by sro successfully", teachers));
    }

    private bool IsMobilePhoneExists(string? mobilePhone, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e => e.MobilePhone == mobilePhone && e.Id != userId)
            : _context.Users.Any(e => e.MobilePhone == mobilePhone);
    }

    private bool IsEmailExists(string email, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e => e.Email.Trim().ToLower().Equals(email.Trim().ToLower()) && e.Id != userId)
            : _context.Users.Any(e => e.Email.Trim().ToLower().Equals(email.Trim().ToLower()));
    }

    private bool IsEmailOrganizationExists(string emailOrganization, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e =>
                e.EmailOrganization.Trim().ToUpper().Equals(emailOrganization.Trim().ToUpper()) && e.Id != userId)
            : _context.Users.Any(e => e.EmailOrganization.Trim().ToUpper().Equals(emailOrganization.Trim().ToUpper()));
    }

    private bool IsCitizenIdentityCardNoExists(string? citizenIdentityCardNo, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e => e.CitizenIdentityCardNo == citizenIdentityCardNo && e.Id != userId)
            : _context.Users.Any(e => e.CitizenIdentityCardNo == citizenIdentityCardNo);
    }

    private bool CanDelete(int id)
    {
        var user = _context.Users
            .Include(u => u.Teacher)
            .Include(u => u.Teacher.Skills)
            .Include(u => u.Teacher.ClassSchedules)
            .Include(u => u.Teacher.DaysOff)
            .Include(u => u.Teacher.GpaRecords)
            .Include(u => u.ActiveRefreshTokens)
            .FirstOrDefault(u => u.Id == id && u.RoleId == RoleIdTeacher && u.ActiveRefreshTokens.Count == 0);

        if (user == null)
        {
            return false;
        }

        return !user.Teacher.Skills.Any() && !user.Teacher.ClassSchedules.Any() &&
               !user.Teacher.DaysOff.Any() && !user.Teacher.GpaRecords.Any();
    }

    private bool IsTaxCodeExists(string? taxCode, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Teachers.Any(t => t.TaxCode == taxCode && t.UserId != userId)
            : _context.Teachers.Any(t => t.TaxCode == taxCode);
    }

    private IQueryable<TeacherResponse> GetAllUserRoleTeacher()
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
                IsActive = u.IsActive,
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