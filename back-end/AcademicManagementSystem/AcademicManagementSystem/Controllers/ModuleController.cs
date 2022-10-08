using System.Text;
using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.CenterController;
using AcademicManagementSystem.Models.CourseModuleSemester;
using AcademicManagementSystem.Models.ModuleController;
using Microsoft.AspNetCore.Authorization;
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
            .Include(m => m.CoursesModulesSemesters)
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
                },
                CourseModuleSemester = new CourseModuleSemesterResponse()
                {
                    CourseCode = m.CoursesModulesSemesters.FirstOrDefault()?.CourseCode,
                    ModuleId = m.CoursesModulesSemesters.FirstOrDefault()?.ModuleId,
                    SemesterId = m.CoursesModulesSemesters.FirstOrDefault()?.SemesterId
                }
            });
        return Ok(CustomResponse.Ok("Modules retrieved successfully", modules));
    }

    // get module by id
    [HttpGet]
    [Route("api/modules/{id}")]
    public IActionResult GetModuleById(int id)
    {
        var module = _context.Modules.Include(m => m.Center)
            .Include(m => m.Center.Province)
            .Include(m => m.Center.District)
            .Include(m => m.Center.Ward)
            .Include(m => m.CoursesModulesSemesters)
            .FirstOrDefault(m => m.Id == id);
        if (module == null)
        {
            return NotFound(CustomResponse.NotFound("Module not found"));
        }

        var moduleResponse = GetModuleResponse(module);
        return Ok(CustomResponse.Ok("Module retrieved successfully", moduleResponse));
    }

    // create module with course code and semester id
    [HttpPost]
    [Route("api/modules")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult CreateModule([FromBody] CreateModuleRequest request)
    {
        request.CourseCode = request.CourseCode?.ToUpper().Trim();
        request.ModuleName = request.ModuleName?.Trim();
        request.ModuleExamNamePortal = request.ModuleExamNamePortal?.Trim();
        request.ModuleType = request.ModuleType?.Trim();
        request.ExamType = request.ExamType?.Trim();
        request.SemesterNamePortal = request.SemesterNamePortal?.Trim();

        //check empty
        if (string.IsNullOrEmpty(request.CourseCode) || string.IsNullOrEmpty(request.ModuleName) ||
            string.IsNullOrEmpty(request.ModuleExamNamePortal) || string.IsNullOrEmpty(request.ModuleType) ||
            string.IsNullOrEmpty(request.ExamType) || string.IsNullOrEmpty(request.SemesterNamePortal))
        {
            var error = ErrorDescription.Error["E1023"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check course code
        request.CourseCode = Regex.Replace(request.CourseCode, StringConstant.RegexWhiteSpaces, " ");
        request.CourseCode = request.CourseCode.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.CourseCode, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1026"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.CourseCode.Length > 100)
        {
            var error = ErrorDescription.Error["E1027"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // is course code exist
        var course = _context.Courses.FirstOrDefault(c => c.Code == request.CourseCode);
        if (course == null)
        {
            var error = ErrorDescription.Error["E1038"];
            return NotFound(CustomResponse.NotFound(error.Message));
        }

        // is center id exist
        var center = _context.Centers.FirstOrDefault(c => c.Id == request.CenterId);
        if (center == null)
        {
            var error = ErrorDescription.Error["E1039"];
            return NotFound(CustomResponse.NotFound(error.Message));
        }

        // is semester id exist
        var semester = _context.Semesters.FirstOrDefault(s => s.Id == request.SemesterId);
        if (semester == null)
        {
            var error = ErrorDescription.Error["E1040"];
            return NotFound(CustomResponse.NotFound(error.Message));
        }

        //check module name
        request.ModuleName = Regex.Replace(request.ModuleName, StringConstant.RegexWhiteSpaces, " ");
        request.ModuleName = request.ModuleName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ModuleName, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1024"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.ModuleName.Length > 255)
        {
            var error = ErrorDescription.Error["E1025"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check exam name portal
        request.ModuleExamNamePortal =
            Regex.Replace(request.ModuleExamNamePortal, StringConstant.RegexWhiteSpaces, " ");
        request.ModuleExamNamePortal = request.ModuleExamNamePortal.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ModuleExamNamePortal, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1028"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.ModuleExamNamePortal.Length > 255)
        {
            var error = ErrorDescription.Error["E1029"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check module type
        request.ModuleType = Regex.Replace(request.ModuleType, StringConstant.RegexWhiteSpaces, " ");
        request.ModuleType = request.ModuleType.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ModuleType, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1030"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.ModuleType.Length > 100)
        {
            var error = ErrorDescription.Error["E1031"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check exam type
        request.ExamType = Regex.Replace(request.ExamType, StringConstant.RegexWhiteSpaces, " ");
        request.ExamType = request.ExamType.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ExamType, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1032"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.ExamType.Length > 100)
        {
            var error = ErrorDescription.Error["E1033"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check semester name portal
        request.SemesterNamePortal = Regex.Replace(request.SemesterNamePortal, StringConstant.RegexWhiteSpaces, " ");
        request.SemesterNamePortal = request.SemesterNamePortal.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.SemesterNamePortal, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1034"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.SemesterNamePortal.Length > 255)
        {
            var error = ErrorDescription.Error["E1035"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check days
        if (request.Days is null or < 1)
        {
            var error = ErrorDescription.Error["E1036"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check hours
        if (request.Hours is null or < 1)
        {
            var error = ErrorDescription.Error["E1037"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var module = new Module()
        {
            CenterId = request.CenterId,
            SemesterNamePortal = request.SemesterNamePortal,
            ModuleName = request.ModuleName,
            ModuleExamNamePortal = request.ModuleExamNamePortal,
            ModuleType = request.ModuleType,
            MaxTheoryGrade = request.MaxTheoryGrade,
            MaxPracticalGrade = request.MaxPracticalGrade,
            Hours = request.Hours,
            Days = request.Days,
            ExamType = request.ExamType,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        var courseCode = request.CourseCode;
        var semesterId = request.SemesterId;

        // is course code, semester id, center id exist
        var courseSemesterCenter = GetCourseSemesterCenter(request, courseCode, semesterId);
        if (!courseSemesterCenter.Any())
        {
            _context.Modules.Add(module);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                var error = ErrorDescription.Error["E1021"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            var courseModuleSemester = new CourseModuleSemester()
            {
                CourseCode = courseCode,
                ModuleId = module.Id,
                SemesterId = semesterId
            };

            _context.CoursesModulesSemesters.Add(courseModuleSemester);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                var error = ErrorDescription.Error["E1022"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }
        }
        else
        {
            var error = ErrorDescription.Error["E1041"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var newModule = _context.Modules.Include(m => m.Center)
            .Include(m => m.Center.Province)
            .Include(m => m.Center.District)
            .Include(m => m.Center.Ward)
            .Include(m => m.CoursesModulesSemesters)
            .FirstOrDefault(m => m.Id == module.Id);
        if (newModule == null)
        {
            return NotFound(CustomResponse.NotFound("Module not found"));
        }

        var moduleResponse = GetModuleResponse(newModule);
        return Ok(CustomResponse.Ok("Module created successfully", moduleResponse));
    }

    private IQueryable<Module> GetCourseSemesterCenter(CreateModuleRequest request, string courseCode, int? semesterId)
    {
        var courseSemesterCenter = _context.Modules
            .Join(_context.CoursesModulesSemesters, m => m.Id, cms => cms.ModuleId, (m, cms) => new { m, cms })
            .Where(@t => @t.cms.CourseCode == courseCode && @t.cms.SemesterId == semesterId &&
                         @t.m.CenterId == request.CenterId)
            .Select(@t => @t.m);
        return courseSemesterCenter;
    }

    // update module by id
    [HttpPut]
    [Route("api/modules/{id}")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult UpdateModuleById(int id, [FromBody] UpdateModuleRequest request)
    {
        var module = _context.Modules.Include(m => m.Center)
            .Include(m => m.Center.Province)
            .Include(m => m.Center.District)
            .Include(m => m.Center.Ward)
            .Include(m => m.CoursesModulesSemesters)
            .FirstOrDefault(m => m.Id == id);
        if (module == null)
        {
            return NotFound(CustomResponse.NotFound("Module not found"));
        }

        request.ModuleName = request.ModuleName?.Trim();
        request.ModuleExamNamePortal = request.ModuleExamNamePortal?.Trim();
        request.ModuleType = request.ModuleType?.Trim();
        request.ExamType = request.ExamType?.Trim();
        request.SemesterNamePortal = request.SemesterNamePortal?.Trim();

        //check empty
        if (string.IsNullOrEmpty(request.ModuleName) || string.IsNullOrEmpty(request.ModuleExamNamePortal) ||
            string.IsNullOrEmpty(request.ModuleType) ||
            string.IsNullOrEmpty(request.ExamType) || string.IsNullOrEmpty(request.SemesterNamePortal))
        {
            var error = ErrorDescription.Error["E1023"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // is center id exist
        var center = _context.Centers.FirstOrDefault(c => c.Id == request.CenterId);
        if (center == null)
        {
            var error = ErrorDescription.Error["E1039"];
            return NotFound(CustomResponse.NotFound(error.Message));
        }

        //check module name
        request.ModuleName = Regex.Replace(request.ModuleName, StringConstant.RegexWhiteSpaces, " ");
        request.ModuleName = request.ModuleName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ModuleName, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1024"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.ModuleName.Length > 255)
        {
            var error = ErrorDescription.Error["E1025"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check exam name portal
        request.ModuleExamNamePortal =
            Regex.Replace(request.ModuleExamNamePortal, StringConstant.RegexWhiteSpaces, " ");
        request.ModuleExamNamePortal = request.ModuleExamNamePortal.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ModuleExamNamePortal, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1028"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.ModuleExamNamePortal.Length > 255)
        {
            var error = ErrorDescription.Error["E1029"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check module type
        request.ModuleType = Regex.Replace(request.ModuleType, StringConstant.RegexWhiteSpaces, " ");
        request.ModuleType = request.ModuleType.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ModuleType, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1030"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.ModuleType.Length > 100)
        {
            var error = ErrorDescription.Error["E1031"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check exam type
        request.ExamType = Regex.Replace(request.ExamType, StringConstant.RegexWhiteSpaces, " ");
        request.ExamType = request.ExamType.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ExamType, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1032"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.ExamType.Length > 100)
        {
            var error = ErrorDescription.Error["E1033"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check semester name portal
        request.SemesterNamePortal = Regex.Replace(request.SemesterNamePortal, StringConstant.RegexWhiteSpaces, " ");
        request.SemesterNamePortal = request.SemesterNamePortal.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.SemesterNamePortal, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1034"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.SemesterNamePortal.Length > 255)
        {
            var error = ErrorDescription.Error["E1035"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check days
        if (request.Days is null or < 1)
        {
            var error = ErrorDescription.Error["E1036"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // check hours
        if (request.Hours is null or < 1)
        {
            var error = ErrorDescription.Error["E1037"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // update
        try
        {
            module.CenterId = request.CenterId;
            module.SemesterNamePortal = request.SemesterNamePortal;
            module.ModuleName = request.ModuleName;
            module.ModuleExamNamePortal = request.ModuleExamNamePortal;
            module.ModuleType = request.ModuleType;
            module.MaxTheoryGrade = request.MaxTheoryGrade;
            module.MaxPracticalGrade = request.MaxPracticalGrade;
            module.Hours = request.Hours;
            module.Days = request.Days;
            module.ExamType = request.ExamType;
            module.UpdatedAt = DateTime.Now;
            _context.Modules.Update(module);

            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1042"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var moduleResponse = GetModuleResponse(module);
        return Ok(CustomResponse.Ok("Module updated successfully", moduleResponse));
    }

    // search module by module name, course code, module type, exam type
    [HttpGet]
    [Route("api/modules/search")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult SearchModule(string? moduleName, string? courseCode,
        string? moduleType, string? examType)
    {
        var sModuleName = moduleName?.Trim() == null ? string.Empty : RemoveDiacritics(moduleName.Trim().ToUpper());
        var sCourseCode = courseCode?.Trim() == null ? string.Empty : RemoveDiacritics(courseCode.Trim().ToUpper());
        var sModuleType = moduleType?.Trim() == null ? string.Empty : RemoveDiacritics(moduleType.Trim().ToUpper());
        var sExamType = examType?.Trim() == null ? string.Empty : RemoveDiacritics(examType.Trim().ToUpper());
        if(sModuleName == String.Empty && sCourseCode == String.Empty && sModuleType == String.Empty && sExamType == String.Empty)
        {
            var modules = GetAllModules();
            return Ok(CustomResponse.Ok("Module search successfully", modules));
        }

        var listModules = GetAllModules();
        var moduleResponse = new List<ModuleResponse>();
        foreach (var module in listModules)
        {
            var s1 = RemoveDiacritics(module.ModuleName!.ToUpper());
            var s2 = RemoveDiacritics(module.CourseModuleSemester!.CourseCode!.ToUpper());
            var s3 = RemoveDiacritics(module.ModuleType!.ToUpper());
            var s4 = RemoveDiacritics(module.ExamType!.ToUpper());
            
            if (s1.Contains(sModuleName) && s2.Contains(sCourseCode) && s3.Contains(sModuleType) && s4.Contains(sExamType))
            {
                moduleResponse.Add(module);
            }
        }
        return Ok(CustomResponse.Ok("Module search successfully", moduleResponse));
    }

    // func get all module
    private IEnumerable<ModuleResponse> GetAllModules()
    {
        var modules = _context.Modules
            .Include(m => m.Center)
            .Include(m => m.CoursesModulesSemesters)
            .Include(m => m.Center.Province)
            .Include(m => m.Center.District)
            .Include(m => m.Center.Ward)
            .ToList()
            .Select(GetModuleResponse);
        
        return modules;
    }
    
    private static ModuleResponse GetModuleResponse(Module module)
    {
        var moduleResponse = new ModuleResponse()
        {
            Id = module.Id, CenterId = module.CenterId, SemesterNamePortal = module.SemesterNamePortal,
            ModuleName = module.ModuleName,
            ModuleExamNamePortal = module.ModuleExamNamePortal, ModuleType = module.ModuleType,
            MaxTheoryGrade = module.MaxTheoryGrade, MaxPracticalGrade = module.MaxPracticalGrade, Hours = module.Hours,
            Days = module.Days, ExamType = module.ExamType, CreatedAt = module.CreatedAt, UpdatedAt = module.UpdatedAt,
            Center = new CenterResponse()
            {
                Id = module.Center.Id,
                Name = module.Center.Name, CreatedAt = module.Center.CreatedAt, UpdatedAt = module.Center.UpdatedAt,
                Province = new ProvinceResponse()
                {
                    Id = module.Center.Province.Id,
                    Name = module.Center.Province.Name,
                    Code = module.Center.Province.Code
                },
                District = new DistrictResponse()
                {
                    Id = module.Center.District.Id,
                    Name = module.Center.District.Name,
                    Prefix = module.Center.District.Prefix,
                },
                Ward = new WardResponse()
                {
                    Id = module.Center.Ward.Id,
                    Name = module.Center.Ward.Name,
                    Prefix = module.Center.Ward.Prefix,
                }
            },
            CourseModuleSemester = new CourseModuleSemesterResponse()
            {
                CourseCode = module.CoursesModulesSemesters.FirstOrDefault()?.CourseCode,
                ModuleId = module.CoursesModulesSemesters.FirstOrDefault()?.ModuleId,
                SemesterId = module.CoursesModulesSemesters.FirstOrDefault()?.SemesterId,
            }
        };
        return moduleResponse;
    }
    
    private static string RemoveDiacritics(string text)
    {
        //regex pattern to remove diacritics
        const string pattern = "\\p{IsCombiningDiacriticalMarks}+";

        var normalizedStr = text.Normalize(NormalizationForm.FormD);

        return Regex.Replace(normalizedStr, pattern, string.Empty)
            .Replace('\u0111', 'd')
            .Replace('\u0110', 'D');
    }
}