using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.CenterController;
using AcademicManagementSystem.Models.CourseModuleSemester;
using AcademicManagementSystem.Models.ModuleController;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class ModuleController : ControllerBase
{
    private readonly AmsContext _context;

    public ModuleController(AmsContext context)
    {
        _context = context;
    }

    // get all modules
    [HttpGet]
    [Route("api/modules")]
    public IActionResult GetModules()
    {
        var modules = _context.Modules.Include(m => m.Center)
            .Include(m => m.Center.Province)
            .Include(m => m.Center.District)
            .Include(m => m.Center.Ward)
            .ToList()
            .Select(m => new ModuleResponse()
            {
                Id = m.Id, CenterId = m.CenterId, SemesterNamePortal = m.SemesterNamePortal, ModuleName = m.ModuleName,
                ModuleExamNamePortal = m.ModuleExamNamePortal, ModuleType = m.ModuleType,
                MaxTheoryGrade = m.MaxTheoryGrade, MaxPracticalGrade = m.MaxPracticalGrade, Hours = m.Hours,
                Days = m.Days, ExamType = m.ExamType, CreatedAt = m.CreatedAt, UpdatedAt = m.UpdatedAt,
                Center = new CenterResponse()
                {
                    Id = m.Center.Id,
                    Name = m.Center.Name, CreatedAt = m.Center.CreatedAt, UpdatedAt = m.Center.UpdatedAt,
                    Province = new ProvinceResponse()
                    {
                        Id = m.Center.Province.Id,
                        Name = m.Center.Province.Name,
                        Code = m.Center.Province.Code
                    },
                    District = new DistrictResponse()
                    {
                        Id = m.Center.District.Id,
                        Name = m.Center.District.Name,
                        Prefix = m.Center.District.Prefix,
                    },
                    Ward = new WardResponse()
                    {
                        Id = m.Center.Ward.Id,
                        Name = m.Center.Ward.Name,
                        Prefix = m.Center.Ward.Prefix,
                    }
                }
            });
        return Ok(CustomResponse.Ok("Modules retrieved successfully", modules));
    }
    
    
}