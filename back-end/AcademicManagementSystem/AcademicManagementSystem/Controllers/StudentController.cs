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

        var students = GetAllStudents();

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
        var student = GetAllStudents().FirstOrDefault(s => s.UserId == id);

        if (student == null)
        {
            return NotFound(CustomResponse.NotFound("Not found student with id: " + id));
        }

        return Ok(CustomResponse.Ok("Student retrieved successfully", student));
    }

    // get all student by center Id
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

    // search student in center by courseCode, student name, enroll number, class, email, email organization
    [HttpGet]
    [Route("api/centers/{centerId:int}/students/search")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult SearchStudentsWithCenterId(int centerId, [FromQuery] string? courseCode,
        [FromQuery] string? studentName,
        [FromQuery] string? enrollNumber, [FromQuery] string? className, [FromQuery] string? email,
        [FromQuery] string? emailOrganization)
    {
        var existedCenter = _context.Centers.Any(c => c.Id == centerId);
        if (!existedCenter)
        {
            return NotFound(CustomResponse.NotFound("Not found center with id: " + centerId));
        }

        var sCourseCode = courseCode == null ? string.Empty : RemoveDiacritics(courseCode.Trim().ToLower());
        var sStudentName = studentName == null ? string.Empty : RemoveDiacritics(studentName.Trim().ToLower());
        var sEnrollNumber = enrollNumber == null ? string.Empty : RemoveDiacritics(enrollNumber.Trim().ToLower());
        var sClassName = className == null ? string.Empty : RemoveDiacritics(className.Trim().ToLower());
        var sEmail = email == null ? string.Empty : RemoveDiacritics(email.Trim().ToLower());
        var sEmailOrganization = emailOrganization == null
            ? string.Empty
            : RemoveDiacritics(emailOrganization.Trim().ToLower());

        var students = GetAllStudentsByCenterId(centerId);

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

    // get student by center id and student id
    [HttpGet]
    [Route("api/centers/{centerId:int}/students/{studentId:int}")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetStudentByCenterIdAndStudentId(int centerId, int studentId)
    {
        var existedCenter = _context.Centers.Any(c => c.Id == centerId);
        if (!existedCenter)
        {
            return NotFound(CustomResponse.NotFound("Not found center with id: " + centerId));
        }

        var student = GetAllStudentsByCenterId(centerId).FirstOrDefault(s => s.UserId == studentId);

        if (student == null)
        {
            return NotFound(CustomResponse.NotFound("Not found student with id: " + studentId));
        }

        return Ok(CustomResponse.Ok("Student retrieved successfully", student));
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
            .Include(u => u.Student.StudentsClasses)
            .ThenInclude(sc => sc.Class)
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
                },
                ClassId = u.Student.StudentsClasses.First(sc => sc.StudentId == u.Student.UserId).Class.Id,
                ClassName = u.Student.StudentsClasses.First(sc => sc.StudentId == u.Student.UserId).Class.Name
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