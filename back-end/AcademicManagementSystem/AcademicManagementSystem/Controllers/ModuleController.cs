using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
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
public class ModuleController : ControllerBase
{
    private readonly AmsContext _context;
    private const int PracticeExam = 5;
    private const int TheoryExam = 6;
    private const int PracticeExamResit = 7;
    private const int TheoryExamResit = 8;
    private const int FinalProject = 9;

    public ModuleController(AmsContext context)
    {
        _context = context;
    }

    // get all modules
    [HttpGet]
    [Route("api/modules")]
    [Authorize(Roles = "admin,sro,teacher,student")]
    public IActionResult GetModules()
    {
        var modules = GetAllModules();
        return Ok(CustomResponse.Ok("Modules retrieved successfully", modules));
    }

    // get module by id
    [HttpGet]
    [Route("api/modules/{id:int}")]
    [Authorize(Roles = "admin,sro,teacher,student")]
    public IActionResult GetModuleById(int id)
    {
        var module = _context.Modules.Include(m => m.Center)
            .Include(m => m.Center.Province)
            .Include(m => m.Center.District)
            .Include(m => m.Center.Ward)
            .Include(m => m.CoursesModulesSemesters)
            .ThenInclude(cms => cms.Semester)
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
    [Authorize(Roles = "admin")]
    public IActionResult CreateModule([FromBody] CreateModuleRequest request)
    {
        request.ModuleName = request.ModuleName.Trim();
        request.ModuleExamNamePortal = request.ModuleExamNamePortal.Trim();
        request.SemesterNamePortal = request.SemesterNamePortal.Trim();
        var listCourseCodes = request.CourseCode;
        var semesterId = request.SemesterId;

        if (CheckCourseCenterSemesterExisted(request, listCourseCodes, out var notFound)) return notFound;

        if (CheckStringNameRequestCreate(request, out var badRequest)) return badRequest;

        if (CheckModuleTypeAndExamTypeRequestCreate(request, out var badRequest1)) return badRequest1;

        if (CheckDayAndHourRequest(request.Days, request.Hours, out var badRequest2)) return badRequest2;

        // moduleName existed
        if (IsModuleNameExisted(request.ModuleName, request.CenterId))
        {
            var error = ErrorDescription.Error["E1053"];
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
        // add module
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

        // add to course_module_semester
        foreach (var courseCode in listCourseCodes)
        {
            // course code, module id, semester id existed
            if (_context.CoursesModulesSemesters.Any(cms =>
                    cms.CourseCode == courseCode && cms.ModuleId == module.Id && cms.SemesterId == semesterId))
            {
                var error = ErrorDescription.Error["1041"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }

            var courseModuleSemester = new CourseModuleSemester()
            {
                CourseCode = courseCode.ToUpper().Trim(),
                ModuleId = module.Id,
                SemesterId = semesterId
            };

            _context.CoursesModulesSemesters.Add(courseModuleSemester);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                var error = ErrorDescription.Error["E1022"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }
        }

        var createdModule = GetCoursesModulesSemestersByModuleId(module.Id).FirstOrDefault();
        if (createdModule == null)
        {
            var error = ErrorDescription.Error["E1045"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        AddDataToGradeCategoryModule(module.Id, request.ExamType);

        return Ok(CustomResponse.Ok("Module created successfully", createdModule));
    }

    private bool IsModuleNameExisted(string moduleName, int centerId)
    {
        return _context.Modules.Any(m =>
            m.CenterId == centerId && string.Equals(m.ModuleName.ToLower(), moduleName.ToLower()));
    }

    private bool CheckCourseCenterSemesterExisted(CreateModuleRequest request, IEnumerable<string> listCourseCodes,
        out IActionResult notFound)
    {
        // is course code exist
        if (listCourseCodes.Select(cc => _context.Courses.FirstOrDefault(c => c.Code == cc.ToUpper().Trim()))
            .Any(course => course == null))
        {
            var error = ErrorDescription.Error["E1038"];
            notFound = NotFound(CustomResponse.NotFound(error.Message));
            return true;
        }

        // is center id exist
        var center = _context.Centers.FirstOrDefault(c => c.Id == request.CenterId);
        if (center == null)
        {
            var error = ErrorDescription.Error["E1039"];
            notFound = NotFound(CustomResponse.NotFound(error.Message));
            return true;
        }

        // is semester id exist
        var semester = _context.Semesters.FirstOrDefault(s => s.Id == request.SemesterId);
        if (semester == null)
        {
            var error = ErrorDescription.Error["E1040"];
            notFound = NotFound(CustomResponse.NotFound(error.Message));
            return true;
        }

        notFound = null!;
        return false;
    }

    private bool CheckDayAndHourRequest(int days, int hours, out IActionResult badRequestObjectResult)
    {
        // check days
        if (days < 1)
        {
            var error = ErrorDescription.Error["E1036"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // check hours
        if (hours < 1)
        {
            var error = ErrorDescription.Error["E1037"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        badRequestObjectResult = null!;
        return false;
    }

    private bool CheckModuleTypeAndExamTypeRequestCreate(CreateModuleRequest request, out IActionResult badRequest)
    {
        // module type and exam type
        if (request.ModuleType is < 1 or > 3)
        {
            var error = ErrorDescription.Error["E1031"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (request.ExamType is < 1 or > 4)
        {
            var error = ErrorDescription.Error["E1033"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // set null for max grade with exam type
        switch (request.ExamType)
        {
            case 1:
                request.MaxPracticalGrade = null;
                if (request.MaxTheoryGrade == null)
                {
                    var error1 = ErrorDescription.Error["E1059"];
                    badRequest = BadRequest(CustomResponse.BadRequest(error1.Message, error1.Type));
                    return true;
                }

                break;
            case 2:
                request.MaxTheoryGrade = null;
                if (request.MaxPracticalGrade == null)
                {
                    var error2 = ErrorDescription.Error["E1060"];
                    badRequest = BadRequest(CustomResponse.BadRequest(error2.Message, error2.Type));
                    return true;
                }

                break;
            case 3:
                if (request.MaxTheoryGrade == null || request.MaxPracticalGrade == null)
                {
                    var error3 = ErrorDescription.Error["E1061"];
                    badRequest = BadRequest(CustomResponse.BadRequest(error3.Message, error3.Type));
                    return true;
                }

                break;
            case 4:
                request.MaxPracticalGrade = null;
                request.MaxTheoryGrade = null;
                break;
            default:
                var error = ErrorDescription.Error["E1045"];
            {
                badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                return true;
            }
        }

        // check max grade
        if (request.MaxTheoryGrade is < 1 || request.MaxPracticalGrade is < 1)
        {
            var error = ErrorDescription.Error["E1043"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        badRequest = null!;
        return false;
    }

    private bool CheckStringNameRequestCreate(CreateModuleRequest request, out IActionResult badRequest)
    {
        request.ModuleName = request.ModuleName.Trim();
        request.ModuleExamNamePortal = request.ModuleExamNamePortal.Trim();
        request.SemesterNamePortal = request.SemesterNamePortal.Trim();
        var listCourseCodes = request.CourseCode;

        // check empty string
        if (string.IsNullOrWhiteSpace(request.ModuleName) || string.IsNullOrWhiteSpace(request.ModuleExamNamePortal) ||
            string.IsNullOrWhiteSpace(request.SemesterNamePortal))
        {
            var error = ErrorDescription.Error["E1023"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // check course code
        foreach (var courseCode in listCourseCodes)
        {
            var newCourseCode = Regex.Replace(courseCode, StringConstant.RegexWhiteSpaces, " ");
            newCourseCode = newCourseCode.Replace(" ' ", "'").Trim();
            if (Regex.IsMatch(newCourseCode, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
            {
                var error = ErrorDescription.Error["E1026"];
                badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                return true;
            }

            if (newCourseCode.Length <= 100) continue;
            var error1 = ErrorDescription.Error["E1027"];
            badRequest = BadRequest(CustomResponse.BadRequest(error1.Message, error1.Type));
            return true;
        }

        //check module name
        request.ModuleName = Regex.Replace(request.ModuleName, StringConstant.RegexWhiteSpaces, " ");
        request.ModuleName = request.ModuleName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ModuleName, StringConstant.RegexSpecialCharacterNotAllowForModuleName))
        {
            var error = ErrorDescription.Error["E1024"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (request.ModuleName.Length > 255)
        {
            var error = ErrorDescription.Error["E1025"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // check exam name portal
        request.ModuleExamNamePortal =
            Regex.Replace(request.ModuleExamNamePortal, StringConstant.RegexWhiteSpaces, " ");
        request.ModuleExamNamePortal = request.ModuleExamNamePortal.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ModuleExamNamePortal, StringConstant.RegexSpecialCharacterNotAllowForModuleName))
        {
            var error = ErrorDescription.Error["E1028"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (request.ModuleExamNamePortal.Length > 255)
        {
            var error = ErrorDescription.Error["E1029"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // check semester name portal
        request.SemesterNamePortal = Regex.Replace(request.SemesterNamePortal, StringConstant.RegexWhiteSpaces, " ");
        request.SemesterNamePortal = request.SemesterNamePortal.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.SemesterNamePortal, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1034"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (request.SemesterNamePortal.Length > 255)
        {
            var error = ErrorDescription.Error["E1035"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        badRequest = null!;
        return false;
    }

    // update module by id
    [HttpPut]
    [Route("api/modules/{id:int}")]
    [Authorize(Roles = "admin")]
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

        var isChangeExamType = request.ExamType != module.ExamType;

        var canUpdate = CanDeleteUpdate(id);
        if (!canUpdate)
        {
            var error = ErrorDescription.Error["E1137"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.ModuleName = request.ModuleName.Trim();
        request.ModuleExamNamePortal = request.ModuleExamNamePortal.Trim();
        request.SemesterNamePortal = request.SemesterNamePortal.Trim();

        // is center id exist
        var center = _context.Centers.FirstOrDefault(c => c.Id == request.CenterId);
        if (center == null)
        {
            var error = ErrorDescription.Error["E1039"];
            return NotFound(CustomResponse.NotFound(error.Message));
        }

        if (CheckStringNameRequestUpdate(request, out var actionResult)) return actionResult;

        // check string name
        if (IsModuleNameWithDifferentIdExisted(request.ModuleName, id, request.CenterId))
        {
            var error = ErrorDescription.Error["E1054"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (CheckModuleTypeAndExamTypeRequestUpdate(request, out var badRequest)) return badRequest;

        if (CheckDayAndHourRequest(request.Days, request.Hours, out var badRequest1)) return badRequest1;

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
        catch (Exception)
        {
            var error = ErrorDescription.Error["E1042"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var updatedModule = GetCoursesModulesSemestersByModuleId(module.Id).FirstOrDefault();
        if (updatedModule == null)
        {
            var error = ErrorDescription.Error["E1044"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (isChangeExamType)
        {
            AddDataToGradeCategoryModule(module.Id, request.ExamType);
        }

        return Ok(CustomResponse.Ok("Module updated successfully", updatedModule));
    }

    // is module name with different id existed
    private bool IsModuleNameWithDifferentIdExisted(string moduleName, int id, int centerId)
    {
        return _context.Modules.Any(m =>
            m.Id != id && m.CenterId == centerId && string.Equals(m.ModuleName.ToLower(), moduleName.ToLower()));
    }

    private bool CheckModuleTypeAndExamTypeRequestUpdate(UpdateModuleRequest request, out IActionResult badRequest)
    {
        // module type and exam type
        if (request.ModuleType is < 1 or > 3)
        {
            var error = ErrorDescription.Error["E1031"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (request.ExamType is < 1 or > 4)
        {
            var error = ErrorDescription.Error["E1033"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // set null for max grade with exam type
        switch (request.ExamType)
        {
            case 1:
                request.MaxPracticalGrade = null;
                if (request.MaxTheoryGrade == null)
                {
                    var error1 = ErrorDescription.Error["E1059"];
                    badRequest = BadRequest(CustomResponse.BadRequest(error1.Message, error1.Type));
                    return true;
                }

                break;
            case 2:
                request.MaxTheoryGrade = null;
                if (request.MaxPracticalGrade == null)
                {
                    var error2 = ErrorDescription.Error["E1060"];
                    badRequest = BadRequest(CustomResponse.BadRequest(error2.Message, error2.Type));
                    return true;
                }

                break;
            case 3:
                if (request.MaxTheoryGrade == null || request.MaxPracticalGrade == null)
                {
                    var error3 = ErrorDescription.Error["E1061"];
                    badRequest = BadRequest(CustomResponse.BadRequest(error3.Message, error3.Type));
                    return true;
                }

                break;
            case 4:
                request.MaxPracticalGrade = null;
                request.MaxTheoryGrade = null;
                break;
            default:
                var error = ErrorDescription.Error["E1045"];
            {
                badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                return true;
            }
        }

        // check max grade
        if (request.MaxTheoryGrade is < 1 || request.MaxPracticalGrade is < 1)
        {
            var error = ErrorDescription.Error["E1043"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        badRequest = null!;
        return false;
    }

    private bool CheckStringNameRequestUpdate(UpdateModuleRequest request, out IActionResult actionResult)
    {
        request.ModuleName = request.ModuleName.Trim();
        request.ModuleExamNamePortal = request.ModuleExamNamePortal.Trim();
        request.SemesterNamePortal = request.SemesterNamePortal.Trim();

        //check empty
        if (string.IsNullOrWhiteSpace(request.ModuleName) || string.IsNullOrWhiteSpace(request.ModuleExamNamePortal) ||
            string.IsNullOrWhiteSpace(request.SemesterNamePortal))
        {
            var error = ErrorDescription.Error["E1023"];
            actionResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        //check module name
        request.ModuleName = Regex.Replace(request.ModuleName, StringConstant.RegexWhiteSpaces, " ");
        request.ModuleName = request.ModuleName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ModuleName, StringConstant.RegexSpecialCharacterNotAllowForModuleName))
        {
            var error = ErrorDescription.Error["E1024"];
            actionResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (request.ModuleName.Length > 255)
        {
            var error = ErrorDescription.Error["E1025"];
            actionResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // check exam name portal
        request.ModuleExamNamePortal =
            Regex.Replace(request.ModuleExamNamePortal, StringConstant.RegexWhiteSpaces, " ");
        request.ModuleExamNamePortal = request.ModuleExamNamePortal.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ModuleExamNamePortal, StringConstant.RegexSpecialCharacterNotAllowForModuleName))
        {
            var error = ErrorDescription.Error["E1028"];
            actionResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (request.ModuleExamNamePortal.Length > 255)
        {
            var error = ErrorDescription.Error["E1029"];
            actionResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // check semester name portal
        request.SemesterNamePortal = Regex.Replace(request.SemesterNamePortal, StringConstant.RegexWhiteSpaces, " ");
        request.SemesterNamePortal = request.SemesterNamePortal.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.SemesterNamePortal, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
        {
            var error = ErrorDescription.Error["E1034"];
            actionResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (request.SemesterNamePortal.Length > 255)
        {
            var error = ErrorDescription.Error["E1035"];
            actionResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        actionResult = null!;
        return false;
    }

    // search module by module name, course code, module type, exam type
    [HttpGet]
    [Route("api/modules/search")]
    [Authorize(Roles = "admin,sro")]
    public IActionResult SearchModule(string? moduleName, string? courseCode,
        string? moduleType, string? examType, string? semesterId)
    {
        var sModuleName = moduleName?.Trim() == null ? string.Empty : RemoveDiacritics(moduleName.Trim().ToUpper());
        var sCourseCode = courseCode?.Trim() == null ? string.Empty : RemoveDiacritics(courseCode.Trim().ToUpper());
        var sModuleType = moduleType?.Trim() == null ? null : moduleType.Trim();
        var sExamType = examType?.Trim() == null ? null : examType.Trim();
        var sSemesterId = semesterId?.Trim() == null ? null : semesterId.Trim();

        if (sModuleName == string.Empty && sCourseCode == string.Empty && string.IsNullOrEmpty(sModuleType) &&
            string.IsNullOrEmpty(sExamType) && string.IsNullOrEmpty(sSemesterId))
        {
            var modules = GetAllModules();
            return Ok(CustomResponse.Ok("Module searched successfully", modules));
        }

        var listCoursesModulesSemesters = GetAllModules();

        var moduleResponse = new List<ModuleResponse>();
        foreach (var m in listCoursesModulesSemesters)
        {
            var s1 = RemoveDiacritics(m.ModuleName!.ToUpper());
            var s2 = "";

            foreach (var cc in m.CourseCode!)
            {
                s2 += RemoveDiacritics(cc.ToUpper());
            }

            var s3 = m.ModuleType!.ToString()!;
            var s4 = m.ExamType!.ToString()!;
            var s5 = m.Semester.Id.ToString()!;

            sModuleType ??= "";

            sExamType ??= "";

            sSemesterId ??= "";

            if (s1.Contains(sModuleName) && s2.Contains(sCourseCode) && s3.Contains(sModuleType) &&
                s4.Contains(sExamType) && s5.Contains(sSemesterId))
            {
                moduleResponse.Add(m);
            }
        }

        return Ok(CustomResponse.Ok("Module searched successfully", moduleResponse));
    }

    // can delete module
    [HttpGet]
    [Route("api/modules/{id:int}/can-delete")]
    [Authorize(Roles = "admin")]
    public IActionResult CanDeleteModule(int id)
    {
        var module = _context.Modules.FirstOrDefault(m => m.Id == id);
        if (module == null)
        {
            return NotFound(CustomResponse.NotFound("Not Found Module"));
        }

        var canDelete = CanDeleteUpdate(id);

        return Ok(CustomResponse.Ok("Can delete module", new CheckModuleCanDeleteResponse()
        {
            CanDelete = canDelete
        }));
    }

    // delete module
    [HttpDelete]
    [Route("api/modules/{id:int}")]
    [Authorize(Roles = "admin")]
    public IActionResult DeleteModule(int id)
    {
        var module = _context.Modules
            .Include(m => m.CoursesModulesSemesters)
            .FirstOrDefault(m => m.Id == id);
        if (module == null)
        {
            return NotFound(CustomResponse.NotFound("Not Found Module"));
        }

        var canDelete = CanDeleteUpdate(id);
        if (!canDelete)
        {
            var error = ErrorDescription.Error["E1138"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // delete course module semester
        _context.CoursesModulesSemesters.RemoveRange(module.CoursesModulesSemesters);

        // delete grade item -> grade category module
        DeleteDataGradeCategoryModuleAndItems(id);

        // delete module
        _context.Modules.Remove(module);
        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            return BadRequest(CustomResponse.BadRequest(e.Message, e.GetType().ToString()));
        }

        return Ok(CustomResponse.Ok("Module deleted successfully", null!));
    }

    private bool CanDeleteUpdate(int id)
    {
        var selectModule = _context.Modules
            .Include(m => m.ClassSchedules)
            .Include(m => m.GpaRecords)
            .Include(m => m.CoursesModulesSemesters)
            .Include(m => m.GradeCategoryModule)
            .FirstOrDefault(m => m.Id == id);

        if (selectModule == null)
        {
            return false;
        }

        return !selectModule.ClassSchedules.Any() && !selectModule.GpaRecords.Any();
    }

    // func get all courses module semester
    // private IEnumerable<CourseModuleSemesterResponse> GetAllCoursesModulesSemesters()
    // {
    //     var courseModuleSemesters = _context.CoursesModulesSemesters
    //         .Include(cms => cms.Course)
    //         .Include(cms => cms.Module)
    //         .Include(cms => cms.Semester)
    //         .Include(cms => cms.Course.CourseFamily)
    //         .Include(cms => cms.Module.Center)
    //         .Include(cms => cms.Module.Center.Province)
    //         .Include(cms => cms.Module.Center.District)
    //         .Include(cms => cms.Module.Center.Ward)
    //         .Select(GetCourseModuleSemesterResponse)
    //         .ToList();
    //     return courseModuleSemesters;
    // }

    // func get all modules
    private IEnumerable<ModuleResponse> GetAllModules()
    {
        var modules = _context.Modules.Include(m => m.Center)
            .Include(m => m.Center.Province)
            .Include(m => m.Center.District)
            .Include(m => m.Center.Ward)
            .Include(m => m.CoursesModulesSemesters)
            .ThenInclude(cms => cms.Semester)
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
                CourseCode = m.CoursesModulesSemesters.Select(cms => cms.Course.Code).ToList(),
                Semester = new SemesterResponse()
                {
                    Id = m.CoursesModulesSemesters.Select(cms => cms.Semester.Id).FirstOrDefault(),
                    Name = m.CoursesModulesSemesters.Select(cms => cms.Semester.Name).FirstOrDefault()!
                }
            }).ToList();
        return modules;
    }

    private IEnumerable<CourseModuleSemesterResponse> GetCoursesModulesSemestersByModuleId(int id)
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
            .Where(cms => cms.ModuleId == id).AsEnumerable()
            .Select(GetCourseModuleSemesterResponse);
        return courseModuleSemesters;
    }

    // func get course_module_semester response
    private static CourseModuleSemesterResponse GetCourseModuleSemesterResponse(CourseModuleSemester cms)
    {
        var courseModuleSemester = new CourseModuleSemesterResponse()
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
        };
        return courseModuleSemester;
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
            CourseCode = module.CoursesModulesSemesters.Select(cms => cms.CourseCode).ToList(),
            Semester = new SemesterResponse()
            {
                Id = module.CoursesModulesSemesters.Select(cms => cms.Semester.Id).FirstOrDefault(),
                Name = module.CoursesModulesSemesters.Select(cms => cms.Semester.Name).FirstOrDefault()!
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

    private void AddDataToGradeCategoryModule(int moduleId, int examType)
    {
        DeleteDataGradeCategoryModuleAndItems(moduleId);

        var gradeItemNamePe = _context.GradeCategories.Find(PracticeExam)!.Name;
        var gradeItemNamePeResit = _context.GradeCategories.Find(PracticeExamResit)!.Name;

        var gradeCategoryModule = new GradeCategoryModule()
        {
            ModuleId = moduleId,
            GradeCategoryId = PracticeExam,
            TotalWeight = 100,
            QuantityGradeItem = 1,
            GradeItems = new List<GradeItem>()
            {
                new GradeItem()
                {
                    Name = gradeItemNamePe
                }
            }
        };

        var gradeCategoryModuleResit = new GradeCategoryModule()
        {
            ModuleId = moduleId,
            GradeCategoryId = PracticeExamResit,
            TotalWeight = 100,
            QuantityGradeItem = 1,
            GradeItems = new List<GradeItem>()
            {
                new GradeItem()
                {
                    Name = gradeItemNamePeResit
                }
            }
        };

        switch (examType)
        {
            // theory exam, theory exam weight not managed in this system -> 0%
            case 1:
                gradeCategoryModule.GradeCategoryId = TheoryExam;
                gradeCategoryModule.TotalWeight = 0;
                gradeCategoryModuleResit.GradeCategoryId = TheoryExamResit;
                gradeCategoryModuleResit.TotalWeight = 0;
                break;
            // practical exam
            case 2:
                break;
            // both theory and practical exam
            case 3:
                // add grade category for theory exam and theory exam resit
                var gradeCategoryModuleTe = new GradeCategoryModule()
                {
                    ModuleId = moduleId,
                    GradeCategoryId = TheoryExam,
                    TotalWeight = 0,
                    QuantityGradeItem = 1,
                    GradeItems = new List<GradeItem>()
                    {
                        new GradeItem()
                        {
                            Name = _context.GradeCategories.Find(TheoryExam)!.Name
                        }
                    }
                };
                var gradeCategoryModuleTeResit = new GradeCategoryModule()
                {
                    ModuleId = moduleId,
                    GradeCategoryId = TheoryExamResit,
                    TotalWeight = 0,
                    QuantityGradeItem = 1,
                    GradeItems = new List<GradeItem>()
                    {
                        new GradeItem()
                        {
                            Name = _context.GradeCategories.Find(TheoryExamResit)!.Name
                        }
                    }
                };
                gradeCategoryModule.TotalWeight = 100;
                gradeCategoryModuleResit.TotalWeight = 100;
                _context.GradeCategoryModules.Add(gradeCategoryModuleTe);
                _context.GradeCategoryModules.Add(gradeCategoryModuleTeResit);
                break;
            case 4: // no exam, add only final project
                var gradeCategoryFinal = new GradeCategoryModule()
                {
                    ModuleId = moduleId,
                    GradeCategoryId = FinalProject,
                    TotalWeight = 100,
                    QuantityGradeItem = 1,
                    GradeItems = new List<GradeItem>()
                    {
                        new GradeItem()
                        {
                            Name = _context.GradeCategories.Find(FinalProject)!.Name,
                        }
                    }
                };
                _context.GradeCategoryModules.Add(gradeCategoryFinal);
                _context.SaveChanges();
                return;
        }

        _context.GradeCategoryModules.Add(gradeCategoryModule);
        _context.GradeCategoryModules.Add(gradeCategoryModuleResit);

        _context.SaveChanges();
    }

    private void DeleteDataGradeCategoryModuleAndItems(int moduleId)
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