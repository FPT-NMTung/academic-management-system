using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.StatisticController;
using AcademicManagementSystem.Models.StudentGradeController.StudentGradeModel;
using AcademicManagementSystem.Models.StudentGradeController.StudentGradeModel.GradeItem;
using AcademicManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class StatisticController : ControllerBase
{
    private readonly AmsContext _context;
    private readonly IUserService _userService;
    private const int ClassStatusMerged = 6;
    private const int PracticeExam = 5;
    private const int TheoryExam = 6;
    private const int PracticeExamResit = 7;
    private const int TheoryExamResit = 8;
    private const int ExamTypeTe = 1;
    private const int ExamTypePe = 2;
    private const int ExamTypeBothPeAndTe = 3;
    private const int ExamTypeNoTakeExam = 4;


    public StatisticController(AmsContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpGet]
    [Route("api/statistics/classes/{classId:int}/modules/{moduleId:int}/pass-rate")]
    [Authorize(Roles = "sro")]
    public IActionResult GetPassRateOfClassAndModule(int classId, int moduleId)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());

        var clazz = _context.Classes
            .Include(c => c.StudentsClasses)
            .ThenInclude(c => c.Student)
            .Include(c => c.CourseFamily)
            .Include(c => c.CourseFamily.Courses)
            .ThenInclude(c => c.CoursesModulesSemesters)
            .ThenInclude(cms => cms.Module)
            .Include(c => c.Center)
            .Include(c => c.ClassSchedules)
            .ThenInclude(cs => cs.Module)
            .FirstOrDefault(c => c.Id == classId);

        if (clazz == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found"));
        }

        if (clazz.ClassStatusId == ClassStatusMerged)
        {
            var error = ErrorDescription.Error["E0401"];
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

        var module = clazz.CourseFamily.Courses
            .Select(c => c.CoursesModulesSemesters)
            .SelectMany(listCms => listCms)
            .Select(cms => cms.Module)
            .FirstOrDefault(m => m.Id == moduleId);

        var moduleProgressScores = GetGradesOfStudentsInClass(clazz, moduleId);

        var progressScore = moduleProgressScores.FirstOrDefault();

        if (progressScore == null)
        {
            return Ok(CustomResponse.Ok("Get pass rate of class and module successfully", null!));
        }

        var students = progressScore.Students;

        var passedCount = 0;
        foreach (var s in students)
        {
            var totalScore = CalculateAverageScore(s, module!);
            Console.WriteLine("student: " + s.UserId + " total score: " + totalScore);
            if (totalScore != null)
            {
                var roundedScore = Math.Round((double)totalScore, 2);
                // round to 2 double places
                if (roundedScore >= 5)
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
                Id = module!.Id,
                Name = module.ModuleName,
            },
            // NumberOfStudents = numberOfStudent,
            NumberOfStudents = students.Count,
            NumberOfPassStudents = passedCount
        };

        return Ok(CustomResponse.Ok("Get pass rate of class and module successfully", response));
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
        double? tempPe = 0;
        double? tempPeResit = 0;
        double? tempTe = 0;
        double? tempTeResit = 0;


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
                        totalScore += item.Grade * category.TotalWeight / 100 / category.QuantityGradeItem;
                    }
                }
            }

            totalScore += (totalScoreTheory + totalScorePractice);
            totalScore /= 2;
            // student has not taken the exam | don't have grade
            if ((flagPractice && flagPracticeResit) || (flagTheory && flagTheoryResit))
            {
                return 0;
            }
        }

        return totalScore;
    }

    // // get grades of all students in class
    // private IQueryable<StudentInfoAndGradeResponse> GetGradesOfStudentsInClass(int classId, int moduleId)
    // {
    //     var grades = _context.StudentGrades
    //         .Include(sg => sg.Class)
    //         .Include(sg => sg.Student)
    //         .Include(sg => sg.Student.User)
    //         .Include(sg => sg.GradeItem)
    //         .Include(sg => sg.GradeItem.GradeCategoryModule)
    //         .Include(sg => sg.GradeItem.GradeCategoryModule.GradeCategory)
    //         .Include(sg => sg.GradeItem.GradeCategoryModule.Module)
    //         .ThenInclude(m => m.GradeCategoryModule)
    //         .ThenInclude(gcm => gcm.GradeItems)
    //         .Where(sg => sg.ClassId == classId && sg.GradeItem.GradeCategoryModule.ModuleId == moduleId
    //                                            && sg.Student.StudentsClasses.Any(sc =>
    //                                                sc.IsActive && sc.ClassId == classId))
    //         .Select(sg => new StudentInfoAndGradeResponse
    //         {
    //             UserId = sg.StudentId,
    //             EnrollNumber = sg.Student.EnrollNumber,
    //             EmailOrganization = sg.Student.User.EmailOrganization,
    //             FirstName = sg.Student.User.FirstName,
    //             LastName = sg.Student.User.LastName,
    //             Avatar = sg.Student.User.Avatar,
    //             GradeCategories = sg.GradeItem.GradeCategoryModule.Module.GradeCategoryModule
    //                 .Select(gcm => new GradeCategoryWithItemsResponse()
    //                 {
    //                     Id = gcm.GradeCategory.Id,
    //                     Name = gcm.GradeCategory.Name,
    //                     TotalWeight = gcm.TotalWeight,
    //                     QuantityGradeItem = gcm.QuantityGradeItem,
    //                     GradeItems = gcm.GradeItems.Select(gi => new GradeItemWithStudentScoreResponse()
    //                     {
    //                         Id = gi.Id,
    //                         Name = gi.Name,
    //                         Grade = gi.StudentGrades
    //                             .Where(gis => gis.StudentId == sg.StudentId)
    //                             .Select(gis => gis.Grade)
    //                             .FirstOrDefault(),
    //                         Comment = gi.StudentGrades
    //                             .Where(gis => gis.StudentId == sg.StudentId)
    //                             .Select(gis => gis.Comment)
    //                             .FirstOrDefault()
    //                     }).ToList()
    //                 })
    //                 .ToList()
    //         });
    //
    //     return grades;
    // }
    // get grades of all students in class
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
}