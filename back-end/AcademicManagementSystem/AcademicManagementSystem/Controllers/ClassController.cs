using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.CenterController;
using AcademicManagementSystem.Models.ClassController;
using AcademicManagementSystem.Models.ClassDaysController;
using AcademicManagementSystem.Models.ClassStatusController;
using AcademicManagementSystem.Models.CourseController;
using AcademicManagementSystem.Models.CourseFamilyController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class ClassController : ControllerBase
{
    private readonly AmsContext _context;

    public ClassController(AmsContext context)
    {
        _context = context;
    }

    // get all classes
    [HttpGet]
    [Route("api/classes")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult GetClasses()
    {
        var classes = _context.Classes.Include(c => c.Center)
            .Include(c => c.Course)
            .Include(c => c.ClassDays)
            .Include(c => c.ClassStatus)
            .Select(c => new ClassResponse()
            {
                Id = c.Id, Name = c.Name, CenterId = c.CenterId, CourseCode = c.CourseCode, ClassDaysId = c.ClassDaysId,
                ClassStatusId = c.ClassStatusId, StartDate = c.StartDate, CompletionDate = c.CompletionDate,
                GraduationDate = c.GraduationDate, ClassHourStart = c.ClassHourStart, ClassHourEnd = c.ClassHourEnd,
                CreatedAt = c.CreatedAt, UpdatedAt = c.UpdatedAt,
                Center = new CenterResponse()
                {
                    Id = c.Center.Id, Name = c.Center.Name, CreatedAt = c.Center.CreatedAt,
                    UpdatedAt = c.Center.UpdatedAt,
                    Province = new ProvinceResponse()
                    {
                        Id = c.Center.Province.Id, Code = c.Center.Province.Code, Name = c.Center.Province.Name,
                    },
                    District = new DistrictResponse()
                    {
                        Id = c.Center.District.Id, Name = c.Center.District.Name, Prefix = c.Center.District.Prefix
                    },
                    Ward = new WardResponse()
                    {
                        Id = c.Center.Ward.Id, Name = c.Center.Ward.Name, Prefix = c.Center.Ward.Prefix
                    }
                },
                Course = new CourseResponse()
                {
                    Code = c.Course.Code, Name = c.Course.Name, IsActive = c.Course.IsActive,
                    SemesterCount = c.Course.SemesterCount, CourseFamilyCode = c.Course.CourseFamilyCode,
                    CreatedAt = c.Course.CreatedAt, UpdatedAt = c.Course.UpdatedAt,
                    CourseFamily = new CourseFamilyResponse()
                    {
                        Code = c.Course.CourseFamily.Code, Name = c.Course.CourseFamily.Name,
                        IsActive = c.Course.CourseFamily.IsActive, PublishedYear = c.Course.CourseFamily.PublishedYear,
                        CreatedAt = c.Course.CourseFamily.CreatedAt, UpdatedAt = c.Course.CourseFamily.UpdatedAt
                    }
                },
                ClassDays = new ClassDaysResponse()
                {
                    Id = c.ClassDays.Id, Value = c.ClassDays.Value
                },
                ClassStatus = new ClassStatusResponse()
                {
                    Id = c.ClassStatus.Id, Value = c.ClassStatus.Value
                }
            }).ToList();
        return Ok(CustomResponse.Ok("Classes retrieved successfully", classes));
    }
    
    
}