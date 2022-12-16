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
    private const int ExamTypeTe = 1;
    private const int ExamTypePe = 2;
    private const int ExamTypeBothPeAndTe = 3;
    private const int ExamTypeNoTakeExam = 4;

    public GradeCategoryModuleController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/modules/{moduleId:int}/grades")]
    [Authorize(Roles = "admin")]
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


    /*
     * Manage score of a module (can't change grade category type: theory exam)
     */
    [HttpPost]
    [Route("api/modules/{moduleId:int}/grades")]
    [Authorize(Roles = "admin")]
    public IActionResult UpdateGradeCategoryModule(int moduleId,
        [FromBody] CreateGradeCategoryModuleRequest request)
    {
        var module = _context.Modules.FirstOrDefault(m => m.Id == moduleId);
        if (module == null)
        {
            var error = ErrorDescription.Error["E0062"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check module is learning 
        var isModuleInSchedule = _context.Modules
            .Include(m => m.ClassSchedules)
            .Any(m => m.Id == moduleId && m.ClassSchedules.Any());

        if (isModuleInSchedule)
        {
            var error = ErrorDescription.Error["E0067_5"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        RemoveDataGradeCategoryAndGradeItem(moduleId);

        // not null and count > 0
        if (request.GradeCategoryDetails is { Count: > 0 })
        {
            // ExamTypePe: only take theory exam -> can't add any grade category (because theory exam is default and auto add later)
            // ExamTypeNoTakeExam: not take exam -> add only grade category FINAL PROJECT (auto add later)
            if (module.ExamType is ExamTypeTe or ExamTypeNoTakeExam)
            {
                var error = ErrorDescription.Error["E0067_3"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            // check duplicate grade category id
            var duplicateGradeCategoryId = request.GradeCategoryDetails
                .GroupBy(g => g.GradeCategoryId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateGradeCategoryId.Any())
            {
                var error = ErrorDescription.Error["E0067_4"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            // not manage theory exam, this grade category will auto add later
            if (request.GradeCategoryDetails.Any(x => x.GradeCategoryId is TheoryExam))
            {
                var error = ErrorDescription.Error["E0067_2"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            // not allow to create pe resit and te resit
            if (request.GradeCategoryDetails.Any(x => x.GradeCategoryId is PracticeExamResit or TheoryExamResit))
            {
                var error = ErrorDescription.Error["E0066"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            // calculate total weight of grade categories requested
            var totalWeight = request.GradeCategoryDetails.Sum(gcd => gcd.TotalWeight);

            if (totalWeight != 100)
            {
                var error = ErrorDescription.Error["E0057"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            //check list request have practice exam
            var isHavePeExam = request.GradeCategoryDetails.Any(gcd => gcd.GradeCategoryId == PracticeExam);
            // practice exam or both exam but don't have practice exam 
            if (module.ExamType is ExamTypePe or ExamTypeBothPeAndTe && !isHavePeExam)
            {
                var error = ErrorDescription.Error["E0067"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            foreach (var gcd in request.GradeCategoryDetails)
            {
                // get grade category name in db
                var gradeCategoryName = _context.GradeCategories.Find(gcd.GradeCategoryId)!.Name;

                if (gcd.TotalWeight <= 0)
                {
                    var error = ErrorDescription.Error["E0067_1"];
                    return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                }

                // exams or final project must has only one item
                if (gcd.GradeCategoryId is PracticeExam or FinalProject && gcd.QuantityGradeItem != 1)
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
                    if (gcd.QuantityGradeItem > 1)
                    {
                        gradeItem.Name = gradeCategoryName + $" {i + 1}";
                    }

                    gradeCategoryModule.GradeItems.Add(gradeItem);
                }

                _context.GradeCategoryModules.Add(gradeCategoryModule);

                // auto create grade category module resit and its grade items for PE
                if (gcd.GradeCategoryId == PracticeExam)
                {
                    var gradeCategoryModuleResit = new GradeCategoryModule()
                    {
                        ModuleId = moduleId,
                        GradeCategoryId = PracticeExamResit,
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
                    _context.GradeCategoryModules.Add(gradeCategoryModuleResit);
                }
            }

            // auto add theory exam type
            if (module.ExamType == ExamTypeBothPeAndTe)
            {
                AutoAddTheoryExam(module);
            }
        }
        /*
         * case request null or count = 0
         * - module exam type is te(1) or both(3) pe & te -> auto add theory exam
         * - module exam type is pe(4) -> auto 
         */
        else
        {
            switch (module.ExamType)
            {
                // auto add theory exam and theory exam resit if module exam type is 1(theory exam)
                case ExamTypeTe:
                    AutoAddTheoryExam(module);
                    break;
                // auto add final project when no take exam
                case ExamTypeNoTakeExam:
                    AutoAddFinalProject(module);
                    break;
            }
        }
        
        module.UpdatedAt = DateTime.Now;

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

    private void AutoAddTheoryExam(Module module)
    {
        var theoryExamName = _context.GradeCategories.Find(TheoryExam)!.Name;

        // add theory exam
        var te = new GradeCategoryModule()
        {
            ModuleId = module.Id,
            GradeCategoryId = TheoryExam,
            // theory exam weight not managed in this system -> 0%
            TotalWeight = 0,
            QuantityGradeItem = 1,
            GradeItems = new List<GradeItem>()
            {
                new GradeItem()
                {
                    Name = theoryExamName
                }
            }
        };

        // add theory exam resit
        var teResit = new GradeCategoryModule()
        {
            ModuleId = module.Id,
            GradeCategoryId = TheoryExamResit,
            TotalWeight = 0,
            QuantityGradeItem = 1,
            GradeItems = new List<GradeItem>()
            {
                new GradeItem()
                {
                    Name = theoryExamName + " Resit"
                }
            }
        };

        _context.GradeCategoryModules.Add(te);
        _context.GradeCategoryModules.Add(teResit);
    }

    private void AutoAddFinalProject(Module module)
    {
        var finalProjectName = _context.GradeCategories.Find(FinalProject)!.Name;

        // add final project
        var fp = new GradeCategoryModule()
        {
            ModuleId = module.Id,
            GradeCategoryId = FinalProject,
            TotalWeight = 100,
            QuantityGradeItem = 1,
            GradeItems = new List<GradeItem>()
            {
                new GradeItem()
                {
                    Name = finalProjectName
                }
            }
        };

        _context.GradeCategoryModules.Add(fp);
    }
}