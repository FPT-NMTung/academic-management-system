using System.Text;
using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.CourseController;
using AcademicManagementSystem.Models.CourseFamilyController;
using AcademicManagementSystem.Models.GenderController;
using AcademicManagementSystem.Models.RoleController;
using AcademicManagementSystem.Models.UserController.StudentController;
using AcademicManagementSystem.Models.UserController.TeacherController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class StudentController : ControllerBase
{
    private readonly AmsContext _context;
    private const int RoleIdStudent = 4;

    public StudentController(AmsContext context)
    {
        _context = context;
    }

    // get all students
    [HttpGet]
    [Route("api/students")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetStudents()
    {
        var students = GetAllStudents();
        return Ok(!students.Any()
            ? CustomResponse.Ok("There is no students", students)
            : CustomResponse.Ok("Students retrieved successfully", students));
    }
    
    // get all students
    [HttpGet]
    [Route("api/centers/{centerId:int}/students")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetStudentsByCenterId(int centerId)
    {
        // is center exists
        var existedCenter = _context.Centers.Any(c => c.Id == centerId);
        if (!existedCenter)
        {
            return NotFound(CustomResponse.NotFound("Not found center with id: " + centerId));
        }

        var students = GetAllStudentsByCenterId(centerId);
        return Ok(!students.Any()
            ? CustomResponse.Ok("There is no students in this center", students)
            : CustomResponse.Ok("Students in center " + centerId + " retrieved successfully", students));
    }

    // search student by by firstName, lastName, mobilePhone, email, emailOrganization
    [HttpGet]
    [Route("api/centers/{centerId:int}/students/search")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult SearchStudents(int centerId, [FromQuery] string? firstName, [FromQuery] string? lastName,
        [FromQuery] string? mobilePhone, [FromQuery] string? email,
        [FromQuery] string? emailOrganization)
    {
        var existedCenter = _context.Centers.Any(c => c.Id == centerId);
        if (!existedCenter)
        {
            return NotFound(CustomResponse.NotFound("Not found center with id: " + centerId));
        }

        var sFirstName = firstName == null ? string.Empty : RemoveDiacritics(firstName.Trim().ToLower());
        var sLastName = lastName == null ? string.Empty : RemoveDiacritics(lastName.Trim().ToLower());
        var sMobilePhone = mobilePhone == null ? string.Empty : RemoveDiacritics(mobilePhone.Trim().ToLower());
        var sEmail = email == null ? string.Empty : RemoveDiacritics(email.Trim().ToLower());
        var sEmailOrganization = emailOrganization == null
            ? string.Empty
            : RemoveDiacritics(emailOrganization.Trim().ToLower());

        var students = GetAllStudentsByCenterId(centerId);

        // if user didn't input any search condition, return all teachers
        if (sFirstName == string.Empty && sLastName == string.Empty
                                       && sMobilePhone == string.Empty
                                       && sEmail == string.Empty && sEmailOrganization == string.Empty)
        {
            return Ok(CustomResponse.Ok("Students searched successfully", students));
        }

        var studentResponses = new List<StudentResponse>();
        foreach (var t in students)
        {
            var s1 = RemoveDiacritics(t.FirstName!.ToLower());
            var s2 = RemoveDiacritics(t.LastName!.ToLower());
            // t.Nickname ??= string.Empty;
            // var s3 = RemoveDiacritics(t.Nickname.ToLower());
            var s4 = RemoveDiacritics(t.MobilePhone!.ToLower());
            var s5 = RemoveDiacritics(t.Email!.ToLower());
            var s6 = RemoveDiacritics(t.EmailOrganization!.ToLower());

            if (s1.Contains(sFirstName)
                && s2.Contains(sLastName)
                // && s3.Contains(sNickname)
                && s4.Contains(sMobilePhone)
                && s5.Contains(sEmail)
                && s6.Contains(sEmailOrganization))
            {
                studentResponses.Add(t);
            }
        }

        return Ok(CustomResponse.Ok("Students searched successfully", studentResponses));
    }

    private List<StudentResponse> GetAllStudentsByCenterId(int centerId)
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
            .Where(u => u.RoleId == RoleIdStudent && !u.Student.IsDraft && u.CenterId == centerId)
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
                }
            }).ToList();
        return students;
    }
    
        private List<StudentResponse> GetAllStudents()
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
                }
            }).ToList();
        return students;
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