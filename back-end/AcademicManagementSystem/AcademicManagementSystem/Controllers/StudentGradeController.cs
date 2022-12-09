using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.StudentGradeController;
using AcademicManagementSystem.Models.StudentGradeController.StudentGradeModel;
using AcademicManagementSystem.Models.StudentGradeController.StudentGradeModel.GradeItem;
using AcademicManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class GradeStudentController : ControllerBase
{
    private readonly AmsContext _context;
    private readonly IUserService _userService;
    private const int PracticeExam = 5;
    private const int TheoryExam = 6;
    private const int PracticeExamResit = 7;
    private const int TheoryExamResit = 8;
    private const int ExamTypeTheory = 1;
    private const int ExamTypeNoTakeExam = 4;
    private const int ClassStatusMerged = 6;

    public GradeStudentController(AmsContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    // student get their grades by class and module
    [HttpGet]
    [Route("api/classes/{classId:int}/modules/{moduleId:int}/grades-students/students")]
    [Authorize(Roles = "student")]
    public IActionResult GetStudentGrades(int classId, int moduleId)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());

        var clazz = _context.Classes
            .Include(c => c.CourseFamily)
            .Include(c => c.CourseFamily.Courses)
            .ThenInclude(c => c.CoursesModulesSemesters)
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Module)
            .FirstOrDefault(c => c.Id == classId);

        if (clazz == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }
        
        if (clazz.ClassStatusId == ClassStatusMerged)
        {
            var error = ErrorDescription.Error["E0310"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var student = _context.Students
            .Include(s => s.StudentsClasses)
            .Include(s => s.User)
            .Where(s => s.UserId == userId)
            .FirstOrDefault(s => s.StudentsClasses.Any(sc => sc.ClassId == classId));

        if (student is not { IsDraft: false })
        {
            return NotFound(CustomResponse.NotFound("Student not found in class"));
        }

        var isModuleForThisClass = clazz.CourseFamily.Courses
            .Select(c => c.CoursesModulesSemesters)
            .Any(listCms => listCms
                .Any(cms => cms.ModuleId == moduleId));

        if (!isModuleForThisClass)
        {
            return NotFound(CustomResponse.NotFound("Module not for this class"));
        }

        var moduleProgressScores = _context.GradeItems
            .Include(gi => gi.GradeCategoryModule)
            .Include(gi => gi.GradeCategoryModule.GradeCategory)
            .Where(gi => gi.GradeCategoryModule.ModuleId == moduleId)
            .Select(gi => new StudentGradeForStudentResponse()
            {
                GradeCategoryId = gi.GradeCategoryModule.GradeCategory.Id,
                GradeCategoryName = gi.GradeCategoryModule.GradeCategory.Name,
                TotalWeight = gi.GradeCategoryModule.TotalWeight,
                QuantityGradeItem = gi.GradeCategoryModule.QuantityGradeItem,
                GradeItem = new GradeItemWithStudentScoreResponse()
                {
                    Id = gi.Id,
                    Name = gi.Name,
                    Grade = gi.StudentGrades
                        .Where(sg => sg.StudentId == userId && sg.ClassId == classId)
                        .Select(sg => sg.Grade)
                        .FirstOrDefault(),
                    Comment = gi.StudentGrades
                        .Where(sg => sg.StudentId == userId && sg.ClassId == classId)
                        .Select(sg => sg.Comment)
                        .FirstOrDefault()
                }
            });

        return Ok(CustomResponse.Ok("Student get grades successfully", moduleProgressScores));
    }

    // student get their grades by class and module
    [HttpGet]
    [Route("api/classes/{classId:int}/modules/{moduleId:int}/students/{studentId:int}/grades")]
    [Authorize(Roles = "sro")]
    public IActionResult SroGetGradesOfSpecificStudent(int classId, int moduleId, int studentId)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        var sro = _context.Sros.Include(s => s.User.Center).First(s => s.UserId == userId);

        var clazz = _context.Classes
            .Include(c => c.CourseFamily)
            .Include(c => c.CourseFamily.Courses)
            .ThenInclude(c => c.CoursesModulesSemesters)
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Module)
            .FirstOrDefault(c => c.Id == classId);

        if (clazz == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }

        if (sro.User.Center.Id != clazz.Center.Id)
        {
            return Unauthorized(CustomResponse.Unauthorized("You are not authorized to access this resource"));
        }

        if (clazz.ClassStatusId == ClassStatusMerged)
        {
            var error = ErrorDescription.Error["E0310"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var isModuleForThisClass = clazz.CourseFamily.Courses
            .Select(c => c.CoursesModulesSemesters)
            .Any(listCms => listCms
                .Any(cms => cms.ModuleId == moduleId));

        if (!isModuleForThisClass)
        {
            return NotFound(CustomResponse.NotFound("Module not for this class"));
        }

        var student = _context.Students
            .Include(s => s.StudentsClasses)
            .Include(s => s.User)
            .Where(s => s.UserId == studentId)
            .FirstOrDefault(s => s.StudentsClasses.Any(sc => sc.ClassId == classId));

        if (student is not { IsDraft: false })
        {
            return NotFound(CustomResponse.NotFound("Student not found in class"));
        }

        var moduleProgressScores = _context.GradeItems
            .Include(gi => gi.GradeCategoryModule)
            .Include(gi => gi.GradeCategoryModule.GradeCategory)
            .Where(gi => gi.GradeCategoryModule.ModuleId == moduleId)
            .Select(gi => new StudentGradeForStudentResponse()
            {
                GradeCategoryId = gi.GradeCategoryModule.GradeCategory.Id,
                GradeCategoryName = gi.GradeCategoryModule.GradeCategory.Name,
                TotalWeight = gi.GradeCategoryModule.TotalWeight,
                QuantityGradeItem = gi.GradeCategoryModule.QuantityGradeItem,
                GradeItem = new GradeItemWithStudentScoreResponse()
                {
                    Id = gi.Id,
                    Name = gi.Name,
                    Grade = gi.StudentGrades
                        .Where(sg => sg.StudentId == studentId && sg.ClassId == classId)
                        .Select(sg => sg.Grade)
                        .FirstOrDefault(),
                    Comment = gi.StudentGrades
                        .Where(sg => sg.StudentId == studentId && sg.ClassId == classId)
                        .Select(sg => sg.Comment)
                        .FirstOrDefault()
                }
            });

        return Ok(CustomResponse.Ok("SRO get grades of specific student successfully", moduleProgressScores));
    }

    // SRO get all grade of students in class 
    [HttpGet]
    [Route("api/classes/{classId:int}/modules/{moduleId:int}/grades-students/sros")]
    [Authorize(Roles = "sro")]
    public IActionResult SroGetListStudentGrades(int classId, int moduleId)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());

        var sro = _context.Sros.Include(s => s.User.Center).First(s => s.UserId == userId);

        var clazz = _context.Classes
            .Include(c => c.CourseFamily)
            .Include(c => c.CourseFamily.Courses)
            .ThenInclude(c => c.CoursesModulesSemesters)
            .Include(c => c.Center)
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Module)
            .FirstOrDefault(c => c.Id == classId);

        if (clazz == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }

        if (sro.User.Center.Id != clazz.Center.Id)
        {
            return Unauthorized(CustomResponse.Unauthorized("You are not authorized to access this resource"));
        }

        var isModuleForThisClass = clazz.CourseFamily.Courses
            .Select(c => c.CoursesModulesSemesters)
            .Any(listCms => listCms
                .Any(cms => cms.ModuleId == moduleId));

        if (!isModuleForThisClass)
        {
            return NotFound(CustomResponse.NotFound("Module not for this class"));
        }

        var moduleProgressScores = GetGradesOfStudentsInClass(clazz, moduleId);
        return Ok(CustomResponse.Ok("SRO get grade of all students in class successfully", moduleProgressScores));
    }

    // teacher get all grade of students in class 
    [HttpGet]
    [Route("api/classes/{classId:int}/modules/{moduleId:int}/grades-students/teachers")]
    [Authorize(Roles = "teacher")]
    public IActionResult TeacherGetListStudentGrades(int classId, int moduleId)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());

        var clazz = _context.Classes
            .Include(c => c.CourseFamily)
            .Include(c => c.CourseFamily.Courses)
            .ThenInclude(c => c.CoursesModulesSemesters)
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Module)
            .FirstOrDefault(c => c.Id == classId);

        if (clazz == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }

        var isTeacherTeachThisClass = clazz.ClassSchedules.Any(cs =>
            cs.ClassId == classId && cs.ModuleId == moduleId && cs.TeacherId == userId);

        // check teacher is teach this schedule (class and module)
        if (!isTeacherTeachThisClass)
        {
            return Unauthorized(CustomResponse.Unauthorized("You are not authorized to access this resource"));
        }

        var isModuleForThisClass = clazz.CourseFamily.Courses
            .Select(c => c.CoursesModulesSemesters)
            .Any(listCms => listCms
                .Any(cms => cms.ModuleId == moduleId));

        if (!isModuleForThisClass)
        {
            return NotFound(CustomResponse.NotFound("Module not for this class"));
        }

        var moduleProgressScores = GetGradesOfStudentsInClass(clazz, moduleId);
        return Ok(CustomResponse.Ok("Teacher get progress scores of students successfully", moduleProgressScores));
    }

    [HttpGet]
    [Route("api/students/semesters/modules")]
    [Authorize(Roles = "student")]
    public IActionResult GetLearningModulesInSemestersOfStudent()
    {
        var userId = Convert.ToInt32(_userService.GetUserId());

        // get module that have been started in class, (don't get merged classes)
        var classSchedule = _context.ClassSchedules
            .Include(cs => cs.Class)
            .Include(cs => cs.Class.StudentsClasses)
            .Include(cs => cs.Module)
            .Include(cs => cs.Module.CoursesModulesSemesters)
            .ThenInclude(cms => cms.Course)
            .Include(cs => cs.Module.CoursesModulesSemesters)
            .ThenInclude(cms => cms.Semester)
            .Where(cs => cs.StartDate.Date <= DateTime.Today && cs.Class.ClassStatusId != ClassStatusMerged &&
                         cs.Class.StudentsClasses.Any(sc => sc.StudentId == userId));

        var semesters = classSchedule.Select(cs => cs.Module.CoursesModulesSemesters)
            .SelectMany(cms => cms)
            // .Where(cms => cms.CourseCode == student.CourseCode)
            .Select(cms => cms.Semester)
            .Select(s => new SemesterWithModuleResponse()
            {
                Id = s.Id,
                Name = s.Name,
                Modules = classSchedule.Where(cs => cs.Module.CoursesModulesSemesters.Any(cms =>
                        // cms.CourseCode == student.CourseCode &&
                        cms.SemesterId == s.Id))
                    .Select(cs => new ModuleWithClassResponse()
                    {
                        Id = cs.Module.Id,
                        Name = cs.Module.ModuleName,

                        Class = new BasicClassResponse()
                        {
                            Id = cs.Class.Id,
                            Name = cs.Class.Name,
                        },
                    }).ToList()
            });

        // distinct semester
        var distinctSemesters = semesters.GroupBy(s => s.Id)
            .Select(s => s.First())
            .ToList();

        return Ok(CustomResponse.Ok("Student get semesters and modules successfully", distinctSemesters));
    }

    [HttpGet]
    [Route("api/students/{studentId:int}/semesters/modules")]
    [Authorize(Roles = "sro")]
    public IActionResult GetLearningModulesInSemestersOfStudent(int studentId)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        var userSro = _context.Users.First(u => u.Id == userId);
        var userStudent = _context.Users.FirstOrDefault(u => u.Id == studentId);
        if (userStudent == null || userStudent.CenterId != userSro.CenterId)
        {
            return NotFound(CustomResponse.NotFound("Student not found in this center"));
        }

        // get module that have been started in class, (don't get merged classes)
        var classSchedule = _context.ClassSchedules
            .Include(cs => cs.Class)
            .Include(cs => cs.Class.StudentsClasses)
            .Include(cs => cs.Module)
            .Include(cs => cs.Module.CoursesModulesSemesters)
            .ThenInclude(cms => cms.Course)
            .Include(cs => cs.Module.CoursesModulesSemesters)
            .ThenInclude(cms => cms.Semester)
            .Where(cs => cs.StartDate.Date <= DateTime.Today && cs.Class.ClassStatusId != ClassStatusMerged &&
                         cs.Class.StudentsClasses.Any(sc => sc.StudentId == userStudent.Id));

        var semesters = classSchedule.Select(cs => cs.Module.CoursesModulesSemesters)
            .SelectMany(cms => cms)
            // .Where(cms => cms.CourseCode == student.CourseCode)
            .Select(cms => cms.Semester)
            .Select(s => new SemesterWithModuleResponse()
            {
                Id = s.Id,
                Name = s.Name,
                Modules = classSchedule.Where(cs => cs.Module.CoursesModulesSemesters.Any(cms =>
                        // cms.CourseCode == student.CourseCode &&
                        cms.SemesterId == s.Id))
                    .Select(cs => new ModuleWithClassResponse()
                    {
                        Id = cs.Module.Id,
                        Name = cs.Module.ModuleName,

                        Class = new BasicClassResponse()
                        {
                            Id = cs.Class.Id,
                            Name = cs.Class.Name,
                        },
                    }).ToList()
            });

        // distinct semester
        var distinctSemesters = semesters.GroupBy(s => s.Id)
            .Select(s => s.First())
            .ToList();

        return Ok(CustomResponse.Ok("Sro get semesters of student and modules successfully", distinctSemesters));
    }

    [HttpPost]
    [Route("api/classes/{classId:int}/modules/{moduleId:int}/grades/sros")]
    [Authorize(Roles = "sro")]
    public IActionResult SroUpdateGradeStudentsInClass(int classId, int moduleId,
        [FromBody] List<StudentGradeRequest> request)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        var user = _context.Users.Include(u => u.Sro).First(u => u.Id == userId);

        // get class that have logged in student in this class and modules for this class
        var classContext = _context.Classes
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Module)
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Sessions)
            .Include(c => c.Center)
            .Include(c => c.StudentsClasses)
            .ThenInclude(sc => sc.Student)
            .FirstOrDefault(c => c.Id == classId);

        if (classContext == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }

        var isModuleInClass = classContext.ClassSchedules.Any(cs => cs.ModuleId == moduleId);

        if (!isModuleInClass)
        {
            return NotFound(CustomResponse.NotFound("Module not found in this class schedule"));
        }

        // check user center is same center of class
        if (classContext.CenterId != user.CenterId)
        {
            return Unauthorized(CustomResponse.Unauthorized("You are not authorized to access this resource"));
        }

        // can't update grades if module exam type is don't take exam
        // var module = classContext.ClassSchedules.First(cs => cs.ModuleId == moduleId).Module;
        // if (module.ExamType is ExamTypeNoTakeExam)
        // {
        //     var error = ErrorDescription.Error["E0307"];
        //     return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        // }

        // // first session in class schedule
        // var firstSession = classContext.ClassSchedules
        //     .First(cs => cs.ModuleId == moduleId)
        //     .Sessions.OrderBy(s => s.LearningDate).First();
        //
        // // last session in class schedule
        // var lastSession = classContext.ClassSchedules
        //     .First(cs => cs.ModuleId == moduleId)
        //     .Sessions.OrderBy(s => s.LearningDate).Last();
        //
        // // check module is start learning and no more than 5 days after last session
        // var canUpdateModule = classContext.ClassSchedules
        //     .Any(cs => cs.ModuleId == moduleId &&
        //                firstSession.LearningDate.Date <= DateTime.Today &&
        //                DateTime.Today <= lastSession.LearningDate.AddDays(5));
        //
        var canUpdateGrade =
            classContext.ClassSchedules.Any(cs =>
                cs.StartDate <= DateTime.Today && cs.ModuleId == moduleId);

        if (!canUpdateGrade)
        {
            var error = ErrorDescription.Error["E0301"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // remove duplicate if have any studentId and gradeItemId
        var distinctRequest = request.GroupBy(r => new { r.StudentId, r.GradeItemId })
            .Select(r => r.First())
            .ToList();

        // get student in class (student that is active in class and not is draft)) 
        var studentsIdInClass = classContext.StudentsClasses.Where(sc => sc.IsActive && !sc.Student.IsDraft)
            .Select(sc => sc.StudentId).ToList();

        var requestStudentsId = distinctRequest.Select(r => r.StudentId).ToList();

        // students from request that not in this class
        var requestStudentIdNotInClass = requestStudentsId.Except(studentsIdInClass).ToList();

        if (requestStudentIdNotInClass.Any())
        {
            var error = ErrorDescription.Error["E0302"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // get grade items of module
        var gradeItemsOfModule = _context.GradeItems
            .Include(gi => gi.GradeCategoryModule)
            .Where(gi => gi.GradeCategoryModule.ModuleId == moduleId);

        // get gradeItemsId of module
        var gradeItemsIdOfModule = gradeItemsOfModule.Select(gi => gi.Id).ToList();

        // get gradeItemsId from request
        var requestGradeItemsId = distinctRequest.Select(r => r.GradeItemId).ToList();

        // gradeItems from request that not in this module
        var requestGradeItemsIdNotInModule = requestGradeItemsId.Except(gradeItemsIdOfModule).ToList();

        if (requestGradeItemsIdNotInModule.Any())
        {
            var error = ErrorDescription.Error["E0303"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var module = classContext.ClassSchedules.First(cs => cs.ModuleId == moduleId).Module;
        var maxTheoryGrade = module.MaxTheoryGrade;
        var maxPracticeGrade = module.MaxPracticalGrade;

        var gradesOfAllStudentInThisClass = _context.StudentGrades
            .Where(sg => sg.ClassId == classId);

        foreach (var r in distinctRequest)
        {
            var gradeItem = gradeItemsOfModule.First(gi => gi.Id == r.GradeItemId);
            // except exams, limit grade is 10
            if (gradeItem.GradeCategoryModule.GradeCategoryId != PracticeExam &&
                gradeItem.GradeCategoryModule.GradeCategoryId != TheoryExam &&
                gradeItem.GradeCategoryModule.GradeCategoryId != PracticeExamResit &&
                gradeItem.GradeCategoryModule.GradeCategoryId != TheoryExamResit &&
                r.Grade is < 0 or > 10)
            {
                var error = ErrorDescription.Error["E0304"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            // exams will depends on max grade practical of module
            if (gradeItem.GradeCategoryModule.GradeCategoryId is PracticeExam or PracticeExamResit
                && maxPracticeGrade != null && (r.Grade is < 0 || r.Grade > maxPracticeGrade))
            {
                var error = ErrorDescription.Error["E0309"];
                return BadRequest(CustomResponse.BadRequest(error.Message + "(" + maxPracticeGrade + ")", error.Type));
            }

            // exams will depends on max grade theory of module
            if (gradeItem.GradeCategoryModule.GradeCategoryId is TheoryExam or TheoryExamResit
                && maxTheoryGrade != null && (r.Grade is < 0 || r.Grade > maxTheoryGrade))
            {
                var error = ErrorDescription.Error["E0308"];
                return BadRequest(CustomResponse.BadRequest(error.Message + "(" + maxTheoryGrade + ")", error.Type));
            }

            var studentGrade = new StudentGrade()
            {
                ClassId = classId,
                StudentId = r.StudentId,
                GradeItemId = r.GradeItemId,
                Grade = r.Grade,
                Comment = r.Comment,
            };

            var isExist = gradesOfAllStudentInThisClass.Any(sg => sg.StudentId == r.StudentId
                                                                  && sg.GradeItemId == r.GradeItemId);

            // if have any record in student grade of this student by classId, studentId, gradeItemId then update else add
            if (!isExist)
            {
                _context.StudentGrades.Add(studentGrade);
            }
            else
            {
                _context.StudentGrades.Update(studentGrade);
            }
        }

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0305"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var response = GetGradesOfStudentsInClass(classContext, moduleId);

        return Ok(CustomResponse.Ok("Sro update progress scores successfully", response));
    }

    [HttpPost]
    [Route("api/classes/{classId:int}/modules/{moduleId:int}/grades/teachers")]
    [Authorize(Roles = "teacher")]
    public IActionResult TeacherUpdateGradeStudentsInClass(int classId, int moduleId,
        [FromBody] List<StudentGradeRequest> request)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        var user = _context.Users.Include(u => u.Teacher).First(u => u.Id == userId);

        // get class that have logged in student in this class and modules for this class
        var classContext = _context.Classes
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Module)
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Sessions)
            .Include(c => c.Center)
            .Include(c => c.StudentsClasses)
            .ThenInclude(sc => sc.Student)
            .FirstOrDefault(c => c.Id == classId);

        if (classContext == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }

        var isModuleInClass = classContext.ClassSchedules.Any(cs => cs.ModuleId == moduleId);

        if (!isModuleInClass)
        {
            return NotFound(CustomResponse.NotFound("Module not found int this class schedule"));
        }

        var module = classContext.ClassSchedules.First(cs => cs.ModuleId == moduleId).Module;

        switch (module.ExamType)
        {
            // teacher can't update grades if module exam type is theory (just sro)
            case ExamTypeTheory:
                return Unauthorized(CustomResponse.Unauthorized("You are not able to update grades for this module"));

            // // can't update grades if module exam type is don't take exam
            // case ExamTypeNoTakeExam:
            // {
            //     var error = ErrorDescription.Error["E0307"];
            //     return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            // }
        }

        // check user center is same center of class
        if (classContext.CenterId != user.CenterId)
        {
            return Unauthorized(CustomResponse.Unauthorized("You are not authorized to access this resource"));
        }

        var isTeacherTeachThisClass = classContext.ClassSchedules.Any(cs =>
            cs.ClassId == classId && cs.ModuleId == moduleId && cs.TeacherId == user.Id);

        // check teacher is teach this schedule (class and module)
        if (!isTeacherTeachThisClass)
        {
            return Unauthorized(CustomResponse.Unauthorized("You are not authorized to access this resource"));
        }

        // first session in class schedule
        var firstSession = classContext.ClassSchedules
            .First(cs => cs.ModuleId == moduleId)
            .Sessions.OrderBy(s => s.LearningDate).First();

        // last session in class schedule
        var lastSession = classContext.ClassSchedules
            .First(cs => cs.ModuleId == moduleId)
            .Sessions.OrderBy(s => s.LearningDate).Last();

        // check module is start learning and no more than 3 days after last session
        var canUpdateModule = classContext.ClassSchedules
            .Any(cs => cs.ModuleId == moduleId &&
                       firstSession.LearningDate.Date <= DateTime.Today &&
                       DateTime.Today <= lastSession.LearningDate.Date);

        if (!canUpdateModule)
        {
            var error = ErrorDescription.Error["E0300"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // remove duplicate if have any studentId and gradeItemId
        var distinctRequest = request.GroupBy(r => new { r.StudentId, r.GradeItemId })
            .Select(r => r.First())
            .ToList();

        // get student in class (student that is active in class and not is draft)) 
        var studentsIdInClass = classContext.StudentsClasses.Where(sc => sc.IsActive && !sc.Student.IsDraft)
            .Select(sc => sc.StudentId).ToList();

        var requestStudentsId = distinctRequest.Select(r => r.StudentId).ToList();

        // students from request that not in this class
        var requestStudentIdNotInClass = requestStudentsId.Except(studentsIdInClass).ToList();

        if (requestStudentIdNotInClass.Any())
        {
            var error = ErrorDescription.Error["E0302"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // get grade items of module
        var gradeItemsOfModule = _context.GradeItems
            .Include(gi => gi.GradeCategoryModule)
            .Where(gi => gi.GradeCategoryModule.ModuleId == moduleId);

        // get gradeItemsId of module
        var gradeItemsIdOfModule = gradeItemsOfModule.Select(gi => gi.Id).ToList();

        // get gradeItemsId from request
        var requestGradeItemsId = distinctRequest.Select(r => r.GradeItemId).ToList();

        // gradeItems from request that not in this module
        var requestGradeItemsIdNotInModule = requestGradeItemsId.Except(gradeItemsIdOfModule).ToList();

        if (requestGradeItemsIdNotInModule.Any())
        {
            var error = ErrorDescription.Error["E0303"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // get all grades of all student in this class
        var gradesOfAllStudentInThisClass = _context.StudentGrades
            .Where(sg => sg.ClassId == classId);

        foreach (var r in distinctRequest)
        {
            if (r.Grade is < 0 or > 10)
            {
                var error = ErrorDescription.Error["E0304"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            var studentGrade = new StudentGrade()
            {
                ClassId = classId,
                StudentId = r.StudentId,
                GradeItemId = r.GradeItemId,
                Grade = r.Grade,
                Comment = r.Comment,
            };

            // if have any record in student grade of this student by classId, studentId, gradeItemId then update else add
            var isExist = gradesOfAllStudentInThisClass.Any(sg => sg.StudentId == r.StudentId
                                                                  && sg.GradeItemId == r.GradeItemId);
            if (!isExist)
            {
                _context.StudentGrades.Add(studentGrade);
            }
            else
            {
                _context.StudentGrades.Update(studentGrade);
            }

            // if grade item is exams -> error (teacher can't update these grade items)
            var gradeItem = gradeItemsOfModule.First(gi => gi.Id == r.GradeItemId);
            if (gradeItem.GradeCategoryModule.GradeCategoryId
                is PracticeExam or TheoryExam or PracticeExamResit or TheoryExamResit)
            {
                var error = ErrorDescription.Error["E0306"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }
        }

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0305"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var response = GetGradesOfStudentsInClass(classContext, moduleId);

        return Ok(CustomResponse.Ok("Teacher update progress scores successfully", response));
    }

    // get grades of all students in class
    private IQueryable<ListStudentGradeResponse> GetGradesOfStudentsInClass(Class clazz, int moduleId)
    {
        return _context.Modules
            .Include(m => m.ClassSchedules)
            .ThenInclude(cs => cs.Class)
            .ThenInclude(c => c.StudentsClasses)
            .Include(m => m.GradeCategoryModule)
            .ThenInclude(gcm => gcm.GradeCategory)
            .Include(m => m.GradeCategoryModule)
            .ThenInclude(gcm => gcm.GradeItems)
            .ThenInclude(gi => gi.StudentGrades)
            .Where(m => m.Id == moduleId)
            .Select(m => new ListStudentGradeResponse()
            {
                Class = new BasicClassResponse()
                {
                    Id = clazz.Id,
                    Name = clazz.Name
                },

                Module = new BasicModuleResponse()
                {
                    Id = m.Id,
                    Name = m.ModuleName
                },

                Students = m.ClassSchedules.SelectMany(cs => cs.Class.StudentsClasses)
                    .Where(cs => cs.ClassId == clazz.Id && cs.IsActive && !cs.Student.IsDraft)
                    .Select(sc => new StudentInfoAndGradeResponse()
                    {
                        UserId = sc.Student.UserId,
                        EnrollNumber = sc.Student.EnrollNumber,
                        EmailOrganization = sc.Student.User.EmailOrganization,
                        FirstName = sc.Student.User.FirstName,
                        LastName = sc.Student.User.LastName,
                        Avatar = sc.Student.User.Avatar,
                        GradeCategories = m.GradeCategoryModule
                            .Select(gcm => new GradeCategoryWithItemsResponse()
                            {
                                Id = gcm.GradeCategory.Id,
                                Name = gcm.GradeCategory.Name,
                                TotalWeight = gcm.TotalWeight,
                                QuantityGradeItem = gcm.QuantityGradeItem,
                                GradeItems = gcm.GradeItems
                                    .Select(gi => new GradeItemWithStudentScoreResponse()
                                    {
                                        Id = gi.Id,
                                        Name = gi.Name,
                                        Grade = gi.StudentGrades.FirstOrDefault(sg =>
                                            sg.StudentId == sc.Student.UserId && sg.ClassId == clazz.Id)!.Grade,
                                        Comment = gi.StudentGrades.FirstOrDefault(sg =>
                                            sg.StudentId == sc.Student.UserId && sg.ClassId == clazz.Id)!.Comment
                                    })
                                    .ToList()
                            }).ToList()
                    }).ToList()
            });
    }

    private IQueryable<StudentGradeResponse> GetGradesOfSpecificStudent(Class clazz, int moduleId, Student student)
    {
        return _context.Modules
            .Include(m => m.GradeCategoryModule)
            .ThenInclude(gcm => gcm.GradeCategory)
            .Include(m => m.GradeCategoryModule)
            .ThenInclude(gcm => gcm.GradeItems)
            .ThenInclude(gi => gi.StudentGrades)
            .ThenInclude(sg => sg.Class)
            .Where(m => m.Id == moduleId)
            .Select(m => new StudentGradeResponse()
            {
                Class = new BasicClassResponse()
                {
                    Id = clazz.Id,
                    Name = clazz.Name
                },

                Module = new BasicModuleResponse()
                {
                    Id = m.Id,
                    Name = m.ModuleName
                },

                Student = new StudentInfoAndGradeResponse()
                {
                    UserId = student.UserId,
                    EnrollNumber = student.EnrollNumber,
                    EmailOrganization = student.User.EmailOrganization,
                    FirstName = student.User.FirstName,
                    LastName = student.User.LastName,
                    Avatar = student.User.Avatar,
                    GradeCategories = m.GradeCategoryModule
                        .Select(gcm => new GradeCategoryWithItemsResponse()
                        {
                            Id = gcm.GradeCategory.Id,
                            Name = gcm.GradeCategory.Name,
                            TotalWeight = gcm.TotalWeight,
                            QuantityGradeItem = gcm.QuantityGradeItem,
                            GradeItems = gcm.GradeItems
                                .Select(gi => new GradeItemWithStudentScoreResponse()
                                {
                                    Id = gi.Id,
                                    Name = gi.Name,
                                    Grade = gi.StudentGrades.FirstOrDefault(sg =>
                                        sg.StudentId == student.UserId && sg.ClassId == clazz.Id)!.Grade,
                                    Comment = gi.StudentGrades.FirstOrDefault(sg =>
                                        sg.StudentId == student.UserId && sg.ClassId == clazz.Id)!.Comment
                                })
                                .ToList()
                        }).ToList()
                },
            });
    }
}