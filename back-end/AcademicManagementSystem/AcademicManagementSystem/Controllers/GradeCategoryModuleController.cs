using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.GradeCategoryController;
using AcademicManagementSystem.Models.GradeCategoryModule;
using AcademicManagementSystem.Models.GradeItemController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class GradeCategoryModuleController : ControllerBase
{
    private readonly AmsContext _context;

    public GradeCategoryModuleController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/grade-categories-modules/{moduleId:int}")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetGradeCategoriesDetailByModuleId(int moduleId)
    {
        if (_context.Modules.Find(moduleId) == null)
        {
            var error = ErrorDescription.Error["E0056"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var response = GetGradeCategoryModuleResponsesByModuleId(moduleId);
        return Ok(CustomResponse.Ok("Get all grade category module successfully", response));
    }

    [HttpPost]
    [Route("api/grade-categories-modules")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult CreateGradeCategoryModule([FromBody] CreateGradeCategoryModuleRequest request)
    {
        // delete all grade items by the grade category module id
        var gradeItems =
            _context.GradeItems
                .Include(gi => gi.GradeCategoryModule)
                .ThenInclude(gcd => gcd.Module)
                .Where(gi => gi.GradeCategoryModule.ModuleId == request.ModuleId);
        _context.GradeItems.RemoveRange(gradeItems);

        // delete all grade category module by module id
        var gradeCategoryModules =
            _context.GradeCategoryModules.Where(gcm => gcm.ModuleId == request.ModuleId);
        _context.GradeCategoryModules.RemoveRange(gradeCategoryModules);

        if (request.GradeCategoryDetails != null)
        {
            var eachWeight = request.GradeCategoryDetails.Sum(gcd => gcd.TotalWeight);

            if (eachWeight != 100)
            {
                var error = ErrorDescription.Error["E0057"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            foreach (var gcd in request.GradeCategoryDetails)
            {
                var gradeCategoryName = _context.GradeCategories.Find(gcd.GradeCategoryId)!.Name;

                var gradeCategoryModule = new GradeCategoryModule()
                {
                    ModuleId = request.ModuleId,
                    GradeCategoryId = gcd.GradeCategoryId,
                    TotalWeight = gcd.TotalWeight,
                    QuantityGradeItem = gcd.QuantityGradeItem,
                    GradeItems = new List<GradeItem>()
                };

                // auto create grade items
                for (var i = 0; i < gcd.QuantityGradeItem; i++)
                {
                    var gradeItem = new GradeItem()
                    {
                        GradeCategoryModuleId = gradeCategoryModule.Id,
                        Name = gradeCategoryName,
                    };
                    if (!gradeCategoryName.Contains("Exam") || !gradeCategoryName.Contains("Final"))
                    {
                        gradeItem.Name = gradeCategoryName + $" {i + 1}";
                    }

                    gradeCategoryModule.GradeItems.Add(gradeItem);
                }

                _context.GradeCategoryModules.Add(gradeCategoryModule);
            }
        }

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0058"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var response = GetGradeCategoryModuleResponsesByModuleId(request.ModuleId);


        return Ok(CustomResponse.Ok("Create grade category module successfully", response));
    }

    private IEnumerable<GradeCategoryModuleResponse> GetGradeCategoryModuleResponsesByModuleId(int moduleId)
    {
        return _context.GradeCategoryModules
            .Include(gcm => gcm.Module)
            .Include(gcm => gcm.GradeCategory)
            .Where(gcm => gcm.Module.Id == moduleId)
            .Select(gcm => new GradeCategoryModuleResponse()
            {
                Id = gcm.Id,
                GradeCategory = new GradeCategoryResponse()
                {
                    Id = gcm.GradeCategory.Id,
                    Name = gcm.GradeCategory.Name
                },
                TotalWeight = gcm.TotalWeight,
                QuantityGradeItem = gcm.QuantityGradeItem,
                GradeItems = gcm.GradeItems.Select(gi => new GradeItemResponse()
                {
                    Id = gi.Id,
                    Name = gi.Name
                }).ToList()
            });
    }
}