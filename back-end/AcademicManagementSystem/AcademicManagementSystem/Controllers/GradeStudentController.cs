using AcademicManagementSystem.Context;
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
        var user = _context.Users.First(u => u.Id == userId);
        var student = _context.Students.First(s => s.UserId == userId);

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
        
        if(!isStudentInClass)
        {
            return Unauthorized(CustomResponse.Unauthorized("You are not authorized to access this resource"));
        }

        var moduleProgressScores = _context.Modules
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
                    UserId = user.Id,
                    EnrollNumber = student.EnrollNumber,
                    EmailOrganization = user.EmailOrganization,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Avatar = user.Avatar,
                    Grades = m.GradeCategoryModule
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
                                        sg.StudentId == user.Id && sg.ClassId == classId)!.Grade,
                                    Comment = gi.StudentGrades.FirstOrDefault(sg =>
                                        sg.StudentId == user.Id && sg.ClassId == classId)!.Comment
                                })
                                .ToList()
                        }).ToList()
                },
            });
        
        return Ok(CustomResponse.Ok("Student get progress scores successfully", moduleProgressScores));
    }
 
}