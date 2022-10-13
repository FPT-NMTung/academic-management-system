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
    private const int PracticeExam = 5;
    private const int FinalExam = 6;
    private const int PracticeExamResit = 7;
    private const int FinalExamResit = 8;

    public GradeCategoryModuleController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/modules/{moduleId:int}/grades")]
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
    [Route("api/modules/{moduleId:int}/grades")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult CreateGradeCategoryModule(int moduleId, [FromBody] CreateGradeCategoryModuleRequest request)
    {
        RemoveDataGradeCategoryAndGradeItem(moduleId);
        if (request.GradeCategoryDetails != null)
        {
            var eachWeight = request.GradeCategoryDetails.Sum(gcd => gcd.TotalWeight);

            if (eachWeight != 100)
            {
                var error = ErrorDescription.Error["E0057"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            var module = _context.Modules.FirstOrDefault(m => m.Id == moduleId);
            if (module == null)
            {
                var error = ErrorDescription.Error["E0062"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            //check list request have practice exam and final exam
            var pe = request.GradeCategoryDetails.FirstOrDefault(gcd => gcd.GradeCategoryId == PracticeExam);
            var fe = request.GradeCategoryDetails.FirstOrDefault(gcd => gcd.GradeCategoryId == FinalExam);
            // both PE & FE are required
            if (module.ExamType == 3 && (pe == null || fe == null))
            {
                var error = ErrorDescription.Error["E0067"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            foreach (var gcd in request.GradeCategoryDetails)
            {
                var gradeCategoryName = _context.GradeCategories.Find(gcd.GradeCategoryId)!.Name;

                if (gcd.GradeCategoryId is PracticeExamResit or FinalExamResit)
                {
                    var error = ErrorDescription.Error["E0066"];
                    return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                }

                switch (module.ExamType)
                {
                    // theory exam(final exam)
                    case 1:
                        if (gcd.GradeCategoryId is PracticeExam or PracticeExamResit)
                        {
                            var error = ErrorDescription.Error["E0063"];
                            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                        }

                        break;
                    // practical exam
                    case 2:
                        if (gcd.GradeCategoryId is FinalExam or FinalExamResit)
                        {
                            var error = ErrorDescription.Error["E0064"];
                            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                        }

                        break;
                    // not take exam
                    case 4:
                        if (gcd.GradeCategoryId is PracticeExam or PracticeExamResit or FinalExam or FinalExamResit)
                        {
                            var error = ErrorDescription.Error["E0065"];
                            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                        }

                        break;
                }

                // final exam must have only one
                if (gcd.GradeCategoryId is PracticeExam or FinalExam or PracticeExamResit or FinalExamResit &&
                    gcd.QuantityGradeItem != 1)
                {
                    var error = ErrorDescription.Error["E0060"];
                    return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                }

                if (gcd.QuantityGradeItem is < 1 or > 10)
                {
                    var error = ErrorDescription.Error["E0061"];
                    return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                }

                var gradeCategoryModule = new GradeCategoryModule()
                {
                    ModuleId = moduleId,
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
                    if (!gradeCategoryName.Contains("Exam") ||
                        !gradeCategoryName.Contains("Final"))
                    {
                        gradeItem.Name = gradeCategoryName + $" {i + 1}";
                    }

                    gradeCategoryModule.GradeItems.Add(gradeItem);
                }

                _context.GradeCategoryModules.Add(gradeCategoryModule);

                // auto create grade category module resit and its grade items
                var gradeCategoryModuleResit = new GradeCategoryModule()
                {
                    ModuleId = moduleId,
                    GradeCategoryId = gcd.GradeCategoryId,
                    TotalWeight = gcd.TotalWeight,
                    QuantityGradeItem = 1,
                    GradeItems = new List<GradeItem>()
                    {
                        new GradeItem()
                        {
                            Name = gradeCategoryName + " Resit"
                        }
                    }
                };

                switch (gcd.GradeCategoryId)
                {
                    // practice exam
                    case PracticeExam:
                        gradeCategoryModuleResit.GradeCategoryId = PracticeExamResit;
                        _context.GradeCategoryModules.Add(gradeCategoryModuleResit);
                        break;
                    // final exam
                    case FinalExam:
                        gradeCategoryModuleResit.GradeCategoryId = FinalExamResit;
                        _context.GradeCategoryModules.Add(gradeCategoryModuleResit);
                        break;
                }
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

        var response = GetGradeCategoryModuleResponsesByModuleId(moduleId);

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

    private void RemoveDataGradeCategoryAndGradeItem(int moduleId)
    {
        // delete all grade items by the grade category module id
        var gradeItems =
            _context.GradeItems
                .Include(gi => gi.GradeCategoryModule)
                .ThenInclude(gcd => gcd.Module)
                .Where(gi => gi.GradeCategoryModule.ModuleId == moduleId);
        _context.GradeItems.RemoveRange(gradeItems);

        // delete all grade category module by module id
        var gradeCategoryModules =
            _context.GradeCategoryModules.Where(gcm => gcm.ModuleId == moduleId);
        _context.GradeCategoryModules.RemoveRange(gradeCategoryModules);
    }
}