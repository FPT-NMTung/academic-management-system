using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.CenterController;
using AcademicManagementSystem.Models.CourseController;
using AcademicManagementSystem.Models.CourseFamilyController;
using AcademicManagementSystem.Models.CourseModuleSemester;
using AcademicManagementSystem.Models.ModuleController;
using AcademicManagementSystem.Models.SemesterController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class CourseModuleSemesterController : ControllerBase
{
    private readonly AmsContext _context;

    public CourseModuleSemesterController(AmsContext context)
    {
        _context = context;
    }

    // get all course module semesters
    [HttpGet]
    [Route("api/courses-modules-semesters")]
    [Authorize(Roles = "admin,sro,teacher,student")]
    public IActionResult GetCourseModuleSemesters()
    {
        var courseModuleSemesters = _context.CoursesModulesSemesters
            .Include(cms => cms.Course)
            .Include(cms => cms.Module)
            .Include(cms => cms.Semester)
            .Include(cms => cms.Course.CourseFamily)
            .Include(cms => cms.Module.Center)
            .Include(cms => cms.Module.Center.Province)
            .Include(cms => cms.Module.Center.District)
            .Include(cms => cms.Module.Center.Ward)
            .Select(cms => new CourseModuleSemesterResponse()
            {
                CourseCode = cms.CourseCode,
                ModuleId = cms.ModuleId,
                SemesterId = cms.SemesterId,
                Course = new CourseResponse()
                {
                    Code = cms.Course.Code, Name = cms.Course.Name, CourseFamilyCode = cms.Course.CourseFamilyCode,
                    SemesterCount = cms.Course.SemesterCount, IsActive = cms.Course.IsActive,
                    CreatedAt = cms.Course.CreatedAt,
                    UpdatedAt = cms.Course.UpdatedAt, CourseFamily = new CourseFamilyResponse()
                    {
                        Code = cms.Course.CourseFamily.Code, Name = cms.Course.CourseFamily.Name,
                        PublishedYear = cms.Course.CourseFamily.PublishedYear,
                        IsActive = cms.Course.CourseFamily.IsActive,
                        CreatedAt = cms.Course.CourseFamily.CreatedAt, UpdatedAt = cms.Course.CourseFamily.UpdatedAt
                    }
                },
                Module = new ModuleResponse()
                {
                    Id = cms.Module.Id, ModuleName = cms.Module.ModuleName, ModuleType = cms.Module.ModuleType,
                    CenterId = cms.Module.CenterId, Days = cms.Module.Days, Hours = cms.Module.Hours,
                    MaxPracticalGrade = cms.Module.MaxPracticalGrade, MaxTheoryGrade = cms.Module.MaxTheoryGrade,
                    ExamType = cms.Module.ExamType, SemesterNamePortal = cms.Module.SemesterNamePortal,
                    ModuleExamNamePortal = cms.Module.ModuleExamNamePortal,
                    CreatedAt = cms.Module.CreatedAt, UpdatedAt = cms.Module.UpdatedAt,
                    Center = new CenterResponse()
                    {
                        Id = cms.Module.Center.Id, Name = cms.Module.Center.Name,
                        CreatedAt = cms.Module.Center.CreatedAt, UpdatedAt = cms.Module.Center.UpdatedAt,
                        Province = new ProvinceResponse()
                        {
                            Id = cms.Module.Center.Province.Id,
                            Code = cms.Module.Center.Province.Code,
                            Name = cms.Module.Center.Province.Name,
                        },
                        District = new DistrictResponse()
                        {
                            Id = cms.Module.Center.District.Id,
                            Name = cms.Module.Center.District.Name,
                            Prefix = cms.Module.Center.District.Prefix
                        },
                        Ward = new WardResponse()
                        {
                            Id = cms.Module.Center.Ward.Id,
                            Name = cms.Module.Center.Ward.Name,
                            Prefix = cms.Module.Center.Ward.Prefix
                        }
                    }
                },
                Semester = new SemesterResponse()
                {
                    Id = cms.Semester.Id, Name = cms.Semester.Name
                }
            }).ToList();
        return Ok(CustomResponse.Ok("Get all course module semesters successfully", courseModuleSemesters));
    }

    // get course module semester by course code
    [HttpGet]
    [Route("api/courses-modules-semesters/courses/{courseCode}")]
    [Authorize(Roles = "admin,sro,teacher,student")]
    public IActionResult GetCourseModuleSemesterByCourseCode(string courseCode)
    {
        var courseModuleSemester = _context.CoursesModulesSemesters
            .Include(cms => cms.Course)
            .Include(cms => cms.Module)
            .Include(cms => cms.Semester)
            .Include(cms => cms.Course.CourseFamily)
            .Include(cms => cms.Module.Center)
            .Include(cms => cms.Module.Center.Province)
            .Include(cms => cms.Module.Center.District)
            .Include(cms => cms.Module.Center.Ward)
            .Where(cms => cms.CourseCode == courseCode)
            .Select(cms => new CourseModuleSemesterResponse()
            {
                CourseCode = cms.CourseCode,
                ModuleId = cms.ModuleId,
                SemesterId = cms.SemesterId,
                Course = new CourseResponse()
                {
                    Code = cms.Course.Code, Name = cms.Course.Name, CourseFamilyCode = cms.Course.CourseFamilyCode,
                    SemesterCount = cms.Course.SemesterCount, IsActive = cms.Course.IsActive,
                    CreatedAt = cms.Course.CreatedAt,
                    UpdatedAt = cms.Course.UpdatedAt, CourseFamily = new CourseFamilyResponse()
                    {
                        Code = cms.Course.CourseFamily.Code, Name = cms.Course.CourseFamily.Name,
                        PublishedYear = cms.Course.CourseFamily.PublishedYear,
                        IsActive = cms.Course.CourseFamily.IsActive,
                        CreatedAt = cms.Course.CourseFamily.CreatedAt, UpdatedAt = cms.Course.CourseFamily.UpdatedAt
                    }
                },
                Module = new ModuleResponse()
                {
                    Id = cms.Module.Id, ModuleName = cms.Module.ModuleName, ModuleType = cms.Module.ModuleType,
                    CenterId = cms.Module.CenterId, Days = cms.Module.Days, Hours = cms.Module.Hours,
                    MaxPracticalGrade = cms.Module.MaxPracticalGrade, MaxTheoryGrade = cms.Module.MaxTheoryGrade,
                    ExamType = cms.Module.ExamType, SemesterNamePortal = cms.Module.SemesterNamePortal,
                    ModuleExamNamePortal = cms.Module.ModuleExamNamePortal,
                    CreatedAt = cms.Module.CreatedAt, UpdatedAt = cms.Module.UpdatedAt,
                    Center = new CenterResponse()
                    {
                        Id = cms.Module.Center.Id, Name = cms.Module.Center.Name,
                        CreatedAt = cms.Module.Center.CreatedAt, UpdatedAt = cms.Module.Center.UpdatedAt,
                        Province = new ProvinceResponse()
                        {
                            Id = cms.Module.Center.Province.Id,
                            Code = cms.Module.Center.Province.Code,
                            Name = cms.Module.Center.Province.Name,
                        },
                        District = new DistrictResponse()
                        {
                            Id = cms.Module.Center.District.Id,
                            Name = cms.Module.Center.District.Name,
                            Prefix = cms.Module.Center.District.Prefix
                        },
                        Ward = new WardResponse()
                        {
                            Id = cms.Module.Center.Ward.Id,
                            Name = cms.Module.Center.Ward.Name,
                            Prefix = cms.Module.Center.Ward.Prefix
                        }
                    }
                },
                Semester = new SemesterResponse()
                {
                    Id = cms.Semester.Id, Name = cms.Semester.Name
                }
            }).ToList();
        if (!courseModuleSemester.Any())
        {
            return NotFound(CustomResponse.NotFound("Course_module_semester by course code not found"));
        }

        return Ok(CustomResponse.Ok("Get all course module semesters successfully", courseModuleSemester));
    }

    // get course module semester by semester id
    [HttpGet]
    [Route("api/courses-modules-semesters/semesters/{semesterId:int}")]
    [Authorize(Roles = "admin,sro,teacher,student")]
    public IActionResult GetCourseModuleSemesterBySemesterId(int semesterId)
    {
        var courseModuleSemester = _context.CoursesModulesSemesters
            .Include(cms => cms.Course)
            .Include(cms => cms.Module)
            .Include(cms => cms.Semester)
            .Include(cms => cms.Course.CourseFamily)
            .Include(cms => cms.Module.Center)
            .Include(cms => cms.Module.Center.Province)
            .Include(cms => cms.Module.Center.District)
            .Include(cms => cms.Module.Center.Ward)
            .Where(cms => cms.SemesterId == semesterId)
            .Select(cms => new CourseModuleSemesterResponse()
            {
                CourseCode = cms.CourseCode,
                ModuleId = cms.ModuleId,
                SemesterId = cms.SemesterId,
                Course = new CourseResponse()
                {
                    Code = cms.Course.Code, Name = cms.Course.Name, CourseFamilyCode = cms.Course.CourseFamilyCode,
                    SemesterCount = cms.Course.SemesterCount, IsActive = cms.Course.IsActive,
                    CreatedAt = cms.Course.CreatedAt,
                    UpdatedAt = cms.Course.UpdatedAt, CourseFamily = new CourseFamilyResponse()
                    {
                        Code = cms.Course.CourseFamily.Code, Name = cms.Course.CourseFamily.Name,
                        PublishedYear = cms.Course.CourseFamily.PublishedYear,
                        IsActive = cms.Course.CourseFamily.IsActive,
                        CreatedAt = cms.Course.CourseFamily.CreatedAt, UpdatedAt = cms.Course.CourseFamily.UpdatedAt
                    }
                },
                Module = new ModuleResponse()
                {
                    Id = cms.Module.Id, ModuleName = cms.Module.ModuleName, ModuleType = cms.Module.ModuleType,
                    CenterId = cms.Module.CenterId, Days = cms.Module.Days, Hours = cms.Module.Hours,
                    MaxPracticalGrade = cms.Module.MaxPracticalGrade, MaxTheoryGrade = cms.Module.MaxTheoryGrade,
                    ExamType = cms.Module.ExamType, SemesterNamePortal = cms.Module.SemesterNamePortal,
                    ModuleExamNamePortal = cms.Module.ModuleExamNamePortal,
                    CreatedAt = cms.Module.CreatedAt, UpdatedAt = cms.Module.UpdatedAt,
                    Center = new CenterResponse()
                    {
                        Id = cms.Module.Center.Id, Name = cms.Module.Center.Name,
                        CreatedAt = cms.Module.Center.CreatedAt, UpdatedAt = cms.Module.Center.UpdatedAt,
                        Province = new ProvinceResponse()
                        {
                            Id = cms.Module.Center.Province.Id,
                            Code = cms.Module.Center.Province.Code,
                            Name = cms.Module.Center.Province.Name,
                        },
                        District = new DistrictResponse()
                        {
                            Id = cms.Module.Center.District.Id,
                            Name = cms.Module.Center.District.Name,
                            Prefix = cms.Module.Center.District.Prefix
                        },
                        Ward = new WardResponse()
                        {
                            Id = cms.Module.Center.Ward.Id,
                            Name = cms.Module.Center.Ward.Name,
                            Prefix = cms.Module.Center.Ward.Prefix
                        }
                    }
                },
                Semester = new SemesterResponse()
                {
                    Id = cms.Semester.Id, Name = cms.Semester.Name
                }
            }).ToList();
        if (!courseModuleSemester.Any())
        {
            return NotFound(CustomResponse.NotFound("Course_module_semester by semester id not found"));
        }

        return Ok(CustomResponse.Ok("Get course module semesters by semesterId successfully", courseModuleSemester));
    }
    
    // get course module semester by semester id
    [HttpGet]
    [Route("api/courses-modules-semesters/modules/{moduleId}")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult GetCourseModuleSemesterByModuleId(int moduleId)
    {
        var courseModuleSemester = _context.CoursesModulesSemesters
            .Include(cms => cms.Course)
            .Include(cms => cms.Module)
            .Include(cms => cms.Semester)
            .Include(cms => cms.Course.CourseFamily)
            .Include(cms => cms.Module.Center)
            .Include(cms => cms.Module.Center.Province)
            .Include(cms => cms.Module.Center.District)
            .Include(cms => cms.Module.Center.Ward)
            .Where(cms => cms.ModuleId == moduleId)
            .Select(cms => new CourseModuleSemesterResponse()
            {
                CourseCode = cms.CourseCode,
                ModuleId = cms.ModuleId,
                SemesterId = cms.SemesterId,
                Course = new CourseResponse()
                {
                    Code = cms.Course.Code, Name = cms.Course.Name, CourseFamilyCode = cms.Course.CourseFamilyCode,
                    SemesterCount = cms.Course.SemesterCount, IsActive = cms.Course.IsActive,
                    CreatedAt = cms.Course.CreatedAt,
                    UpdatedAt = cms.Course.UpdatedAt, CourseFamily = new CourseFamilyResponse()
                    {
                        Code = cms.Course.CourseFamily.Code, Name = cms.Course.CourseFamily.Name,
                        PublishedYear = cms.Course.CourseFamily.PublishedYear,
                        IsActive = cms.Course.CourseFamily.IsActive,
                        CreatedAt = cms.Course.CourseFamily.CreatedAt, UpdatedAt = cms.Course.CourseFamily.UpdatedAt
                    }
                },
                Module = new ModuleResponse()
                {
                    Id = cms.Module.Id, ModuleName = cms.Module.ModuleName, ModuleType = cms.Module.ModuleType,
                    CenterId = cms.Module.CenterId, Days = cms.Module.Days, Hours = cms.Module.Hours,
                    MaxPracticalGrade = cms.Module.MaxPracticalGrade, MaxTheoryGrade = cms.Module.MaxTheoryGrade,
                    ExamType = cms.Module.ExamType, SemesterNamePortal = cms.Module.SemesterNamePortal,
                    ModuleExamNamePortal = cms.Module.ModuleExamNamePortal,
                    CreatedAt = cms.Module.CreatedAt, UpdatedAt = cms.Module.UpdatedAt,
                    Center = new CenterResponse()
                    {
                        Id = cms.Module.Center.Id, Name = cms.Module.Center.Name,
                        CreatedAt = cms.Module.Center.CreatedAt, UpdatedAt = cms.Module.Center.UpdatedAt,
                        Province = new ProvinceResponse()
                        {
                            Id = cms.Module.Center.Province.Id,
                            Code = cms.Module.Center.Province.Code,
                            Name = cms.Module.Center.Province.Name,
                        },
                        District = new DistrictResponse()
                        {
                            Id = cms.Module.Center.District.Id,
                            Name = cms.Module.Center.District.Name,
                            Prefix = cms.Module.Center.District.Prefix
                        },
                        Ward = new WardResponse()
                        {
                            Id = cms.Module.Center.Ward.Id,
                            Name = cms.Module.Center.Ward.Name,
                            Prefix = cms.Module.Center.Ward.Prefix
                        }
                    }
                },
                Semester = new SemesterResponse()
                {
                    Id = cms.Semester.Id, Name = cms.Semester.Name
                }
            });
        if (!courseModuleSemester.Any())
        {
            return NotFound(CustomResponse.NotFound("Course_module_semester by module id not found"));
        }

        return Ok(CustomResponse.Ok("Get course module semesters by moduleID successfully", courseModuleSemester.First()));
    }
}