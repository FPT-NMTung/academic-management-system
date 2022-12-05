using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.CourseController;
using AcademicManagementSystem.Models.CourseFamilyController;
using AcademicManagementSystem.Models.GenderController;
using AcademicManagementSystem.Models.GpaController;
using AcademicManagementSystem.Models.RoleController;
using AcademicManagementSystem.Models.UserController.StudentController;
using AcademicManagementSystem.Services;
using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class GpaController : ControllerBase
{
    private readonly AmsContext _context;
    private readonly User _user;
    private const int RoleIdStudent = 4;

    public GpaController(AmsContext context, IUserService userService)
    {
        _context = context;
        var userId = Convert.ToInt32(userService.GetUserId());
        _user = _context.Users.FirstOrDefault(u => u.Id == userId)!;
    }

    // get all form
    [HttpGet]
    [Route("api/gpa/forms")]
    [Authorize(Roles = "admin, sro, student")]
    public IActionResult GetForms()
    {
        var forms = _context.Forms
            .Select(f => new FormResponse()
            {
                Id = f.Id, Title = f.Title, Description = f.Description
            }).ToList();
        return Ok(CustomResponse.Ok("Forms retrieved successfully", forms));
    }

    // get all question by form id
    [HttpGet]
    [Route("api/gpa/forms/{formId:int}/questions")]
    [Authorize(Roles = "admin, sro, student")]
    public IActionResult GetQuestionsByFormId(int formId)
    {
        // check if form exists
        var form = _context.Forms.Find(formId);
        if (form == null)
        {
            return NotFound(CustomResponse.NotFound("Form not found with id " + formId));
        }

        var questions = _context.Questions
            .Include(q => q.Forms)
            .Where(q => q.Forms.Any(f => f.Id == formId))
            .Select(q => new QuestionResponse()
            {
                Id = q.Id, Content = q.Content
            }).ToList();
        return Ok(CustomResponse.Ok("Questions retrieved successfully", questions));
    }

    // get all answer by question id
    [HttpGet]
    [Route("api/gpa/forms/{formId:int}/questions/{questionId:int}/answers")]
    [Authorize(Roles = "admin, sro, student")]
    public IActionResult GetAnswersByQuestionId(int formId, int questionId)
    {
        // check if form exists or not
        var form = _context.Forms.Find(formId);
        if (form == null)
        {
            return NotFound(CustomResponse.NotFound("Form not found with id " + formId));
        }

        // check if question in form exists or not
        var question = _context.Questions
            .Include(q => q.Forms)
            .Where(q => q.Forms.Any(f => f.Id == formId))
            .FirstOrDefault(q => q.Id == questionId);
        if (question == null)
        {
            return NotFound(
                CustomResponse.NotFound("Question not found with id " + questionId + " in form id " + formId));
        }

        var answers = _context.Answers
            .Include(a => a.Question)
            .Include(a => a.Question.Forms)
            .Where(a => a.Question.Forms.Any(f => f.Id == formId) && a.Question.Id == questionId)
            .Select(a => new AnswerResponse()
            {
                Id = a.Id, QuestionId = a.QuestionId, AnswerNo = a.AnswerNo, Content = a.Content
            })
            .ToList();
        return Ok(CustomResponse.Ok("Answers retrieved successfully", answers));
    }

    // get all question and answer by form id
    [HttpGet]
    [Route("api/gpa/forms/{formId:int}/questions/answers")]
    [Authorize(Roles = "admin, sro, student")]
    public IActionResult GetQuestionsAndAnswersByFormId(int formId)
    {
        // check if form exists
        var form = _context.Forms.Find(formId);
        if (form == null)
        {
            return NotFound(CustomResponse.NotFound("Form not found with id " + formId));
        }

        var questions = _context.Questions
            .Include(q => q.Forms)
            .Include(q => q.Answers)
            .Where(q => q.Forms.Any(f => f.Id == formId))
            .Select(q => new QuestionResponse()
            {
                Id = q.Id, Content = q.Content, Answer = q.Answers.Select(a => new AnswerResponse()
                {
                    Id = a.Id, QuestionId = a.QuestionId, AnswerNo = a.AnswerNo, Content = a.Content
                }).ToList()
            })
            .ToList();
        foreach (var question in questions)
        {
            var answers = _context.Answers
                .Include(a => a.Question)
                .Include(a => a.Question.Forms)
                .Where(a => a.Question.Forms.Any(f => f.Id == formId) && a.Question.Id == question.Id)
                .Select(a => new AnswerResponse()
                {
                    Id = a.Id, QuestionId = a.QuestionId, AnswerNo = a.AnswerNo, Content = a.Content
                })
                .ToList();
            question.Answer = answers;
        }

        return Ok(CustomResponse.Ok("Questions and answers retrieved successfully", questions));
    }

    [HttpPost]
    [Route("api/gpa/{classId:int}/request-email")]
    [Authorize(Roles = "sro")]
    public IActionResult SendEmailRequestGpa(int classId)
    {
        // check class exists in center
        var classToSend = _context.Classes.FirstOrDefault(c => c.Id == classId && c.CenterId == _user.CenterId);
        if (classToSend == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found in center with id " + classId));
        }

        // get students in class
        var students = GetAllStudentsByClassId(classId);
        if (students.Count == 0)
        {
            return NotFound(CustomResponse.NotFound("No student found in class with id " + classId));
        }

        // get email of students
        var emailsStudent = students.Select(s => s.EmailOrganization).ToList();

        // get today date and format
        var today = DateTime.Now;
        var todayString = today.ToString("dd/MM/yyyy");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        var connectionString = configuration.GetConnectionString("AzureEmailConnectionString");
        var emailClient = new EmailClient(connectionString);
        var emailContent =
            new EmailContent("[QUAN TRỌNG] Yêu cầu lấy đánh giá về việc giảng dạy của giảng viên.") // subject
            {
                PlainText = "Học viên hãy vào lịch học và đánh giá giảng viên trong buổi học ngày hôm nay (" +
                            todayString + ").\n" +
                            "Những ai đã thực hiện đánh giá có thể bỏ qua Email này." // content
            };

        var listEmail = new List<EmailAddress>();
        foreach (var email in emailsStudent)
        {
            listEmail.Add(new EmailAddress(email));
        }

        var emailRecipients = new EmailRecipients(listEmail);
        var emailMessage = new EmailMessage("ams-no-reply@nmtung.dev", emailContent, emailRecipients);
        SendEmailResult emailResult = emailClient.Send(emailMessage, CancellationToken.None);

        return Ok(CustomResponse.Ok("Request has been sent successfully", emailResult));
    }

    private List<StudentResponse> GetAllStudentsByClassId(int id)
    {
        var students = _context.Users.Include(u => u.Student)
            .Include(u => u.Student.StudentsClasses)
            .Include(u => u.Student.Course)
            .Include(u => u.Student.Course.CourseFamily)
            .Include(u => u.Province)
            .Include(u => u.District)
            .Include(u => u.Ward)
            .Include(u => u.Center)
            .Include(u => u.Role)
            .Include(u => u.Gender)
            .Where(u => u.RoleId == RoleIdStudent &&
                        u.Student.StudentsClasses.Any(sc => sc.ClassId == id && sc.IsActive))
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
            })
            .Where(s => s.CenterId == _user.CenterId)
            .ToList();
        return students;
    }
}