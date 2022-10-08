using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.CenterController;
using AcademicManagementSystem.Models.CourseController;
using AcademicManagementSystem.Models.CourseFamilyController;
using AcademicManagementSystem.Models.CourseModuleSemester;
using AcademicManagementSystem.Models.ModuleController;
using AcademicManagementSystem.Models.SemesterController;
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
    [Route("api/courses_modules_semesters")]
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
            .ToList()
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
        return Ok(CustomResponse.Ok("Get all course module semesters successfully", courseModuleSemesters));
    }
    
    // get course module semester by course code
    [HttpGet]
    [Route("api/courses_modules_semesters/courses/{courseCode}")]
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
            .FirstOrDefault(cms => cms.CourseCode == courseCode);
        if (courseModuleSemester == null)
        {
            return NotFound(CustomResponse.NotFound("Course_module_semester by course code not found"));
        }

        var courseModuleSemesterResponse = CourseModuleSemesterResponse(courseModuleSemester);
        return Ok(CustomResponse.Ok("Get all course module semesters successfully", courseModuleSemesterResponse));
    }
    
    // get course module semester by semester id
    [HttpGet]
    [Route("api/courses_modules_semesters/semesters/{semesterId}")]
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
            .FirstOrDefault(cms => cms.SemesterId == semesterId);
        if (courseModuleSemester == null)
        {
            return NotFound(CustomResponse.NotFound("Course_module_semester by semester id not found"));
        }

        var courseModuleSemesterResponse = CourseModuleSemesterResponse(courseModuleSemester);
        return Ok(CustomResponse.Ok("Get all course module semesters successfully", courseModuleSemesterResponse));
    }
    
    // get course module semester by module id
    [HttpGet]
    [Route("api/courses_modules_semesters/modules/{moduleId}")]
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
            .FirstOrDefault(cms => cms.ModuleId == moduleId);
        if (courseModuleSemester == null)
        {
            return NotFound(CustomResponse.NotFound("Course_module_semester by module id not found"));
        }

        var courseModuleSemesterResponse = CourseModuleSemesterResponse(courseModuleSemester);
        return Ok(CustomResponse.Ok("Get all course module semesters successfully", courseModuleSemesterResponse));
    }

    private static CourseModuleSemesterResponse CourseModuleSemesterResponse(CourseModuleSemester courseModuleSemester)
    {
        var courseModuleSemesterResponse = new CourseModuleSemesterResponse()
        {
            CourseCode = courseModuleSemester.CourseCode,
            ModuleId = courseModuleSemester.ModuleId,
            SemesterId = courseModuleSemester.SemesterId,
            Course = new CourseResponse()
            {
                Code = courseModuleSemester.Course.Code, Name = courseModuleSemester.Course.Name,
                CourseFamilyCode = courseModuleSemester.Course.CourseFamilyCode,
                SemesterCount = courseModuleSemester.Course.SemesterCount,
                IsActive = courseModuleSemester.Course.IsActive,
                CreatedAt = courseModuleSemester.Course.CreatedAt,
                UpdatedAt = courseModuleSemester.Course.UpdatedAt, CourseFamily = new CourseFamilyResponse()
                {
                    Code = courseModuleSemester.Course.CourseFamily.Code,
                    Name = courseModuleSemester.Course.CourseFamily.Name,
                    PublishedYear = courseModuleSemester.Course.CourseFamily.PublishedYear,
                    IsActive = courseModuleSemester.Course.CourseFamily.IsActive,
                    CreatedAt = courseModuleSemester.Course.CourseFamily.CreatedAt,
                    UpdatedAt = courseModuleSemester.Course.CourseFamily.UpdatedAt
                }
            },
            Module = new ModuleResponse()
            {
                Id = courseModuleSemester.Module.Id, ModuleName = courseModuleSemester.Module.ModuleName,
                ModuleType = courseModuleSemester.Module.ModuleType,
                CenterId = courseModuleSemester.Module.CenterId, Days = courseModuleSemester.Module.Days,
                Hours = courseModuleSemester.Module.Hours,
                MaxPracticalGrade = courseModuleSemester.Module.MaxPracticalGrade,
                MaxTheoryGrade = courseModuleSemester.Module.MaxTheoryGrade,
                ExamType = courseModuleSemester.Module.ExamType,
                SemesterNamePortal = courseModuleSemester.Module.SemesterNamePortal,
                ModuleExamNamePortal = courseModuleSemester.Module.ModuleExamNamePortal,
                CreatedAt = courseModuleSemester.Module.CreatedAt, UpdatedAt = courseModuleSemester.Module.UpdatedAt,
                Center = new CenterResponse()
                {
                    Id = courseModuleSemester.Module.Center.Id, Name = courseModuleSemester.Module.Center.Name,
                    CreatedAt = courseModuleSemester.Module.Center.CreatedAt,
                    UpdatedAt = courseModuleSemester.Module.Center.UpdatedAt,
                    Province = new ProvinceResponse()
                    {
                        Id = courseModuleSemester.Module.Center.Province.Id,
                        Code = courseModuleSemester.Module.Center.Province.Code,
                        Name = courseModuleSemester.Module.Center.Province.Name,
                    },
                    District = new DistrictResponse()
                    {
                        Id = courseModuleSemester.Module.Center.District.Id,
                        Name = courseModuleSemester.Module.Center.District.Name,
                        Prefix = courseModuleSemester.Module.Center.District.Prefix
                    },
                    Ward = new WardResponse()
                    {
                        Id = courseModuleSemester.Module.Center.Ward.Id,
                        Name = courseModuleSemester.Module.Center.Ward.Name,
                        Prefix = courseModuleSemester.Module.Center.Ward.Prefix
                    }
                }
            },
            Semester = new SemesterResponse()
            {
                Id = courseModuleSemester.Semester.Id, Name = courseModuleSemester.Semester.Name
            }
        };
        return courseModuleSemesterResponse;
    }
}