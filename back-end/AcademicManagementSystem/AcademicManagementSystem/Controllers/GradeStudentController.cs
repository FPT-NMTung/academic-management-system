using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.GradeCategoryController;
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
        var student = _context.Students.Include(s => s.User).First(s => s.UserId == userId);

        var clazz = _context.Classes
            .Include(c => c.StudentsClasses)
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Module)
            .FirstOrDefault(c => c.Id == classId);
        if (clazz == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }

        var module = clazz.ClassSchedules.FirstOrDefault(cs => cs.ModuleId == moduleId);
        if (module == null)
        {
            return NotFound(CustomResponse.NotFound("Module not found in this class schedule"));
        }

        var isStudentInClass = clazz.StudentsClasses.Any(sc => sc.StudentId == userId);

        if (!isStudentInClass)
        {
            return Unauthorized(CustomResponse.Unauthorized("You are not authorized to access this resource"));
        }

        var moduleProgressScores = GetGradesOfSpecificStudent(clazz, moduleId, student);

        return Ok(CustomResponse.Ok("Student get progress scores successfully", moduleProgressScores));
    }
    
    // student get their grades by class and module
    [HttpGet]
    [Route("api/classes/{classId:int}/modules/{moduleId:int}/students/{studentId:int}/grades")]
    [Authorize(Roles = "sro")]
    public IActionResult SroGetGradesOfStudent(int classId, int moduleId, int studentId)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        var sro = _context.Sros.Include(s => s.User.Center).First(s => s.UserId == userId);

        var clazz = _context.Classes
            .Include(c => c.Center)
            .Include(c => c.StudentsClasses)
            .ThenInclude(sc => sc.Student)
            .ThenInclude(s => s.User)
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Module)
            .FirstOrDefault(c => c.Id == classId);
        if (clazz == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }
        
        if(sro.User.Center.Id != clazz.Center.Id)
        {
            return Unauthorized(CustomResponse.Unauthorized("You are not authorized to access this resource"));
        }

        var module = clazz.ClassSchedules.FirstOrDefault(cs => cs.ModuleId == moduleId);
        if (module == null)
        {
            return NotFound(CustomResponse.NotFound("Module not found in this class schedule"));
        }

        var student = clazz.StudentsClasses.Select(sc => sc.Student).FirstOrDefault(s => s.UserId == studentId);

        // null or isDraft = true
        if (student is not { IsDraft: false })
        { 
            return NotFound(CustomResponse.NotFound("Student not found in class"));
        }

        var moduleProgressScores = GetGradesOfSpecificStudent(clazz, moduleId, student);

        return Ok(CustomResponse.Ok("Student get progress scores successfully", moduleProgressScores));
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
            .Include(c => c.Center)
            .Include(c => c.StudentsClasses)
            .ThenInclude(sc => sc.Student)
            .ThenInclude(s => s.User)
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Module)
            .FirstOrDefault(c => c.Id == classId);
        
        if (clazz == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }
        
        if(sro.User.Center.Id != clazz.Center.Id)
        {
            return Unauthorized(CustomResponse.Unauthorized("You are not authorized to access this resource"));
        }

        var module = clazz.ClassSchedules.FirstOrDefault(cs => cs.ModuleId == moduleId);
        if (module == null)
        {
            return NotFound(CustomResponse.NotFound("Module not found in this class schedule"));
        }

        var moduleProgressScores = GetGradesOfStudentsInClass(clazz, moduleId);
        return Ok(CustomResponse.Ok("SRO get progress scores of students successfully", moduleProgressScores));
    }

    // teacher get all grade of students in class 
    [HttpGet]
    [Route("api/classes/{classId:int}/modules/{moduleId:int}/grades-students/teachers")]
    [Authorize(Roles = "teacher")]
    public IActionResult TeacherGetListStudentGrades(int classId, int moduleId)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());

        var clazz = _context.Classes
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Module)
            .FirstOrDefault(c => c.Id == classId);
        if (clazz == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }

        var module = clazz.ClassSchedules.FirstOrDefault(cs => cs.ModuleId == moduleId);
        if (module == null)
        {
            return NotFound(CustomResponse.NotFound("Module not found in this class schedule"));
        }

        var isTeacherTeachThisClass = clazz.ClassSchedules.Any(cs =>
            cs.ClassId == classId && cs.ModuleId == moduleId && cs.TeacherId == userId);
        // check teacher is teach this schedule (class and module)
        if (!isTeacherTeachThisClass)
        {
            return Unauthorized(CustomResponse.Unauthorized("You are not authorized to access this resource"));
        }

        var moduleProgressScores = GetGradesOfStudentsInClass(clazz, moduleId);
        return Ok(CustomResponse.Ok("Teacher get progress scores of students successfully", moduleProgressScores));
    }

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
                    .Where(cs => cs.ClassId == clazz.Id)
                    .Select(sc => new StudentInfoAndGradeResponse()
                    {
                        UserId = sc.Student.UserId,
                        EnrollNumber = sc.Student.EnrollNumber,
                        EmailOrganization = sc.Student.User.EmailOrganization,
                        FirstName = sc.Student.User.FirstName,
                        LastName = sc.Student.User.LastName,
                        Avatar = sc.Student.User.Avatar,
                        ProgressScores = m.GradeCategoryModule
                            .Select(gcm => new GradeCategoryWithItemsResponse()
                            {
                                GradeCategory = new GradeCategoryResponse()
                                {
                                    Id = gcm.GradeCategory.Id,
                                    Name = gcm.GradeCategory.Name,
                                },
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
                    ProgressScores = m.GradeCategoryModule
                        .Select(gcm => new GradeCategoryWithItemsResponse()
                        {
                            GradeCategory = new GradeCategoryResponse()
                            {
                                Id = gcm.GradeCategory.Id,
                                Name = gcm.GradeCategory.Name,
                            },
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