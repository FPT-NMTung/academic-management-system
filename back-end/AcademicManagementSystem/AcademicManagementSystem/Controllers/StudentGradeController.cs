using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.StudentGradeController;
using AcademicManagementSystem.Models.StudentGradeController.StatisticModel;
using AcademicManagementSystem.Models.StudentGradeController.StudentGradeModel;
using AcademicManagementSystem.Models.StudentGradeController.StudentGradeModel.GradeItem;
using AcademicManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class StudentGradeController : ControllerBase
{
    private readonly AmsContext _context;
    private readonly IUserService _userService;
    private const int PracticeExam = 5;
    private const int TheoryExam = 6;
    private const int PracticeExamResit = 7;
    private const int TheoryExamResit = 8;
    private const int ClassStatusMerged = 6;
    private const int ExamTypeTe = 1;
    private const int ExamTypePe = 2;
    private const int ExamTypeBothPeAndTe = 3;
    private const int ExamTypeNoTakeExam = 4;
    private const int GradeToPass = 5;

    public StudentGradeController(AmsContext context, IUserService userService)
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

    // sro get specific grades of student by class and module
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
            var error = ErrorDescription.Error["E0600"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // if (clazz.ClassStatusId == ClassStatusMerged)
        // {
        //     var error = ErrorDescription.Error["E0401"];
        //     return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        // }

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
            var error = ErrorDescription.Error["E0600"];
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
            var error = ErrorDescription.Error["E0600"];
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

        var moduleProgressScores = GetGradesOfStudentsInClass(clazz, moduleId);
        return Ok(CustomResponse.Ok("Teacher get progress scores of students successfully", moduleProgressScores));
    }

    [HttpGet]
    [Route("api/students/semesters/modules")]
    [Authorize(Roles = "student")]
    public IActionResult GetLearningModulesInSemestersOfStudent()
    {
        var userId = Convert.ToInt32(_userService.GetUserId());

        // get module that have been started in class
        var classSchedule = _context.ClassSchedules
            .Include(cs => cs.Class)
            .Include(cs => cs.Sessions)
            .ThenInclude(s => s.Attendances)
            .Include(cs => cs.Class.StudentsClasses)
            .Include(cs => cs.Module)
            .Include(cs => cs.Module.CoursesModulesSemesters)
            .ThenInclude(cms => cms.Course)
            .Include(cs => cs.Module.CoursesModulesSemesters)
            .ThenInclude(cms => cms.Semester)
            .Where(cs => cs.StartDate.Date <= DateTime.Today &&
                         cs.Class.StudentsClasses.Any(sc => sc.StudentId == userId));

        var semesters = classSchedule.Select(cs => cs.Module.CoursesModulesSemesters)
            .SelectMany(cms => cms)
            // .Where(cms => cms.CourseCode == student.CourseCode)
            .Select(cms => cms.Semester)
            .Select(s => new SemesterWithModuleResponse()
            {
                Id = s.Id,
                Name = s.Name,
                Modules = classSchedule.Where(cs =>
                        cs.Sessions.Any(se => se.Attendances.Any(a => a.StudentId == userId)) &&
                        cs.Module.CoursesModulesSemesters.Any(cms =>
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

        var response = new List<SemesterWithModuleResponse>();
        foreach (var semester in distinctSemesters)
        {
            if (semester.Modules.Any())
            {
                response.Add(semester);
            }
        }

        return Ok(CustomResponse.Ok("Student get semesters and modules successfully", response));
    }

    [HttpGet]
    [Route("api/students/{studentId:int}/semesters/modules")]
    [Authorize(Roles = "sro,teacher")]
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
            .Include(cs => cs.Sessions)
            .ThenInclude(s => s.Attendances)
            .Include(cs => cs.Class.StudentsClasses)
            .Include(cs => cs.Module)
            .Include(cs => cs.Module.CoursesModulesSemesters)
            .ThenInclude(cms => cms.Course)
            .Include(cs => cs.Module.CoursesModulesSemesters)
            .ThenInclude(cms => cms.Semester)
            .Where(cs => cs.StartDate.Date <= DateTime.Today &&
                         cs.Class.StudentsClasses.Any(sc => sc.StudentId == userStudent.Id));

        var semesters = classSchedule.Select(cs => cs.Module.CoursesModulesSemesters)
            .SelectMany(cms => cms)
            // .Where(cms => cms.CourseCode == student.CourseCode)
            .Select(cms => cms.Semester)
            .Select(s => new SemesterWithModuleResponse()
            {
                Id = s.Id,
                Name = s.Name,
                Modules = classSchedule.Where(cs =>
                        cs.Sessions.Any(se => se.Attendances.Any(a => a.StudentId == studentId)) &&
                        cs.Module.CoursesModulesSemesters.Any(cms =>
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

        var response = new List<SemesterWithModuleResponse>();
        foreach (var semester in distinctSemesters)
        {
            if (semester.Modules.Any())
            {
                response.Add(semester);
            }
        }

        return Ok(CustomResponse.Ok("Sro get semesters of student and modules successfully", response));
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
            var error = ErrorDescription.Error["E0600"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

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
            case ExamTypeTe:
                var error = ErrorDescription.Error["E0600"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check user center is same center of class
        if (classContext.CenterId != user.CenterId)
        {
            var error = ErrorDescription.Error["E0600"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var isTeacherTeachThisClass = classContext.ClassSchedules.Any(cs =>
            cs.ClassId == classId && cs.ModuleId == moduleId && cs.TeacherId == user.Id);

        // check teacher is teach this schedule (class and module)
        if (!isTeacherTeachThisClass)
        {
            var error = ErrorDescription.Error["E0600"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check module is start learning and no more than 3 days after last session
        var canUpdateModule = classContext.ClassSchedules
            .Any(cs => cs.ModuleId == moduleId &&
                       cs.StartDate.Date <= DateTime.Today &&
                       DateTime.Today <= cs.EndDate.Date);

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
            // if grade item is exams -> error (teacher can't update these grade items)
            var gradeItem = gradeItemsOfModule.First(gi => gi.Id == r.GradeItemId);
            if (gradeItem.GradeCategoryModule.GradeCategoryId
                is PracticeExam or TheoryExam or PracticeExamResit or TheoryExamResit)
            {
                var error = ErrorDescription.Error["E0306"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

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

    [HttpGet]
    [Route("api/statistics/classes/{classId:int}/pass-rate")]
    [Authorize(Roles = "sro")]
    public IActionResult GetPassRateOfClass(int classId)
    {
        var clazz = _context.Classes
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Teacher)
            .ThenInclude(t => t.User)
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Module)
            .FirstOrDefault(c => c.Id == classId);

        if (clazz == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }

        // if (clazz.ClassStatusId == ClassStatusMerged)
        // {
        //     var error = ErrorDescription.Error["E0401"];
        //     return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        // }

        var moduleProgressScores = clazz.ClassStatusId == ClassStatusMerged
            ? GetAlsoGradesOfNotActiveStudentsInClass(clazz)
            : GetGradesOfStudentsInClass(clazz);

        var responses = new List<PassRateOfClassAndModuleResponse>();

        foreach (var m in moduleProgressScores)
        {
            var module = _context.Modules.Find(m.Module.Id);
            var passedCount = 0;
            foreach (var student in m.Students)
            {
                var totalScore = CalculateAverageScore(student, module!);
                Console.WriteLine("student: " + student.UserId + " total score: " + totalScore);
                if (totalScore != null)
                {
                    var roundedScore = Math.Round((double)totalScore, 2); // round to 2 decimal places

                    if (roundedScore >= GradeToPass)
                    {
                        passedCount++;
                    }
                }
            }

            var teacher = clazz.ClassSchedules.First(cs => cs.ModuleId == m.Module.Id).Teacher;

            var response = new PassRateOfClassAndModuleResponse()
            {
                Class = new BasicClassResponse()
                {
                    Id = clazz.Id,
                    Name = clazz.Name,
                },
                Module = new BasicModuleResponse()
                {
                    Id = module!.Id,
                    Name = module.ModuleName,
                },
                Teacher = new BasicTeacherInformationResponse()
                {
                    Id = teacher.User.Id,
                    EmailOrganization = teacher.User.EmailOrganization,
                    FirstName = teacher.User.FirstName,
                    LastName = teacher.User.LastName,
                },
                NumberOfStudents = m.Students.Count,
                NumberOfPassStudents = passedCount
            };

            responses.Add(response);
        }

        return Ok(CustomResponse.Ok("Get pass rate of class with all modules successfully", responses));
    }

    [HttpGet]
    [Route("api/statistics/classes/{classId:int}/modules/{moduleId:int}/pass-rate")]
    [Authorize(Roles = "sro")]
    public IActionResult GetPassRateOfClassAndModule(int classId, int moduleId)
    {
        var clazz = _context.Classes
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Teacher)
            .ThenInclude(t => t.User)
            .Include(c => c.CourseFamily)
            .Include(c => c.CourseFamily.Courses)
            .ThenInclude(c => c.CoursesModulesSemesters)
            .ThenInclude(cms => cms.Module)
            .FirstOrDefault(c => c.Id == classId);

        if (clazz == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }

        // if (clazz.ClassStatusId == ClassStatusMerged)
        // {
        //     var error = ErrorDescription.Error["E0401"];
        //     return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        // }

        var isModuleForThisClass = clazz.CourseFamily.Courses
            .Select(c => c.CoursesModulesSemesters)
            .Any(listCms => listCms
                .Any(cms => cms.ModuleId == moduleId));

        if (!isModuleForThisClass)
        {
            return NotFound(CustomResponse.NotFound("Module not for this class"));
        }

        var module = clazz.CourseFamily.Courses
            .Select(c => c.CoursesModulesSemesters)
            .SelectMany(listCms => listCms)
            .Select(cms => cms.Module)
            .FirstOrDefault(m => m.Id == moduleId);

        var moduleProgressScores = clazz.ClassStatusId == ClassStatusMerged
            ? GetAlsoGradesOfNotActiveStudentsInClass(clazz).Where(m => m.Module.Id == moduleId)
            : GetGradesOfStudentsInClass(clazz).Where(m => m.Module.Id == moduleId);

        var progressScore = moduleProgressScores.FirstOrDefault();

        if (progressScore == null)
        {
            return Ok(CustomResponse.Ok("Module may not have in this class schedule", null!));
        }

        var students = progressScore.Students;

        var passedCount = 0;
        foreach (var student in students)
        {
            var totalScore = CalculateAverageScore(student, module!);
            Console.WriteLine("student: " + student.UserId + " total score: " + totalScore);
            if (totalScore != null)
            {
                var roundedScore = Math.Round((double)totalScore, 2); // round to 2 decimal places

                if (roundedScore >= GradeToPass)
                {
                    passedCount++;
                }
            }
        }

        var teacher = clazz.ClassSchedules.First(cs => cs.ModuleId == moduleId).Teacher;

        var response = new PassRateOfClassAndModuleResponse()
        {
            Class = new BasicClassResponse()
            {
                Id = clazz.Id,
                Name = clazz.Name,
            },
            Module = new BasicModuleResponse()
            {
                Id = module!.Id,
                Name = module.ModuleName,
            },
            Teacher = new BasicTeacherInformationResponse()
            {
                Id = teacher.User.Id,
                EmailOrganization = teacher.User.EmailOrganization,
                FirstName = teacher.User.FirstName,
                LastName = teacher.User.LastName,
            },
            // NumberOfStudents = numberOfStudent,
            NumberOfStudents = students.Count,
            NumberOfPassStudents = passedCount
        };

        return Ok(CustomResponse.Ok("Get pass rate of class and module successfully", response));
    }

    [HttpGet]
    [Route("api/statistics/teachers/{teacherId:int}/modules")]
    [Authorize(Roles = "sro")]
    public IActionResult GetModulesTeachByTeacher(int teacherId)
    {
        var teacher = _context.Teachers.Find(teacherId);

        if (teacher == null)
        {
            return NotFound(CustomResponse.NotFound("Teacher not found"));
        }

        // module completed learning in schedule
        var modules = GetModulesTaughtByTeacher(teacherId).ToList();

        var response = modules.Select(m => new BasicModuleResponse()
        {
            Id = m.Id,
            Name = m.ModuleName
        });

        return Ok(CustomResponse.Ok("Get modules teach by teacher successfully", response));
    }

    [HttpGet]
    [Route("api/statistics/teachers/{teacherId:int}/modules/{moduleId:int}/classes")]
    [Authorize(Roles = "sro")]
    public IActionResult GetClassesOfTeacherByModuleId(int teacherId, int moduleId)
    {
        var teacher = _context.Teachers.Find(teacherId);

        if (teacher == null)
        {
            return NotFound(CustomResponse.NotFound("Teacher not found"));
        }

        // class completed learning in schedule
        var classes = GetClassesTaughtByTeacherByModuleId(teacherId, moduleId);

        return Ok(CustomResponse.Ok("Get classes of teacher by module successfully", classes));
    }

    [HttpGet]
    [Route("api/statistics/teachers/{teacherId:int}/pass-rate-all-module")]
    [Authorize(Roles = "sro")]
    public IActionResult GetPassRateAllModuleOfTeacher(int teacherId)
    {
        // modules that teach by this teacher
        var modules = GetModulesTaughtByTeacher(teacherId).ToList();

        var responses = new List<PassRateOfTeacherAndModuleResponse>();

        foreach (var module in modules)
        {
            var numberOfStudentInAllModule = 0;
            var passedCount = 0;
            var classes = GetClassesTaughtByTeacherByModuleId(teacherId, module.Id).ToList();
            foreach (var c in classes)
            {
                var clazz = new Class()
                {
                    Id = c.Id,
                    Name = c.Name,
                };

                var progressScores = GetAlsoGradesOfNotActiveStudentsInClass(clazz)
                    .FirstOrDefault(m => m.Module.Id == module.Id);

                if (progressScores == null)
                {
                    continue;
                }

                var students = progressScores.Students;
                numberOfStudentInAllModule += students.Count;

                foreach (var student in students)
                {
                    var totalScore = CalculateAverageScore(student, module);
                    Console.WriteLine("student: " + student.UserId + " total score: " + totalScore);
                    if (totalScore != null)
                    {
                        var roundedScore = Math.Round((double)totalScore, 2); // round to 2 decimal places

                        if (roundedScore >= GradeToPass)
                        {
                            passedCount++;
                        }
                    }
                }
            }

            var response = new PassRateOfTeacherAndModuleResponse()
            {
                Module = new BasicModuleResponse()
                {
                    Id = module.Id,
                    Name = module.ModuleName,
                },

                NumberOfStudentInAllClass = numberOfStudentInAllModule,
                NumberOfPassStudents = passedCount
            };

            responses.Add(response);
        }


        return Ok(CustomResponse.Ok("Get pass rate all module of teacher successfully", responses));
    }

    [HttpGet]
    [Route("api/statistics/teachers/pass-rate-all-time")]
    [Authorize(Roles = "sro")]
    public IActionResult GetPassRateOfAllTeacherInAllTime()
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        var user = _context.Users.First(u => u.Id == userId);

        // get all teacher in this center
        var teachers = _context.Teachers
            .Include(t => t.User)
            .Where(t => t.User.CenterId == user.CenterId).ToList();

        var responses = new List<PassRateOfTeacherResponse>();

        foreach (var teacher in teachers)
        {
            // modules that teach by this teacher
            var modules = _context.ClassSchedules
                .Include(cs => cs.Module)
                .Include(cs => cs.Class)
                .Where(cs => cs.TeacherId == teacher.UserId && cs.EndDate < DateTime.Today)
                .Select(cs => cs.Module).Distinct().ToList();

            var numberOfStudentInAllModule = 0;
            var passedCount = 0;
            foreach (var module in modules)
            {
                var classes = GetClassesTaughtByTeacherByModuleId(teacher.UserId, module.Id).ToList();
                foreach (var c in classes)
                {
                    var clazz = new Class()
                    {
                        Id = c.Id,
                        Name = c.Name,
                    };

                    var progressScores = GetAlsoGradesOfNotActiveStudentsInClass(clazz)
                        .FirstOrDefault(m => m.Module.Id == module.Id);

                    if (progressScores == null)
                    {
                        continue;
                    }

                    var students = progressScores.Students;
                    numberOfStudentInAllModule += students.Count;

                    foreach (var student in students)
                    {
                        var totalScore = CalculateAverageScore(student, module);
                        Console.WriteLine("student: " + student.UserId + " total score: " + totalScore);
                        if (totalScore != null)
                        {
                            var roundedScore = Math.Round((double)totalScore, 2); // round to 2 decimal places

                            if (roundedScore >= GradeToPass)
                            {
                                passedCount++;
                            }
                        }
                    }
                }
            }

            var response = new PassRateOfTeacherResponse()
            {
                Teacher = new BasicTeacherInformationResponse()
                {
                    Id = teacher.UserId,
                    EmailOrganization = teacher.User.EmailOrganization,
                    FirstName = teacher.User.FirstName,
                    LastName = teacher.User.LastName
                },
                NumberOfAllStudents = numberOfStudentInAllModule,
                NumberOfPassStudents = passedCount
            };
            responses.Add(response);
        }

        return Ok(CustomResponse.Ok("Get pass rate of all teacher in all time successfully", responses));
    }

    [HttpPost]
    [Route("api/statistics/teachers/pass-rate-period-time/get")]
    [Authorize(Roles = "sro")]
    public IActionResult GetPassRateOfAllTeacherInAPeriodOfTime([FromBody] PassRateOfTeacherInAPeriodRequest request)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        var user = _context.Users.First(u => u.Id == userId);

        if (request.FromDate > request.ToDate)
        {
            var error = ErrorDescription.Error["E0500"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // get all teacher in this center
        var teachers = _context.Teachers
            .Include(t => t.User)
            .Where(t => t.User.CenterId == user.CenterId).ToList();

        var fromDate = request.FromDate.Date;
        var toDate = request.ToDate.Date;

        var responses = new List<PassRateOfTeacherResponse>();

        foreach (var teacher in teachers)
        {
            var schedules = _context.ClassSchedules
                .Include(cs => cs.Module)
                .Include(cs => cs.Class)
                // select all schedules in range from date to to date base on start date and end date of schedule
                .Where(cs => cs.TeacherId == teacher.UserId && cs.EndDate < DateTime.Today &&
                             //check overlap two date range
                             ((cs.StartDate.Date <= fromDate && fromDate <= cs.EndDate.Date) ||
                              (cs.StartDate.Date <= toDate && toDate <= cs.EndDate.Date) ||
                              (cs.StartDate.Date >= fromDate && toDate >= cs.EndDate.Date))).ToList();
            var numberOfStudentInAllModule = 0;
            var passedCount = 0;
            foreach (var schedule in schedules)
            {
                var progressScores = GetAlsoGradesOfNotActiveStudentsInClass(schedule.Class)
                    .FirstOrDefault(m => m.Module.Id == schedule.ModuleId);

                if (progressScores == null)
                {
                    continue;
                }

                var students = progressScores.Students;

                numberOfStudentInAllModule += students.Count;

                foreach (var student in students)
                {
                    var totalScore = CalculateAverageScore(student, schedule.Module);
                    Console.WriteLine("student: " + student.UserId + " total score: " + totalScore);
                    if (totalScore != null)
                    {
                        var roundedScore = Math.Round((double)totalScore, 2); // round to 2 decimal places

                        if (roundedScore >= GradeToPass)
                        {
                            passedCount++;
                        }
                    }
                }
            }

            var response = new PassRateOfTeacherResponse()
            {
                Teacher = new BasicTeacherInformationResponse()
                {
                    Id = teacher.UserId,
                    EmailOrganization = teacher.User.EmailOrganization,
                    FirstName = teacher.User.FirstName,
                    LastName = teacher.User.LastName
                },
                NumberOfAllStudents = numberOfStudentInAllModule,
                NumberOfPassStudents = passedCount
            };
            responses.Add(response);
        }

        return Ok(CustomResponse.Ok("Get pass rate of all teacher in a period of time successfully", responses));
    }

    [HttpGet]
    [Route("api/statistics/teachers/{teacherId:int}/modules/{moduleId:int}/pass-rate-all-class")]
    [Authorize(Roles = "sro")]
    public IActionResult GetPassRateOfTeacherByModuleInAllClasses(int teacherId, int moduleId)
    {
        var teacher = _context.Teachers.Find(teacherId);

        if (teacher == null)
        {
            return NotFound(CustomResponse.NotFound("Teacher not found"));
        }

        var schedule = _context.ClassSchedules.Include(cs => cs.Module)
            .FirstOrDefault(cs => cs.TeacherId == teacherId && cs.ModuleId == moduleId);

        if (schedule == null)
        {
            return NotFound(CustomResponse.NotFound("Teacher not teach this module"));
        }

        var module = schedule.Module;

        var classes = GetClassesTaughtByTeacherByModuleId(teacherId, moduleId).ToList();

        var numberOfStudentInAllClass = 0;
        var passedCount = 0;

        foreach (var c in classes)
        {
            var clazz = new Class
            {
                Id = c.Id,
                Name = c.Name
            };

            var progressScores = GetAlsoGradesOfNotActiveStudentsInClass(clazz)
                .FirstOrDefault(m => m.Module.Id == moduleId);

            if (progressScores == null)
            {
                continue;
            }

            var students = progressScores.Students;
            numberOfStudentInAllClass += students.Count;

            foreach (var student in students)
            {
                var totalScore = CalculateAverageScore(student, module);
                Console.WriteLine("student: " + student.UserId + " total score: " + totalScore);
                if (totalScore != null)
                {
                    var roundedScore = Math.Round((double)totalScore, 2); // round to 2 decimal places

                    if (roundedScore >= GradeToPass)
                    {
                        passedCount++;
                    }
                }
            }
        }

        var response = new PassRateOfTeacherAndModuleResponse()
        {
            Module = new BasicModuleResponse()
            {
                Id = module.Id,
                Name = module.ModuleName,
            },
            NumberOfStudentInAllClass = numberOfStudentInAllClass,
            NumberOfPassStudents = passedCount
        };

        return Ok(CustomResponse.Ok("Get pass rate teacher by module in all classes successfully", response));
    }

    [HttpGet]
    [Route("api/statistics/teachers/{teacherId:int}/modules/{moduleId:int}/classes/{classId:int}/pass-rate")]
    [Authorize(Roles = "sro")]
    public IActionResult GetPassRateOfTeacherByModuleInSpecificClass(int teacherId, int moduleId, int classId)
    {
        var teacher = _context.Teachers.Include(t => t.User)
            .FirstOrDefault(t => t.UserId == teacherId);

        if (teacher == null)
        {
            return NotFound(CustomResponse.NotFound("Teacher not found"));
        }

        var schedule = _context.ClassSchedules
            .Include(cs => cs.Class)
            .Include(cs => cs.Module)
            .FirstOrDefault(cs => cs.TeacherId == teacherId && cs.ModuleId == moduleId && cs.EndDate < DateTime.Today);

        if (schedule == null)
        {
            return NotFound(CustomResponse.NotFound("Teacher not teach this module"));
        }

        var module = schedule.Module;

        var classResponse = GetClassesTaughtByTeacherByModuleId(teacherId, moduleId)
            .FirstOrDefault(c => c.Id == classId);

        if (classResponse == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found for this teacher and module"));
        }

        var passedCount = 0;


        var clazz = new Class
        {
            Id = classResponse.Id,
            Name = classResponse.Name
        };

        var progressScores = GetAlsoGradesOfNotActiveStudentsInClass(clazz)
            .FirstOrDefault(m => m.Module.Id == moduleId);

        if (progressScores == null)
        {
            return Ok(CustomResponse.Ok("Module may not have in this class schedule", null!));
        }

        var students = progressScores.Students;

        foreach (var student in students)
        {
            var totalScore = CalculateAverageScore(student, module);

            Console.WriteLine("student: " + student.UserId + " total score: " + totalScore);

            if (totalScore != null)
            {
                var roundedScore = Math.Round((double)totalScore, 2); // round to 2 decimal places

                if (roundedScore >= GradeToPass)
                {
                    passedCount++;
                }
            }
        }

        var response = new PassRateOfClassAndModuleResponse()
        {
            Class = new BasicClassResponse()
            {
                Id = clazz.Id,
                Name = clazz.Name,
            },
            Module = new BasicModuleResponse()
            {
                Id = module.Id,
                Name = module.ModuleName,
            },
            Teacher = new BasicTeacherInformationResponse()
            {
                Id = teacher.User.Id,
                EmailOrganization = teacher.User.EmailOrganization,
                FirstName = teacher.User.FirstName,
                LastName = teacher.User.LastName,
            },
            // NumberOfStudents = numberOfStudent,
            NumberOfStudents = students.Count,
            NumberOfPassStudents = passedCount
        };

        return Ok(CustomResponse.Ok("Get pass rate of teacher by module in specific class successfully", response));
    }

    private static double? CalculateAverageScore(StudentInfoAndGradeResponse studentGrades, Module module)
    {
        double? totalScoreTheory = 0.0;
        double? totalScorePractice = 0.0;
        double? totalScore = 0.0;
        var flagPractice = false;
        var flagTheory = false;
        var flagPracticeResit = false;
        var flagTheoryResit = false;
        double? tempPe = 0.0;
        double? tempPeResit = 0.0;
        double? tempTe = 0.0;
        double? tempTeResit = 0.0;


        // only have theory exam and theory exam resit
        if (module.ExamType is ExamTypeTe)
        {
            foreach (var category in studentGrades.GradeCategories)
            {
                foreach (var item in category.GradeItems)
                {
                    if (category.Id is TheoryExam)
                    {
                        if (item.Grade is null or 0)
                        {
                            flagTheory = true;
                            break;
                        }

                        totalScore -= tempTeResit;
                        tempTe += item.Grade / module.MaxTheoryGrade * 10; // change to base 10
                        totalScore += tempTe;
                    }

                    if (category.Id is TheoryExamResit)
                    {
                        if (item.Grade is null)
                        {
                            flagTheoryResit = true;
                            break;
                        }

                        if (item.Grade == 0)
                        {
                            return item.Grade;
                        }

                        totalScore -= tempTe;
                        tempTeResit += item.Grade / module.MaxTheoryGrade * 10; // change to base 10
                        totalScore += tempTeResit;
                    }
                }
            }

            if (flagTheory && flagTheoryResit)
            {
                return 0;
            }
        }

        // have only one final project
        if (module.ExamType == ExamTypeNoTakeExam)
        {
            foreach (var category in studentGrades.GradeCategories)
            {
                foreach (var item in category.GradeItems)
                {
                    if (item.Grade is null or 0)
                    {
                        return 0;
                    }

                    totalScore += item.Grade;
                }
            }
        }

        // module exam type is both theory and practice or only practice
        if (module.ExamType is ExamTypePe or ExamTypeBothPeAndTe)
        {
            foreach (var category in studentGrades.GradeCategories)
            {
                foreach (var item in category.GradeItems)
                {
                    if (category.Id is PracticeExam)
                    {
                        if (item.Grade is null or 0)
                        {
                            flagPractice = true;
                            break;
                        }

                        totalScorePractice -= tempPeResit;
                        tempPe = item.Grade * category.TotalWeight / 100 / category.QuantityGradeItem /
                            module.MaxPracticalGrade * 10; // change to base 10
                        totalScorePractice += tempPe;
                    }
                    else if (category.Id is PracticeExamResit)
                    {
                        if (item.Grade is null)
                        {
                            flagPracticeResit = true;
                            break;
                        }

                        if (item.Grade == 0)
                        {
                            return item.Grade;
                        }

                        totalScorePractice -= tempPe;
                        tempPeResit = item.Grade * category.TotalWeight / 100 / category.QuantityGradeItem /
                            module.MaxPracticalGrade * 10; // change to base 10
                        totalScorePractice += tempPeResit;
                    }
                    else if (category.Id is TheoryExam)
                    {
                        if (item.Grade is null or 0)
                        {
                            flagTheory = true;
                            break;
                        }

                        totalScoreTheory -= tempTeResit;
                        tempTe += item.Grade / module.MaxTheoryGrade * 10; // change to base 10
                        totalScoreTheory += tempTe;
                    }
                    else if (category.Id is TheoryExamResit)
                    {
                        if (item.Grade is null)
                        {
                            flagTheoryResit = true;
                            break;
                        }

                        if (item.Grade == 0)
                        {
                            return item.Grade;
                        }

                        totalScoreTheory -= tempTe;
                        tempTeResit += item.Grade / module.MaxTheoryGrade * 10; // change to base 10
                        totalScoreTheory += tempTeResit;
                    }
                    else
                    {
                        totalScorePractice += item.Grade * category.TotalWeight / 100 / category.QuantityGradeItem;
                    }
                }
            }

            if (module.ExamType is ExamTypePe)
            {
                totalScore += totalScorePractice;
            }
            else
            {
                totalScore += (totalScoreTheory + totalScorePractice) / 2;
            }

            // student has not taken the exam | don't have grade
            if ((flagPractice && flagPracticeResit) || (flagTheory && flagTheoryResit))
            {
                return 0;
            }
        }

        return totalScore;
    }

    private IQueryable<BasicClassResponse> GetClassesTaughtByTeacherByModuleId(int teacherId, int moduleId)
    {
        return _context.ClassSchedules
            .Include(cs => cs.Class)
            .Where(cs =>
                cs.TeacherId == teacherId && cs.ModuleId == moduleId && cs.EndDate < DateTime.Today)
            //&& cs.Class.ClassStatusId != ClassStatusMerged)
            .Select(cs => new BasicClassResponse()
            {
                Id = cs.Class.Id,
                Name = cs.Class.Name,
            });
    }

    private IQueryable<Module> GetModulesTaughtByTeacher(int teacherId)
    {
        return _context.ClassSchedules
            .Include(cs => cs.Module)
            .Include(cs => cs.Class)
            .Where(cs => cs.TeacherId == teacherId && cs.EndDate < DateTime.Today)
            //&& cs.Class.ClassStatusId != ClassStatusMerged)
            .Select(cs => cs.Module).Distinct();
    }

    private IQueryable<ListStudentGradeResponse> GetGradesOfStudentsInClass(Class clazz)
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
            .Where(m => m.ClassSchedules.Any(cs =>
                cs.ClassId == clazz.Id && cs.ModuleId == m.Id)) // && cs.EndDate <= DateTime.Today))
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

    private IQueryable<ListStudentGradeResponse> GetAlsoGradesOfNotActiveStudentsInClass(Class clazz)
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
            .Where(m => m.ClassSchedules.Any(cs =>
                cs.ClassId == clazz.Id && cs.ModuleId == m.Id)) // && cs.EndDate <= DateTime.Today))
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
                    .Where(cs => cs.ClassId == clazz.Id && !cs.Student.IsDraft) //&& cs.IsActive)
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
}