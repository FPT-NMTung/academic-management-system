using System.Text;
using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.CourseController;
using AcademicManagementSystem.Models.CourseFamilyController;
using AcademicManagementSystem.Models.GenderController;
using AcademicManagementSystem.Models.RoleController;
using AcademicManagementSystem.Models.UserController.StudentController;
using AcademicManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class StudentController : ControllerBase
{
    private readonly AmsContext _context;
    private readonly User _user;
    private const int RoleIdStudent = 4;

    public StudentController(AmsContext context, IUserService userService)
    {
        _context = context;
        var userId = Convert.ToInt32(userService.GetUserId());
        _user = _context.Users.FirstOrDefault(u => u.Id == userId)!;
    }

    // get all students
    [HttpGet]
    [Route("api/students")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetStudents()
    {
        var students = GetAllStudentsInThisCenterByContext();
        return Ok(!students.Any()
            ? CustomResponse.Ok("There is no students", students)
            : CustomResponse.Ok("Students retrieved successfully", students));
    }

    // search all students
    [HttpGet]
    [Route("api/students/search")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult SearchAllStudents([FromQuery] string? courseCode, [FromQuery] string? studentName,
        [FromQuery] string? enrollNumber, [FromQuery] string? className, [FromQuery] string? email,
        [FromQuery] string? emailOrganization)
    {
        var sCourseCode = courseCode == null ? string.Empty : RemoveDiacritics(courseCode.Trim().ToLower());
        var sStudentName = studentName == null ? string.Empty : RemoveDiacritics(studentName.Trim().ToLower());
        var sEnrollNumber = enrollNumber == null ? string.Empty : RemoveDiacritics(enrollNumber.Trim().ToLower());
        var sClassName = className == null ? string.Empty : RemoveDiacritics(className.Trim().ToLower());
        var sEmail = email == null ? string.Empty : RemoveDiacritics(email.Trim().ToLower());
        var sEmailOrganization = emailOrganization == null
            ? string.Empty
            : RemoveDiacritics(emailOrganization.Trim().ToLower());

        var students = GetAllStudentsInThisCenterByContext();

        // if user didn't input any search condition, return all students
        if (sCourseCode == string.Empty && sStudentName == string.Empty && sEnrollNumber == string.Empty &&
            sClassName == string.Empty && sEmail == string.Empty && sEmailOrganization == string.Empty)
        {
            return Ok(CustomResponse.Ok("Students searched successfully", students));
        }

        var studentResponses = new List<StudentResponse>();
        foreach (var student in students)
        {
            var s1 = RemoveDiacritics(student.Course!.Code!.ToLower());
            var s2 = RemoveDiacritics(student.FirstName!.ToLower());
            var s3 = RemoveDiacritics(student.LastName!.ToLower());
            var studentFullName = s2 + " " + s3;
            var s4 = RemoveDiacritics(student.EnrollNumber!.ToLower());
            var s5 = RemoveDiacritics(student.ClassName!.ToLower());
            var s6 = RemoveDiacritics(student.Email!.ToLower());
            var s7 = RemoveDiacritics(student.EmailOrganization!.ToLower());

            if (s1.Contains(sCourseCode)
                && studentFullName.Contains(sStudentName)
                && s4.Contains(sEnrollNumber)
                && s5.Contains(sClassName)
                && s6.Contains(sEmail)
                && s7.Contains(sEmailOrganization))
            {
                studentResponses.Add(student);
            }
        }

        return Ok(CustomResponse.Ok("Students searched successfully", studentResponses));
    }

    // get students by id
    [HttpGet]
    [Route("api/students/{id:int}")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetStudentById(int id)
    {
        var student = GetAllStudentsInThisCenterByContext().FirstOrDefault(s => s.UserId == id);

        if (student == null)
        {
            return NotFound(CustomResponse.NotFound("Not found student with id: " + id + " in this center"));
        }

        return Ok(CustomResponse.Ok("Student retrieved successfully", student));
    }

    // update student information
    [HttpPut]
    [Route("api/students/{id:int}")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult UpdateStudent(int id, [FromBody] UpdateStudentRequest request)
    {
        var user = _context.Users
            .Include(u => u.Student)
            .Include(u => u.Student.Course)
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            return NotFound(CustomResponse.NotFound("Not found user with id: " + id));
        }

        request.FirstName = request.FirstName.Trim();
        request.LastName = request.LastName.Trim();
        request.Email = request.Email.Trim();
        request.EmailOrganization = request.EmailOrganization.Trim();
        request.CourseCode = request.CourseCode.ToUpper().Trim();
        request.ParentalName = request.ParentalName.Trim();
        request.ParentalRelationship = request.ParentalRelationship.Trim();
        request.ContactAddress = request.ContactAddress.Trim();
        request.CitizenIdentityCardPublishedPlace = request.CitizenIdentityCardPublishedPlace.Trim();

        if (string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName) ||
            string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.EmailOrganization) ||
            string.IsNullOrEmpty(request.CourseCode) || string.IsNullOrEmpty(request.ParentalName) ||
            string.IsNullOrEmpty(request.ParentalRelationship) || string.IsNullOrEmpty(request.ContactAddress) ||
            string.IsNullOrEmpty(request.CitizenIdentityCardPublishedPlace))
        {
            var error = ErrorDescription.Error["E1093"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // is course code exists
        var course = _context.Courses.FirstOrDefault(c => c.Code == request.CourseCode && c.IsActive);
        if (course == null)
        {
            var error = ErrorDescription.Error["E1098"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.FirstName = Regex.Replace(request.FirstName, StringConstant.RegexWhiteSpaces, " ");
        // function replace string ex: H ' Hen Nie => H'Hen Nie
        request.FirstName = request.FirstName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.FirstName, StringConstant.RegexSpecialCharsNotAllowForPersonName)
            || Regex.IsMatch(request.FirstName, StringConstant.RegexDigits))
        {
            var error = ErrorDescription.Error["E1076"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.LastName = Regex.Replace(request.LastName, StringConstant.RegexWhiteSpaces, " ");
        request.LastName = request.LastName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.LastName, StringConstant.RegexSpecialCharsNotAllowForPersonName) ||
            Regex.IsMatch(request.LastName, StringConstant.RegexDigits))
        {
            var error = ErrorDescription.Error["E1077"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.ParentalName = Regex.Replace(request.ParentalName, StringConstant.RegexWhiteSpaces, " ");
        request.ParentalName = request.ParentalName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ParentalName, StringConstant.RegexSpecialCharsNotAllowForPersonName) ||
            Regex.IsMatch(request.ParentalName, StringConstant.RegexDigits))
        {
            var error = ErrorDescription.Error["E1099"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.ParentalRelationship =
            Regex.Replace(request.ParentalRelationship, StringConstant.RegexWhiteSpaces, " ");
        if (Regex.IsMatch(request.ParentalRelationship, StringConstant.RegexSpecialCharacter) ||
            Regex.IsMatch(request.ParentalRelationship, StringConstant.RegexDigits))
        {
            var error = ErrorDescription.Error["E1100"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.ContactAddress = Regex.Replace(request.ContactAddress, StringConstant.RegexWhiteSpaces, " ");
        request.ContactAddress = request.ContactAddress.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ContactAddress, StringConstant.RegexSpecialCharacterForAddress))
        {
            var error = ErrorDescription.Error["E1101"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.HighSchool != null)
        {
            request.HighSchool = Regex.Replace(request.HighSchool, StringConstant.RegexWhiteSpaces, " ");
            request.HighSchool = request.HighSchool.Replace(" ' ", "'").Trim();
            if (Regex.IsMatch(request.HighSchool, StringConstant.RegexSpecialCharacterForSchool))
            {
                var error = ErrorDescription.Error["E1103"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }
        }

        if (request.University != null)
        {
            request.University = Regex.Replace(request.University, StringConstant.RegexWhiteSpaces, " ");
            request.University = request.University.Replace(" ' ", "'").Trim();
            if (Regex.IsMatch(request.University, StringConstant.RegexSpecialCharacterForSchool))
            {
                var error = ErrorDescription.Error["E1104"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }
        }

        if (request.WorkingCompany != null)
        {
            request.WorkingCompany = Regex.Replace(request.WorkingCompany, StringConstant.RegexWhiteSpaces, " ");
            request.WorkingCompany = request.WorkingCompany.Replace(" ' ", "'").Trim();
            if (Regex.IsMatch(request.WorkingCompany, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
            {
                var error = ErrorDescription.Error["E1105"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }
        }

        if (request.CompanyPosition != null)
        {
            request.CompanyPosition = Regex.Replace(request.CompanyPosition, StringConstant.RegexWhiteSpaces, " ");
            if (Regex.IsMatch(request.CompanyPosition, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
            {
                var error = ErrorDescription.Error["E1106"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }
        }

        if (request.CompanyAddress != null)
        {
            request.CompanyAddress = Regex.Replace(request.CompanyAddress, StringConstant.RegexWhiteSpaces, " ");
            if (Regex.IsMatch(request.CompanyAddress, StringConstant.RegexSpecialCharacterForAddress))
            {
                var error = ErrorDescription.Error["E1107"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }
        }

        if (IsMobilePhoneExists(request.MobilePhone, true, id))
        {
            var error = ErrorDescription.Error["E1078"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.MobilePhone, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E1079"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.ContactPhone, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E1079_1"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.HomePhone != null && !Regex.IsMatch(request.HomePhone, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E1079_2"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.ParentalPhone, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E1079_3"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailExists(request.Email, true, id))
        {
            var error = ErrorDescription.Error["E1080"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailExists(request.EmailOrganization, true, id))
        {
            var error = ErrorDescription.Error["E1081"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.Email, StringConstant.RegexEmailCopilot))
        {
            var error = ErrorDescription.Error["E1096"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailOrganizationExists(request.EmailOrganization, true, id))
        {
            var error = ErrorDescription.Error["E1082"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailOrganizationExists(request.Email, true, id))
        {
            var error = ErrorDescription.Error["E1081_1"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.EmailOrganization, StringConstant.RegexEmailCopilot))
        {
            var error = ErrorDescription.Error["E1097"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Email == request.EmailOrganization)
        {
            var error = ErrorDescription.Error["E1083"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsCitizenIdentityCardNoExists(request.CitizenIdentityCardNo, true, id))
        {
            var error = ErrorDescription.Error["E1084"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.CitizenIdentityCardPublishedPlace = Regex.Replace(request.CitizenIdentityCardPublishedPlace,
            StringConstant.RegexWhiteSpaces, " ");
        request.CitizenIdentityCardPublishedPlace =
            request.CitizenIdentityCardPublishedPlace.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.CitizenIdentityCardPublishedPlace, StringConstant.RegexSpecialCharacterForAddress))
        {
            var error = ErrorDescription.Error["E1102"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.CitizenIdentityCardNo, StringConstant.RegexCitizenIdCardNo))
        {
            var error = ErrorDescription.Error["E1085"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!IsProvinceExists(request.ProvinceId))
        {
            var error = ErrorDescription.Error["E1088"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!IsDistrictExists(request.DistrictId))
        {
            var error = ErrorDescription.Error["E1089"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!IsWardExists(request.WardId))
        {
            var error = ErrorDescription.Error["E1090"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!IsAddressExists(request.ProvinceId, request.DistrictId, request.WardId))
        {
            var error = ErrorDescription.Error["E1091"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!IsGenderExists(request.GenderId))
        {
            var error = ErrorDescription.Error["E1092"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Status is < 1 or > 7)
        {
            var error = ErrorDescription.Error["E1094"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.FeePlan < 0)
        {
            var error = ErrorDescription.Error["E1108"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Promotion < 0)
        {
            var error = ErrorDescription.Error["E1109"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }
        
        if (request.CompanySalary is < 0)
        {
            var error = ErrorDescription.Error["E1110"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.MobilePhone = request.MobilePhone;
        user.Email = request.Email;
        user.EmailOrganization = request.EmailOrganization;
        user.ProvinceId = request.ProvinceId;
        user.DistrictId = request.DistrictId;
        user.WardId = request.WardId;
        user.GenderId = request.GenderId;
        user.Birthday = request.Birthday.Date;
        user.CitizenIdentityCardNo = request.CitizenIdentityCardNo;
        user.CitizenIdentityCardPublishedDate = request.CitizenIdentityCardPublishedDate.Date;
        user.CitizenIdentityCardPublishedPlace = request.CitizenIdentityCardPublishedPlace;
        user.UpdatedAt = DateTime.Now;
        user.Student = new Student()
        {
            EnrollNumber = user.Student.EnrollNumber,
            Status = request.Status,
            StatusDate = request.Status == user.Student.Status ? user.Student.StatusDate : DateTime.Now.Date,
            HomePhone = request.HomePhone,
            ContactPhone = request.ContactPhone,
            ParentalName = request.ParentalName,
            ParentalRelationship = request.ParentalRelationship,
            ContactAddress = request.ContactAddress,
            ParentalPhone = request.ParentalPhone,
            ApplicationDate = request.ApplicationDate,
            ApplicationDocument = request.ApplicationDocument,
            HighSchool = request.HighSchool,
            University = request.University,
            FacebookUrl = request.FacebookUrl,
            PortfolioUrl = request.PortfolioUrl,
            WorkingCompany = request.WorkingCompany,
            CompanySalary = request.CompanySalary,
            CompanyPosition = request.CompanyPosition,
            CompanyAddress = request.CompanyAddress,
            FeePlan = request.FeePlan,
            Promotion = request.Promotion,
            CourseCode = request.CourseCode
        };
        _context.Users.Update(user);
        _context.Students.Update(user.Student);
        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1114"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var studentResponse = GetAllStudentsInThisCenterByContext().FirstOrDefault(s => s.UserId == id);
        if (studentResponse == null)
        {
            var error = ErrorDescription.Error["E1095"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Student updated successfully", studentResponse));
    }

    private List<StudentResponse> GetAllStudentsInThisCenterByContext()
    {
        var students = _context.Users.Include(u => u.Student)
            .Include(u => u.Student.Course)
            .Include(u => u.Student.Course.CourseFamily)
            .Include(u => u.Province)
            .Include(u => u.District)
            .Include(u => u.Ward)
            .Include(u => u.Center)
            .Include(u => u.Role)
            .Include(u => u.Gender)
            .Include(u => u.Student.StudentsClasses)
            .ThenInclude(sc => sc.Class)
            .Where(u => u.RoleId == RoleIdStudent && !u.Student.IsDraft)
            .Select(u => new StudentResponse()
            {
                UserId = u.Student.UserId, Promotion = u.Student.Promotion, Status = u.Student.Status,
                University = u.Student.University, ApplicationDate = u.Student.ApplicationDate,
                ApplicationDocument = u.Student.ApplicationDocument, CompanyAddress = u.Student.CompanyAddress,
                CompanyPosition = u.Student.CompanyPosition, CompanySalary = u.Student.CompanySalary,
                ContactAddress = u.Student.ContactAddress, ContactPhone = u.Student.ContactPhone,
                CourseCode = u.Student.CourseCode, EnrollNumber = u.Student.EnrollNumber,
                FacebookUrl = u.Student.FacebookUrl, FeePlan = u.Student.FeePlan, HighSchool = u.Student.HighSchool,
                HomePhone = u.Student.HomePhone, ParentalName = u.Student.ParentalName,
                ParentalRelationship = u.Student.ParentalRelationship, ParentalPhone = u.Student.ParentalPhone,
                PortfolioUrl = u.Student.PortfolioUrl, StatusDate = u.Student.StatusDate,
                WorkingCompany = u.Student.WorkingCompany, IsDraft = u.Student.IsDraft, Avatar = u.Avatar,

                FirstName = u.FirstName, LastName = u.LastName, Birthday = u.Birthday, Email = u.Email,
                MobilePhone = u.MobilePhone, CenterId = u.CenterId, EmailOrganization = u.EmailOrganization,
                CenterName = u.Center.Name, CreatedAt = u.CreatedAt, UpdatedAt = u.UpdatedAt,
                CitizenIdentityCardNo = u.CitizenIdentityCardNo,
                CitizenIdentityCardPublishedDate = u.CitizenIdentityCardPublishedDate,
                CitizenIdentityCardPublishedPlace = u.CitizenIdentityCardPublishedPlace,

                Course = new CourseResponse()
                {
                    Code = u.Student.Course.Code, Name = u.Student.Course.Name,
                    SemesterCount = u.Student.Course.SemesterCount,
                    CourseFamilyCode = u.Student.Course.CourseFamilyCode, IsActive = u.Student.Course.IsActive,
                    CreatedAt = u.Student.Course.CreatedAt,
                    UpdatedAt = u.Student.Course.UpdatedAt, CourseFamily = new CourseFamilyResponse()
                    {
                        Code = u.Student.Course.CourseFamily.Code, Name = u.Student.Course.CourseFamily.Name,
                        IsActive = u.Student.Course.CourseFamily.IsActive,
                        PublishedYear = u.Student.Course.CourseFamily.PublishedYear,
                        CreatedAt = u.Student.Course.CourseFamily.CreatedAt,
                        UpdatedAt = u.Student.Course.CourseFamily.UpdatedAt
                    }
                },
                Province = new ProvinceResponse()
                {
                    Id = u.Province.Id, Name = u.Province.Name, Code = u.Province.Code
                },
                District = new DistrictResponse()
                {
                    Id = u.District.Id, Name = u.District.Name, Prefix = u.District.Prefix
                },
                Ward = new WardResponse()
                {
                    Id = u.Ward.Id, Name = u.Ward.Name, Prefix = u.Ward.Prefix
                },
                Gender = new GenderResponse()
                {
                    Id = u.Gender.Id, Value = u.Gender.Value
                },
                Role = new RoleResponse()
                {
                    Id = u.Role.Id, Value = u.Role.Value
                },
                ClassId = u.Student.StudentsClasses.First(sc => sc.StudentId == u.Student.UserId).Class.Id,
                ClassName = u.Student.StudentsClasses.First(sc => sc.StudentId == u.Student.UserId).Class.Name
            })
            .Where(u => u.CenterId == _user.CenterId)
            .ToList();
        return students;
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

    private bool IsProvinceExists(int provinceId)
    {
        return _context.Provinces.Any(e => e.Id == provinceId);
    }

    private bool IsDistrictExists(int districtId)
    {
        return _context.Districts.Any(e => e.Id == districtId);
    }

    private bool IsWardExists(int wardId)
    {
        return _context.Wards.Any(e => e.Id == wardId);
    }

    private bool IsAddressExists(int provinceId, int districtId, int wardId)
    {
        return _context.Provinces
            .Include(p => p.Districts)
            .Include(p => p.Wards)
            .Any(p => p.Id == provinceId && p.Districts.Any(d => d.Id == districtId && d.Province.Id == provinceId) &&
                      p.Wards.Any(w =>
                          w.Id == wardId && w.District.Id == districtId && w.District.Province.Id == provinceId));
    }

    private bool IsGenderExists(int genderId)
    {
        return _context.Genders.Any(g => g.Id == genderId);
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