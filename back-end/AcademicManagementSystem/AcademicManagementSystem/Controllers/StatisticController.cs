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
    private const int GradeToPass = 5;


    public StatisticController(AmsContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
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

        if (clazz.ClassStatusId == ClassStatusMerged)
        {
            var error = ErrorDescription.Error["E0401"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var moduleProgressScores = GetGradesOfStudentsInClass(clazz);

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
        var userId = Convert.ToInt32(_userService.GetUserId());

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

        var moduleProgressScores = GetGradesOfStudentsInClass(clazz)
            .Where(m => m.Module.Id == moduleId);

        var progressScore = moduleProgressScores.FirstOrDefault();

        if (progressScore == null)
        {
            return Ok(CustomResponse.Ok("Get pass rate of class and module successfully", null!));
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

        var modules = _context.ClassSchedules
            .Include(cs => cs.Module)
            .Where(cs => cs.TeacherId == teacherId)
            .Select(cs => new BasicModuleResponse()
            {
                Id = cs.Module.Id,
                Name = cs.Module.ModuleName,
            });

        return Ok(CustomResponse.Ok("Get modules teach by teacher successfully", modules));
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

        var classes = GetClassesTeachByTeacherByModuleId(teacherId, moduleId);

        return Ok(CustomResponse.Ok("Get classes of teacher by module successfully", classes));
    }

    [HttpGet]
    [Route("api/statistics/teachers/{teacherId:int}/modules/{moduleId:int}/pass-rate")]
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

        var classes = GetClassesTeachByTeacherByModuleId(teacherId, moduleId).ToList();

        var numberOfStudentInAllClass = 0;
        var passedCount = 0;

        foreach (var c in classes)
        {
            var clazz = new Class
            {
                Id = c.Id,
                Name = c.Name
            };

            var progressScores = GetGradesOfStudentsInClass(clazz)
                .FirstOrDefault(m => m.Module.Id == moduleId);

            if (progressScores == null)
            {
                continue;
            }

            var students = progressScores.Students;
            numberOfStudentInAllClass += students.Count;

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
            .Include(cs => cs.Module)
            .FirstOrDefault(cs => cs.TeacherId == teacherId && cs.ModuleId == moduleId);

        if (schedule == null)
        {
            return NotFound(CustomResponse.NotFound("Teacher not teach this module"));
        }

        var module = schedule.Module;

        var classResponse = GetClassesTeachByTeacherByModuleId(teacherId, moduleId)
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

        var progressScores = GetGradesOfStudentsInClass(clazz)
            .FirstOrDefault(m => m.Module.Id == moduleId);

        if (progressScores == null)
        {
            return Ok(CustomResponse.Ok("Get pass rate of teacher by class and module successfully", null!));
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
                        totalScorePractice += item.Grade * category.TotalWeight / 100 / category.QuantityGradeItem;
                    }
                }
            }

            totalScore += (totalScoreTheory + totalScorePractice) / 2;
            // student has not taken the exam | don't have grade
            if ((flagPractice && flagPracticeResit) || (flagTheory && flagTheoryResit))
            {
                return 0;
            }
        }

        return totalScore;
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

    private IQueryable<BasicClassResponse> GetClassesTeachByTeacherByModuleId(int teacherId, int moduleId)
    {
        return _context.ClassSchedules
            .Include(cs => cs.Class)
            .Where(cs => cs.TeacherId == teacherId && cs.ModuleId == moduleId)
            .Select(cs => new BasicClassResponse()
            {
                Id = cs.Class.Id,
                Name = cs.Class.Name,
            });
    }

    // get grades of all students in class
    // private IQueryable<ListStudentGradeResponse> GetGradesOfStudentsInClass(Class clazz, int moduleId)
    // {
    //     return _context.Modules
    //         .Include(m => m.ClassSchedules)
    //         .ThenInclude(cs => cs.Class)
    //         .ThenInclude(c => c.StudentsClasses)
    //         .Include(m => m.GradeCategoryModule)
    //         .ThenInclude(gcm => gcm.GradeCategory)
    //         .Include(m => m.GradeCategoryModule)
    //         .ThenInclude(gcm => gcm.GradeItems)
    //         .ThenInclude(gi => gi.StudentGrades)
    //         .Where(m => m.ClassSchedules.Any(cs => cs.ClassId == clazz.Id && cs.ModuleId == m.Id))
    //         .Select(m => new ListStudentGradeResponse()
    //         {
    //             Class = new BasicClassResponse()
    //             {
    //                 Id = clazz.Id,
    //                 Name = clazz.Name
    //             },
    //
    //             Module = new BasicModuleResponse()
    //             {
    //                 Id = m.Id,
    //                 Name = m.ModuleName
    //             },
    //
    //             Students = m.ClassSchedules.SelectMany(cs => cs.Class.StudentsClasses)
    //                 .Where(cs => cs.ClassId == clazz.Id && cs.IsActive && !cs.Student.IsDraft)
    //                 .Select(sc => new StudentInfoAndGradeResponse()
    //                 {
    //                     UserId = sc.Student.UserId,
    //                     EnrollNumber = sc.Student.EnrollNumber,
    //                     EmailOrganization = sc.Student.User.EmailOrganization,
    //                     FirstName = sc.Student.User.FirstName,
    //                     LastName = sc.Student.User.LastName,
    //                     Avatar = sc.Student.User.Avatar,
    //                     GradeCategories = m.GradeCategoryModule
    //                         .Select(gcm => new GradeCategoryWithItemsResponse()
    //                         {
    //                             Id = gcm.GradeCategory.Id,
    //                             Name = gcm.GradeCategory.Name,
    //                             TotalWeight = gcm.TotalWeight,
    //                             QuantityGradeItem = gcm.QuantityGradeItem,
    //                             GradeItems = gcm.GradeItems
    //                                 .Select(gi => new GradeItemWithStudentScoreResponse()
    //                                 {
    //                                     Id = gi.Id,
    //                                     Name = gi.Name,
    //                                     Grade = gi.StudentGrades.FirstOrDefault(sg =>
    //                                         sg.StudentId == sc.Student.UserId && sg.ClassId == clazz.Id)!.Grade,
    //                                     Comment = gi.StudentGrades.FirstOrDefault(sg =>
    //                                         sg.StudentId == sc.Student.UserId && sg.ClassId == clazz.Id)!.Comment
    //                                 })
    //                                 .ToList()
    //                         }).ToList()
    //                 }).ToList()
    //         });
    // }
}