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
    private const int TheoryExam = 6;
    private const int PracticeExamResit = 7;
    private const int TheoryExamResit = 8;
    private const int FinalProject = 9;

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
            // calculate total each weight of grade category except theory exam and it's resit
            var totalEachWeight = request.GradeCategoryDetails
                .Where(gcd => gcd.GradeCategoryId != 6 && gcd.GradeCategoryId != 8).Sum(gcd => gcd.TotalWeight);

            if (totalEachWeight != 100)
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

            //check list request have practice exam and theory exam
            var pe = request.GradeCategoryDetails.FirstOrDefault(gcd => gcd.GradeCategoryId == PracticeExam);
            var te = request.GradeCategoryDetails.FirstOrDefault(gcd => gcd.GradeCategoryId == TheoryExam);
            // both PE & TE are required
            if (module.ExamType == 3 && (pe == null || te == null))
            {
                var error = ErrorDescription.Error["E0067"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            foreach (var gcd in request.GradeCategoryDetails)
            {
                // get grade category name in db
                var gradeCategoryName = _context.GradeCategories.Find(gcd.GradeCategoryId)!.Name;

                if (gcd.GradeCategoryId != TheoryExam && gcd.GradeCategoryId != TheoryExamResit
                                                      && gcd.TotalWeight <= 0)
                {
                    var error = ErrorDescription.Error["E0067_1"];
                    return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                }

                // not allow to create pe resit and te resit
                if (gcd.GradeCategoryId is PracticeExamResit or TheoryExamResit)
                {
                    var error = ErrorDescription.Error["E0066"];
                    return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                }

                // check if grade category in requested list isn't match with module exam type
                switch (module.ExamType)
                {
                    // theory exam(final exam) -> can't have practice exam
                    case 1:
                        if (gcd.GradeCategoryId is PracticeExam or PracticeExamResit)
                        {
                            var error = ErrorDescription.Error["E0063"];
                            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                        }

                        break;
                    // practical exam -> can't have theory exam
                    case 2:
                        if (gcd.GradeCategoryId is TheoryExam or TheoryExamResit)
                        {
                            var error = ErrorDescription.Error["E0064"];
                            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                        }

                        break;
                    // not take exam -> can't have practice exam and theory exam
                    case 4:
                        if (gcd.GradeCategoryId is PracticeExam or PracticeExamResit or TheoryExam or TheoryExamResit)
                        {
                            var error = ErrorDescription.Error["E0065"];
                            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                        }

                        break;
                }

                if (gcd.GradeCategoryId is PracticeExam or TheoryExam or PracticeExamResit or TheoryExamResit
                        or FinalProject
                    && gcd.QuantityGradeItem != 1)
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

                // theory exam weight not managed in this system -> 0%
                if (gcd.GradeCategoryId is TheoryExam or TheoryExamResit)
                {
                    gradeCategoryModule.TotalWeight = 0;
                }

                // auto create grade items
                for (var i = 0; i < gcd.QuantityGradeItem; i++)
                {
                    var gradeItem = new GradeItem()
                    {
                        GradeCategoryModuleId = gradeCategoryModule.Id,
                        Name = gradeCategoryName,
                    };
                    // if (!gradeCategoryName.Contains("Exam") ||
                    //     !gradeCategoryName.Contains("Final"))
                    if (gradeCategoryModule.GradeCategoryId is not PracticeExam or TheoryExam or FinalProject)
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
                    case TheoryExam:
                        gradeCategoryModuleResit.GradeCategoryId = TheoryExamResit;
                        // theory exam weight not managed in this system -> 0%
                        gradeCategoryModuleResit.TotalWeight = 0;
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

        return Ok(CustomResponse.Ok("Create|Update grade category module successfully", response));
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