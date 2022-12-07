using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.ClassController;
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

    // student take gpa teacher
    [HttpPost]
    [Route("api/gpa/forms/{formId:int}")]
    [Authorize(Roles = "student")]
    public IActionResult TakeGpaTeacher(int formId, [FromBody] TakeGpaRequest request)
    {
        // check if form exists or not
        var form = _context.Forms.Find(formId);
        if (form == null)
        {
            return NotFound(CustomResponse.NotFound("Form not found with id " + formId));
        }

        // check if class exists or not
        if (!IsClassExisted(request.ClassId))
        {
            var error = ErrorDescription.Error["E1139"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check if teacher exists or not
        if (!IsTeacherExisted(request.TeacherId))
        {
            var error = ErrorDescription.Error["E1140"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check if module exists or not
        if (!IsModuleExisted(request.ModuleId))
        {
            var error = ErrorDescription.Error["E1141"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check if session exists or not
        if (!IsSessionExisted(request.SessionId))
        {
            var error = ErrorDescription.Error["E1142"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check if student is in class or not
        if (!IsStudentInClass(request.ClassId))
        {
            var error = ErrorDescription.Error["E1143"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check if teacher is teaching in class or not
        if (!IsTeacherInClass(request.ClassId, request.TeacherId))
        {
            var error = ErrorDescription.Error["E1144"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check if is module in class or not
        if (!IsModuleInClass(request.ClassId, request.ModuleId))
        {
            var error = ErrorDescription.Error["E1148"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check if is session in class or not
        if (!IsSessionInClass(request.ClassId, request.SessionId, request.ModuleId))
        {
            var error = ErrorDescription.Error["E1149"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check if teacher is teaching this module in class or not
        if (!IsTeacherTeachingModuleSession(request.TeacherId, request.ModuleId, request.SessionId, request.ClassId))
        {
            var error = ErrorDescription.Error["E1150"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check if student has taken gpa teacher in this session or not
        var existedGpa = _context.GpaRecords
            .Include(g => g.Student)
            .Include(g => g.Teacher)
            .Include(g => g.Form)
            .Include(g => g.Class)
            .Include(g => g.Module)
            .Include(g => g.Session)
            .Include(g => g.GpaRecordsAnswers)
            .ThenInclude(gra => gra.Answer)
            .ThenInclude(a => a.Question)
            .FirstOrDefault(g =>
                g.StudentId == _user.Id && g.FormId == formId && g.ClassId == request.ClassId &&
                g.ModuleId == request.ModuleId && g.SessionId == request.SessionId && g.TeacherId == request.TeacherId);
        if (existedGpa != null)
        {
            var error = ErrorDescription.Error["E1145"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var gpaRecord = new GpaRecord()
        {
            StudentId = _user.Id,
            TeacherId = request.TeacherId,
            FormId = formId,
            ClassId = request.ClassId,
            ModuleId = request.ModuleId,
            SessionId = request.SessionId,
            Comment = request.Comment,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _context.GpaRecords.Add(gpaRecord);

        try
        {
            _context.SaveChanges();
        }
        catch
            (Exception e)
        {
            var error = ErrorDescription.Error["E1146"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // add data to gpa record answer
        foreach (var answerId in request.AnswerIds)
        {
            var gpaRecordAnswer = new GpaRecordAnswer()
            {
                GpaRecordId = gpaRecord.Id,
                AnswerId = answerId
            };
            _context.GpaRecordsAnswers.Add(gpaRecordAnswer);
        }

        try
        {
            _context.SaveChanges();
        }
        catch
            (Exception e)
        {
            var error = ErrorDescription.Error["E1147"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("GPA has taken successfully", null!));
    }

    // sro view gpa teacher by teacherId
    [HttpGet]
    [Route("api/gpa/teachers/{teacherId:int}")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult ViewGpaTeacherByTeacherId(int teacherId)
    {
        // check if teacher exists or not
        if (!IsTeacherExisted(teacherId))
        {
            var error = ErrorDescription.Error["E1140"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var gpaRecords = _context.GpaRecords
            .Include(g => g.Student)
            .Include(g => g.Teacher)
            .Include(g => g.Form)
            .Include(g => g.Class)
            .Include(g => g.Module)
            .Include(g => g.Session)
            .Include(g => g.GpaRecordsAnswers)
            .ThenInclude(gra => gra.Answer)
            .ThenInclude(a => a.Question)
            .Where(g => g.TeacherId == teacherId)
            .ToList();

        var gpaRecordAnswer = gpaRecords.Select(g => g.GpaRecordsAnswers).ToList();

        // get list comment
        var comments = gpaRecords.Select(g => g.Comment).ToList();

        // get answerNo by answerId
        var answerNo = new List<int>();
        foreach (var gpaRecordAnswers in gpaRecordAnswer)
        {
            foreach (var record in gpaRecordAnswers)
            {
                answerNo.Add(record.Answer.AnswerNo);
            }
        }

        var sum = 0;
        foreach (var answer in answerNo)
        {
            sum += answer;
        }

        var average = (double)sum / answerNo.Count;
        var gpaResponse = new GpaResponse()
        {
            AverageGpa = average, Comments = comments
        };
        return Ok(CustomResponse.Ok("GPA records has been retrieved successfully", gpaResponse));
    }

    // sro view gpa teacher by teacherId and classId
    [HttpGet]
    [Route("api/gpa/teachers/{teacherId:int}/classes/{classId:int}")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult ViewGpaTeacherByTeacherIdAndClassId(int teacherId, int classId)
    {
        // check if teacher exists or not
        if (!IsTeacherExisted(teacherId))
        {
            var error = ErrorDescription.Error["E1140"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check if class exists or not
        if (!IsClassExisted(classId))
        {
            var error = ErrorDescription.Error["E1139"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var gpaRecords = _context.GpaRecords
            .Include(g => g.Student)
            .Include(g => g.Teacher)
            .Include(g => g.Form)
            .Include(g => g.Class)
            .Include(g => g.Module)
            .Include(g => g.Session)
            .Include(g => g.GpaRecordsAnswers)
            .ThenInclude(gra => gra.Answer)
            .ThenInclude(a => a.Question)
            .Where(g => g.TeacherId == teacherId && g.ClassId == classId)
            .ToList();

        var gpaRecordAnswer = gpaRecords.Select(g => g.GpaRecordsAnswers).ToList();

        // get list comment
        var comments = gpaRecords.Select(g => g.Comment).ToList();

        // get answerNo by answerId
        var answerNo = new List<int>();
        foreach (var gpaRecordAnswers in gpaRecordAnswer)
        {
            foreach (var record in gpaRecordAnswers)
            {
                answerNo.Add(record.Answer.AnswerNo);
            }
        }

        var sum = 0;
        foreach (var answer in answerNo)
        {
            sum += answer;
        }

        var average = (double)sum / answerNo.Count;
        var gpaResponse = new GpaResponse()
        {
            AverageGpa = average, Comments = comments
        };
        return Ok(CustomResponse.Ok("GPA records has been retrieved successfully", gpaResponse));
    }

    // sro view gpa teacher by teacherId and moduleId
    [HttpGet]
    [Route("api/gpa/teachers/{teacherId:int}/modules/{moduleId:int}")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult ViewGpaTeacherByTeacherIdAndModuleId(int teacherId, int moduleId)
    {
        // check if teacher exists or not
        if (!IsTeacherExisted(teacherId))
        {
            var error = ErrorDescription.Error["E1140"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check if module exists or not
        if (!IsModuleExisted(moduleId))
        {
            var error = ErrorDescription.Error["E1141"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var gpaRecords = _context.GpaRecords
            .Include(g => g.Student)
            .Include(g => g.Teacher)
            .Include(g => g.Form)
            .Include(g => g.Class)
            .Include(g => g.Module)
            .Include(g => g.Session)
            .Include(g => g.GpaRecordsAnswers)
            .ThenInclude(gra => gra.Answer)
            .ThenInclude(a => a.Question)
            .Where(g => g.TeacherId == teacherId && g.ModuleId == moduleId)
            .ToList();

        var gpaRecordAnswer = gpaRecords.Select(g => g.GpaRecordsAnswers).ToList();

        // get list comment
        var comments = gpaRecords.Select(g => g.Comment).ToList();

        // get answerNo by answerId
        var answerNo = new List<int>();
        foreach (var gpaRecordAnswers in gpaRecordAnswer)
        {
            foreach (var record in gpaRecordAnswers)
            {
                answerNo.Add(record.Answer.AnswerNo);
            }
        }

        var sum = 0;
        foreach (var answer in answerNo)
        {
            sum += answer;
        }

        var average = (double)sum / answerNo.Count;
        var gpaResponse = new GpaResponse()
        {
            AverageGpa = average, Comments = comments
        };
        return Ok(CustomResponse.Ok("GPA records has been retrieved successfully", gpaResponse));
    }

    // sro view gpa teacher by teacherId, classId, moduleId and sessionId
    [HttpGet]
    [Route("api/gpa/teachers/{teacherId:int}/classes/{classId:int}/modules/{moduleId:int}/sessions/{sessionId:int}")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult ViewGpaTeacherByTeacherIdAndClassIdAndModuleIdAndSessionId(int teacherId, int classId,
        int moduleId, int sessionId)
    {
        // check if teacher exists or not
        if (!IsTeacherExisted(teacherId))
        {
            var error = ErrorDescription.Error["E1140"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check if class exists or not
        if (!IsClassExisted(classId))
        {
            var error = ErrorDescription.Error["E1139"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check if module exists or not
        if (!IsModuleExisted(moduleId))
        {
            var error = ErrorDescription.Error["E1141"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check if session exists or not
        if (!IsSessionExisted(sessionId))
        {
            var error = ErrorDescription.Error["E1142"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var gpaRecords = _context.GpaRecords
            .Include(g => g.Student)
            .Include(g => g.Teacher)
            .Include(g => g.Form)
            .Include(g => g.Class)
            .Include(g => g.Module)
            .Include(g => g.Session)
            .Include(g => g.GpaRecordsAnswers)
            .ThenInclude(gra => gra.Answer)
            .ThenInclude(a => a.Question)
            .Where(g => g.TeacherId == teacherId && g.ClassId == classId && g.ModuleId == moduleId &&
                        g.SessionId == sessionId)
            .ToList();

        var gpaRecordAnswer = gpaRecords.Select(g => g.GpaRecordsAnswers).ToList();

        // get list comment
        var comments = gpaRecords.Select(g => g.Comment).ToList();

        // get answerNo by answerId
        var answerNo = new List<int>();
        foreach (var gpaRecordAnswers in gpaRecordAnswer)
        {
            foreach (var record in gpaRecordAnswers)
            {
                answerNo.Add(record.Answer.AnswerNo);
            }
        }

        var sum = 0;
        foreach (var answer in answerNo)
        {
            sum += answer;
        }

        var average = (double)sum / answerNo.Count;
        var gpaResponse = new GpaResponse()
        {
            AverageGpa = average, Comments = comments
        };
        return Ok(CustomResponse.Ok("GPA records has been retrieved successfully", gpaResponse));
    }

    // get list class of teacher
    [HttpGet]
    [Route("api/gpa/teachers/{teacherId:int}/classes")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetListClassOfTeacher(int teacherId)
    {
        // check if teacher exists or not
        if (!IsTeacherExisted(teacherId))
        {
            var error = ErrorDescription.Error["E1140"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var classes = _context.Classes
            .Include(c => c.GpaRecords)
            .ThenInclude(gr => gr.Teacher)
            .Where(c => c.GpaRecords.Select(gr => gr.TeacherId).Contains(teacherId))
            .Select(c => new ClassGpaResponse()
            {
                Id = c.GpaRecords.Select(gr => gr.TeacherId).Contains(teacherId) ? c.Id : 0,
                Name = c.GpaRecords.Select(gr => gr.TeacherId).Contains(teacherId) ? c.Name : null
            })
            .ToList();

        return Ok(CustomResponse.Ok("List classes of teacher has been retrieved successfully", classes));
    }

    // get list module of teacher
    [HttpGet]
    [Route("api/gpa/teachers/{teacherId:int}/modules")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetListModuleOfTeacher(int teacherId)
    {
        // check if teacher exists or not
        if (!IsTeacherExisted(teacherId))
        {
            var error = ErrorDescription.Error["E1140"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var modules = _context.Modules
            .Include(m => m.GpaRecords)
            .ThenInclude(gr => gr.Teacher)
            .Where(m => m.GpaRecords.Select(gr => gr.TeacherId).Contains(teacherId))
            .Select(m => new ModuleGpaResponse()
            {
                Id = m.GpaRecords.Select(gr => gr.TeacherId).Contains(teacherId) ? m.Id : 0,
                ModuleName = m.GpaRecords.Select(gr => gr.TeacherId).Contains(teacherId) ? m.ModuleName : null
            })
            .ToList();

        return Ok(CustomResponse.Ok("List modules of teacher has been retrieved successfully", modules));
    }

    // is class existed
    private bool IsClassExisted(int classId)
    {
        return _context.Classes.Any(c => c.Id == classId && c.CenterId == _user.CenterId);
    }

    // is teacher existed
    private bool IsTeacherExisted(int teacherId)
    {
        return _context.Teachers
            .Include(t => t.User)
            .Any(t => t.UserId == teacherId && t.User.CenterId == _user.CenterId);
    }

    // is module existed
    private bool IsModuleExisted(int moduleId)
    {
        return _context.Modules.Any(m => m.Id == moduleId && m.CenterId == _user.CenterId);
    }

    // is session existed
    private bool IsSessionExisted(int sessionId)
    {
        return _context.Sessions.Any(s => s.Id == sessionId);
    }

    // is student in class
    private bool IsStudentInClass(int classId)
    {
        return _context.StudentsClasses
            .Any(sc => sc.ClassId == classId && sc.StudentId == _user.Id && sc.IsActive);
    }

    // is teacher is teaching this class
    private bool IsTeacherInClass(int classId, int teacherId)
    {
        return _context.ClassSchedules
            .Include(cs => cs.Teacher)
            .Include(cs => cs.Class)
            .Any(cs => cs.ClassId == classId && cs.TeacherId == teacherId);
    }

    // is module is teaching in this class
    private bool IsModuleInClass(int classId, int moduleId)
    {
        return _context.ClassSchedules
            .Include(cs => cs.Class)
            .Include(cs => cs.Module)
            .Any(cs => cs.ClassId == classId && cs.ModuleId == moduleId);
    }

    // is session is teaching in this class
    private bool IsSessionInClass(int classId, int sessionId, int moduleId)
    {
        return _context.Sessions
            .Include(s => s.ClassSchedule)
            .Any(s => s.ClassSchedule.ClassId == classId && s.Id == sessionId && s.ClassSchedule.ModuleId == moduleId);
    }

    // is teacher teaching this module session in class
    private bool IsTeacherTeachingModuleSession(int teacherId, int moduleId, int sessionId, int classId)
    {
        return _context.Sessions
            .Include(s => s.ClassSchedule)
            .Any(s => s.ClassSchedule.TeacherId == teacherId && s.Id == sessionId &&
                      s.ClassSchedule.ModuleId == moduleId && s.ClassSchedule.ClassId == classId);
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