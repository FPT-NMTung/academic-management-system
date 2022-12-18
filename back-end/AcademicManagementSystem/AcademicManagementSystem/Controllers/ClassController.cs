using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.CenterController;
using AcademicManagementSystem.Models.ClassController;
using AcademicManagementSystem.Models.ClassDaysController;
using AcademicManagementSystem.Models.ClassStatusController;
using AcademicManagementSystem.Models.CourseController;
using AcademicManagementSystem.Models.CourseFamilyController;
using AcademicManagementSystem.Models.GenderController;
using AcademicManagementSystem.Models.ModuleController;
using AcademicManagementSystem.Models.RoleController;
using AcademicManagementSystem.Models.UserController.StudentController;
using AcademicManagementSystem.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class ClassController : ControllerBase
{
    private readonly AmsContext _context;
    private readonly User _user;
    private const int RoleIdStudent = 4;
    private const int NotScheduleYet = 5;
    private const int MaxNumberStudentInClass = 100;
    private const int StatusMerged = 6;
    private readonly IUserService _userService;

    public ClassController(AmsContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
        var userId = Convert.ToInt32(userService.GetUserId());
        _user = _context.Users.FirstOrDefault(u => u.Id == userId)!;
    }

    // get all classes
    [HttpGet]
    [Route("api/classes")]
    [Authorize(Roles = "sro")]
    public IActionResult GetClassesByCurrentSroCenter()
    {
        var classes = GetAllClassesInThisCenterByContext().ToList();
        return Ok(CustomResponse.Ok("Classes retrieved successfully", classes));
    }

    // get class by id
    [HttpGet]
    [Route("api/classes/{id:int}")]
    [Authorize(Roles = "sro")]
    public IActionResult GetClassById(int id)
    {
        var classResponse = GetAllClassesInThisCenterByContext().FirstOrDefault(c => c.Id == id);
        if (classResponse == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found in this center"));
        }

        return Ok(CustomResponse.Ok("Get class by id successfully", classResponse));
    }

    /*
     * sroName is firstName
     */
    [HttpGet]
    [Route("api/classes/search")]
    [Authorize(Roles = "sro")]
    public IActionResult SearchClasses([FromQuery] int? classDaysId, [FromQuery] int? classStatusId,
        [FromQuery] string? className, [FromQuery] string? courseFamilyCode, [FromQuery] string? sroName)
    {
        var sClassName = className == null ? string.Empty : RemoveDiacritics(className.Trim().ToLower());
        var sCourseFamilyCode = courseFamilyCode == null
            ? string.Empty
            : RemoveDiacritics(courseFamilyCode.Trim().ToLower());
        var sSroName = sroName == null ? string.Empty : RemoveDiacritics(sroName.Trim().ToLower());

        var allClasses = GetAllClassesInThisCenterByContext();

        //if user didn't input any search condition, return all classes
        if (classDaysId == null && classStatusId == null && sClassName == string.Empty
            && sCourseFamilyCode == string.Empty && sSroName == string.Empty)
        {
            return Ok(CustomResponse.Ok("Search classes successfully", allClasses));
        }

        var classesResponse = new List<ClassResponse>();
        foreach (var c in allClasses)
        {
            var s1 = RemoveDiacritics(c.Name!.ToLower());
            var s2 = RemoveDiacritics(c.CourseFamilyCode!.ToLower());
            var s3 = RemoveDiacritics(c.SroFirstName!.ToLower());
            var s4 = RemoveDiacritics(c.SroLastName!.ToLower());

            var fullName = s3 + " " + s4;

            if (s1.Contains(sClassName)
                && s2.Contains(sCourseFamilyCode)
                && fullName.Contains(sSroName)
                && (classDaysId == null || c.ClassDaysId == classDaysId)
                && (classStatusId == null || c.ClassStatusId == classStatusId))
            {
                classesResponse.Add(c);
            }
        }

        return Ok(CustomResponse.Ok("Search classes successfully", classesResponse));
    }

    // create new class
    [HttpPost]
    [Route("api/classes")]
    [Authorize(Roles = "sro")]
    public IActionResult CreateClass([FromBody] CreateClassRequest request)
    {
        var userId = Convert.ToInt32(_userService.GetUserId());
        var user = _context.Users.FirstOrDefault(u => u.Id == userId)!;

        request.Name = Regex.Replace(request.Name, StringConstant.RegexWhiteSpaces, " ").Trim();

        var courseFamily = _context.CourseFamilies.FirstOrDefault(cf =>
            cf.Code.Trim().ToLower() == request.CourseFamilyCode.Trim().ToLower());

        if (courseFamily == null)
        {
            var error = ErrorDescription.Error["E0076_1"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (courseFamily.PublishedYear > request.StartDate.Year)
        {
            var error = ErrorDescription.Error["E0076_2"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!courseFamily.IsActive)
        {
            var error = ErrorDescription.Error["E0076_3"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var errorCode = GetCodeIfOccuredErrorWhenCreate(request);

        if (errorCode != null)
        {
            var error = ErrorDescription.Error[errorCode];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var newClass = new Class()
        {
            CenterId = user.CenterId,
            CourseFamilyCode = request.CourseFamilyCode.Trim().ToUpper(),
            ClassDaysId = request.ClassDaysId,
            ClassStatusId = NotScheduleYet,
            SroId = user.Id,
            Name = request.Name,
            StartDate = request.StartDate,
            CompletionDate = request.CompletionDate,
            GraduationDate = request.GraduationDate,
            ClassHourStart = request.ClassHourStart,
            ClassHourEnd = request.ClassHourEnd,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Classes.Add(newClass);
        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0071"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }
        catch (Exception e)
        {
            return BadRequest(CustomResponse.BadRequest(e.Message, e.GetType().FullName!));
        }

        // get new class by id
        var classResponse = GetAllClassesInThisCenterByContext().FirstOrDefault(c => c.Id == newClass.Id);

        if (classResponse == null)
        {
            return BadRequest(CustomResponse.BadRequest("Cannot find created class", "error-not-found"));
        }

        return Ok(CustomResponse.Ok("Create class successfully", classResponse));
    }

    // update class
    [HttpPut]
    [Route("api/classes/{classId:int}")]
    [Authorize(Roles = "sro")]
    public IActionResult UpdateClass(int classId, [FromBody] UpdateClassRequest request)
    {
        request.Name = Regex.Replace(request.Name, StringConstant.RegexWhiteSpaces, " ").Trim();

        var classToUpdate = _context.Classes.FirstOrDefault(c => c.Id == classId && c.CenterId == _user.CenterId);

        if (classToUpdate == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found in this center"));
        }

        var courseFamily = _context.CourseFamilies.FirstOrDefault(cf =>
            cf.Code.Trim().ToLower() == request.CourseFamilyCode.Trim().ToLower());

        if (courseFamily == null)
        {
            var error = ErrorDescription.Error["E0076_1"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (courseFamily.PublishedYear > request.StartDate.Year)
        {
            var error = ErrorDescription.Error["E0076_2"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!courseFamily.IsActive)
        {
            var error = ErrorDescription.Error["E0076_3"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var errorCode = GetCodeIfOccuredErrorWhenUpdate(classId, request, classToUpdate.StartDate);

        if (errorCode != null)
        {
            var error = ErrorDescription.Error[errorCode];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        classToUpdate.CourseFamilyCode = request.CourseFamilyCode.Trim().ToUpper();
        classToUpdate.ClassDaysId = request.ClassDaysId;
        classToUpdate.ClassStatusId = request.ClassStatusId;
        classToUpdate.Name = request.Name;
        classToUpdate.StartDate = request.StartDate;
        classToUpdate.CompletionDate = request.CompletionDate;
        classToUpdate.GraduationDate = request.GraduationDate;
        classToUpdate.ClassHourStart = request.ClassHourStart;
        classToUpdate.ClassHourEnd = request.ClassHourEnd;
        classToUpdate.UpdatedAt = DateTime.Now;

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0071"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }
        catch (Exception e)
        {
            return BadRequest(CustomResponse.BadRequest(e.GetType().FullName!, e.Message));
        }

        // get updated class by id
        var classResponse = GetAllClassesInThisCenterByContext().FirstOrDefault(c => c.Id == classToUpdate.Id);

        if (classResponse == null)
        {
            return BadRequest(CustomResponse.BadRequest("Cannot find updated class", "error-not-found"));
        }

        return Ok(CustomResponse.Ok("Update class successfully", classResponse));
    }

    private string? GetCodeIfOccuredErrorWhenCreate(CreateClassRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return "E0068";
        }

        if (Regex.IsMatch(request.Name, StringConstant.RegexSpecialCharactersNotAllowForClassName))
        {
            return "E0069";
        }

        if (request.ClassHourEnd - request.ClassHourStart < TimeSpan.FromHours(1)
            || request.ClassHourEnd - request.ClassHourStart > TimeSpan.FromHours(4))
        {
            return "E0072";
        }

        if (request.ClassHourEnd > TimeSpan.FromHours(22) || request.ClassHourStart < TimeSpan.FromHours(8))
        {
            return "E0072_1";
        }

        if (request.StartDate.Date < DateTime.Now.Date)
        {
            return "E0073";
        }

        if (request.CompletionDate.Date <= request.StartDate.Date)
        {
            return "E0074";
        }

        if (request.GraduationDate.Date < request.CompletionDate.Date)
        {
            return "E0075";
        }

        return IsClassExist(request.Name, _user.CenterId, false, 0) ? "E0070" : null;
    }

    private string? GetCodeIfOccuredErrorWhenUpdate(int classId, UpdateClassRequest request, DateTime createdStartDate)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return "E0068";
        }

        // allow special characters: ()-_
        if (Regex.IsMatch(request.Name, StringConstant.RegexSpecialCharactersNotAllowForClassName))
        {
            return "E0069";
        }

        if (request.ClassHourEnd - request.ClassHourStart < TimeSpan.FromHours(1)
            || request.ClassHourEnd - request.ClassHourStart > TimeSpan.FromHours(4))
        {
            return "E0072";
        }

        if (request.ClassHourEnd > TimeSpan.FromHours(22) || request.ClassHourStart < TimeSpan.FromHours(8))
        {
            return "E0072_1";
        }

        if (request.StartDate.Date < createdStartDate.Date)
        {
            return "E0076";
        }

        if (request.CompletionDate.Date <= request.StartDate.Date)
        {
            return "E0074";
        }

        if (request.GraduationDate.Date < request.CompletionDate.Date)
        {
            return "E0075";
        }

        return IsClassExist(request.Name, _user.CenterId, true, classId) ? "E0070" : null;
    }

    private bool IsClassExist(string className, int centerId, bool isUpdate, int classId)
    {
        return isUpdate
            ? _context.Classes.Any(c =>
                c.Name.ToLower().Equals(className.ToLower()) && c.CenterId == centerId && c.Id != classId)
            : _context.Classes.Any(c => c.Name.ToLower().Equals(className.ToLower()) && c.CenterId == centerId);
    }

    private IQueryable<ClassResponse> GetAllClassesInThisCenterByContext()
    {
        var userId = Int32.Parse(_userService.GetUserId());
        var centerId = _context.Users.FirstOrDefault(u => u.Id == userId)!.CenterId;
        return _context.Classes.Include(c => c.Center)
            .Include(c => c.ClassDays)
            .Include(c => c.ClassStatus)
            .Include(c => c.Center.Province)
            .Include(c => c.Center.District)
            .Include(c => c.Center.Ward)
            .Include(c => c.CourseFamily)
            .Include(c => c.Sro)
            .ThenInclude(s => s.User)
            .Select(c => new ClassResponse()
            {
                Id = c.Id,
                Name = c.Name,
                CenterId = c.CenterId,
                CourseFamilyCode = c.CourseFamilyCode,
                ClassDaysId = c.ClassDaysId,
                ClassStatusId = c.ClassStatusId,
                StartDate = c.StartDate,
                CompletionDate = c.CompletionDate,
                GraduationDate = c.GraduationDate,
                ClassHourStart = c.ClassHourStart,
                ClassHourEnd = c.ClassHourEnd,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                Center = new CenterResponse()
                {
                    Id = c.Center.Id,
                    Name = c.Center.Name,
                    CreatedAt = c.Center.CreatedAt,
                    UpdatedAt = c.Center.UpdatedAt,
                    Province = new ProvinceResponse()
                    {
                        Id = c.Center.Province.Id,
                        Code = c.Center.Province.Code,
                        Name = c.Center.Province.Name,
                    },
                    District = new DistrictResponse()
                    {
                        Id = c.Center.District.Id,
                        Name = c.Center.District.Name,
                        Prefix = c.Center.District.Prefix
                    },
                    Ward = new WardResponse()
                    {
                        Id = c.Center.Ward.Id,
                        Name = c.Center.Ward.Name,
                        Prefix = c.Center.Ward.Prefix
                    }
                },
                CourseFamily = new CourseFamilyResponse()
                {
                    Code = c.CourseFamily.Code,
                    Name = c.CourseFamily.Name,
                    IsActive = c.CourseFamily.IsActive,
                    PublishedYear = c.CourseFamily.PublishedYear,
                    CreatedAt = c.CourseFamily.CreatedAt,
                    UpdatedAt = c.CourseFamily.UpdatedAt
                },
                ClassDays = new ClassDaysResponse()
                {
                    Id = c.ClassDays.Id,
                    Value = c.ClassDays.Value
                },
                ClassStatus = new ClassStatusResponse()
                {
                    Id = c.ClassStatus.Id,
                    Value = c.ClassStatus.Value
                },
                SroId = c.Sro.UserId,
                SroFirstName = c.Sro.User.FirstName,
                SroLastName = c.Sro.User.LastName
            }).Where(c => c.CenterId == centerId);
    }

    // import student from excel file
    [HttpPost]
    [Route("api/classes/{id:int}/students-from-excel")]
    [Authorize(Roles = "sro")]
    public ActionResult ImportStudentFromExcel(int id)
    {
        //is class exists
        var existedClassInCenter = _context.Classes
            .Include(c => c.Center)
            .Any(c => c.Id == id && c.CenterId == _user.CenterId);
        if (!existedClassInCenter)
        {
            var error = ErrorDescription.Error["E1073"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // is class have student
        var existedStudentInClass = _context.StudentsClasses.Any(sc => sc.ClassId == id);
        if (existedStudentInClass)
        {
            var error = ErrorDescription.Error["E1075"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var studentNo = 0;
        try
        {
            var file = Request.Form.Files[0];
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);

                    // get all rows with value
                    var rows = worksheet.RowsUsed();
                    foreach (var row in rows)
                    {
                        // skip the first and second row (number and column name)
                        if (row.RowNumber() == 1 || row.RowNumber() == 2) continue;
                        studentNo++;
                        //define
                        var enrollNumber = row.Cell(2).Value.ToString();
                        var firstName = row.Cell(3).Value.ToString();
                        var lastName = row.Cell(4).Value.ToString();
                        var statusDate = row.Cell(6).Value.ToString();
                        var gender = row.Cell(7).Value.ToString();
                        var birthday = row.Cell(8).Value.ToString();
                        var mobilePhone = row.Cell(9).Value.ToString();
                        var homePhone = row.Cell(10).Value.ToString() == "" ? null : row.Cell(11).Value.ToString();
                        var contactPhone = row.Cell(11).Value.ToString();
                        var email = row.Cell(12).Value.ToString();
                        var emailOrganization = row.Cell(13).Value.ToString();
                        var identityCardNo = row.Cell(14).Value.ToString();
                        var identityCardPublishedDate = row.Cell(15).Value.ToString();
                        var identityCardPublishedPlace = row.Cell(16).Value.ToString();
                        var contactAddress = row.Cell(17).Value.ToString();
                        var ward = row.Cell(18).Value.ToString();
                        var district = row.Cell(19).Value.ToString();
                        var province = row.Cell(20).Value.ToString();
                        var parentalName = row.Cell(21).Value.ToString();
                        var parentalRelative = row.Cell(22).Value.ToString();
                        var parentalPhone = row.Cell(23).Value.ToString();
                        var applicationDate = row.Cell(24).Value.ToString();
                        var applicationDocuments =
                            row.Cell(25).Value.ToString() == "" ? null : row.Cell(25).Value.ToString();
                        var courseCode = row.Cell(26).Value.ToString();
                        var highSchool = row.Cell(28).Value.ToString() == "" ? null : row.Cell(28).Value.ToString();
                        var university = row.Cell(29).Value.ToString() == "" ? null : row.Cell(29).Value.ToString();
                        var facebookUrl = row.Cell(30).Value.ToString() == "" ? null : row.Cell(30).Value.ToString();
                        var portfolio = row.Cell(31).Value.ToString() == "" ? null : row.Cell(31).Value.ToString();
                        var workingCompany = row.Cell(32).Value.ToString() == "" ? null : row.Cell(32).Value.ToString();
                        var companySalary = row.Cell(33).Value.ToString() == ""
                            ? 0
                            : Convert.ToInt32(row.Cell(33).Value.ToString());
                        var companyPosition =
                            row.Cell(34).Value.ToString() == "" ? null : row.Cell(34).Value.ToString();
                        var companyAddress = row.Cell(35).Value.ToString() == "" ? null : row.Cell(35).Value.ToString();
                        var feePlan = row.Cell(36).Value.ToString();
                        var promotion = row.Cell(37).Value.ToString();

                        // check if user existed => bad request
                        var existedUser = _context.Users.FirstOrDefault(u =>
                            u.Email == email || u.EmailOrganization == emailOrganization ||
                            u.MobilePhone == mobilePhone ||
                            u.CitizenIdentityCardNo == identityCardNo);
                        if (existedUser != null)
                        {
                            var error = ErrorDescription.Error["E1071"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + studentNo, error.Type));
                        }

                        // check if students exist => bad request
                        var existedStudent = _context.Students.FirstOrDefault(u =>
                            string.Equals(u.EnrollNumber.ToLower(), enrollNumber!.ToLower()));
                        if (existedStudent != null)
                        {
                            var error = ErrorDescription.Error["E1070"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + studentNo, error.Type));
                        }

                        if (province == null)
                        {
                            var error = ErrorDescription.Error["E1152"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + studentNo, error.Type));
                        }

                        if (district == null)
                        {
                            var error = ErrorDescription.Error["E1153"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + studentNo, error.Type));
                        }

                        if (ward == null)
                        {
                            var error = ErrorDescription.Error["E1154"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + studentNo, error.Type));
                        }

                        province = Regex.Replace(province, StringConstant.RegexWhiteSpaces, " ");
                        province = province.Replace(" ' ", "'").Trim();
                        district = Regex.Replace(district, StringConstant.RegexWhiteSpaces, " ");
                        district = district.Replace(" ' ", "'").Trim();
                        ward = Regex.Replace(ward, StringConstant.RegexWhiteSpaces, " ");
                        ward = ward.Replace(" ' ", "'").Trim();

                        // get list Province name
                        var listProvinceName = _context.Provinces.Select(p => p.Name).ToList();
                        foreach (var item in listProvinceName)
                        {
                            if (ConvertToUnsignedString(item).Equals(ConvertToUnsignedString(province)))
                            {
                                province = item;
                                break;
                            }
                        }

                        // get province id
                        var provinceId = _context.Provinces.FirstOrDefault(p => p.Name.Equals(province)) == null
                            ? 1
                            : _context.Provinces.FirstOrDefault(p => p.Name.Equals(province))!.Id;

                        // get list District name
                        var listDistrictName = _context.Districts
                            .Where(d => d.ProvinceId == provinceId)
                            .Select(d => d.Name).ToList();
                        foreach (var item in listDistrictName)
                        {
                            if (ConvertToUnsignedString(item).Equals(ConvertToUnsignedString(district)))
                            {
                                district = item;
                                break;
                            }
                        }

                        // get district id
                        var districtId = _context.Districts.FirstOrDefault(d => d.Name.Equals(district)) == null
                            ? 1
                            : _context.Districts.FirstOrDefault(d => d.Name.Equals(district))!.Id;

                        // get list Ward name
                        var listWardName = _context.Wards
                            .Where(w => w.DistrictId == districtId && w.ProvinceId == provinceId)
                            .Select(w => w.Name).ToList();

                        foreach (var item in listWardName)
                        {
                            if (ConvertToUnsignedString(item).Equals(ConvertToUnsignedString(ward)))
                            {
                                ward = item;
                                break;
                            }
                        }

                        // get ward id
                        var wardId = _context.Wards.FirstOrDefault(w => w.Name.Equals(ward)) == null
                            ? 1
                            : _context.Wards.FirstOrDefault(w => w.Name.Equals(ward))!.Id;

                        var newBirthday = DateTime.Parse(birthday ?? throw new InvalidOperationException());
                        var newIdentityCardPublishedDate =
                            DateTime.Parse(identityCardPublishedDate ?? throw new InvalidOperationException());
                        var newStatusDate = DateTime.Parse(statusDate ?? throw new InvalidOperationException());
                        var newApplicationDate =
                            DateTime.Parse(applicationDate ?? throw new InvalidOperationException());

                        var genderId = gender switch
                        {
                            "Male" => 1,
                            "Female" => 2,
                            "Not Known" => 3,
                            "Not Applicable" => 4,
                            _ => 4
                        };

                        // check input
                        // is course code exists
                        var course = _context.Courses.FirstOrDefault(c => c.Code == courseCode && c.IsActive);
                        if (course == null)
                        {
                            var error = ErrorDescription.Error["E1098"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + " at student no " + studentNo,
                                error.Type));
                        }

                        // is enroll number exists
                        var existedEnrollNumber =
                            _context.Students.Any(s =>
                                enrollNumber != null &&
                                string.Equals(s.EnrollNumber.ToLower(), enrollNumber.ToLower()));
                        if (existedEnrollNumber)
                        {
                            var error = ErrorDescription.Error["E1115"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + " at student no " + studentNo,
                                error.Type));
                        }

                        if (firstName != null)
                        {
                            firstName = Regex.Replace(firstName, StringConstant.RegexWhiteSpaces, " ");
                            // function replace string ex: H ' Hen Nie => H'Hen Nie
                            firstName = firstName.Replace(" ' ", "'").Trim();
                            if (Regex.IsMatch(firstName, StringConstant.RegexSpecialCharsNotAllowForPersonName)
                                || Regex.IsMatch(firstName, StringConstant.RegexDigits))
                            {
                                var error = ErrorDescription.Error["E1076"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }
                        }

                        if (lastName != null)
                        {
                            lastName = Regex.Replace(lastName, StringConstant.RegexWhiteSpaces, " ");
                            lastName = lastName.Replace(" ' ", "'").Trim();
                            if (Regex.IsMatch(lastName, StringConstant.RegexSpecialCharsNotAllowForPersonName) ||
                                Regex.IsMatch(lastName, StringConstant.RegexDigits))
                            {
                                var error = ErrorDescription.Error["E1077"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }
                        }

                        if (parentalName != null)
                        {
                            parentalName = Regex.Replace(parentalName, StringConstant.RegexWhiteSpaces, " ");
                            parentalName = parentalName.Replace(" ' ", "'").Trim();
                            if (Regex.IsMatch(parentalName, StringConstant.RegexSpecialCharsNotAllowForPersonName) ||
                                Regex.IsMatch(parentalName, StringConstant.RegexDigits))
                            {
                                var error = ErrorDescription.Error["E1099"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }
                        }

                        if (parentalRelative != null)
                        {
                            parentalRelative = Regex.Replace(parentalRelative, StringConstant.RegexWhiteSpaces, " ");
                            if (Regex.IsMatch(parentalRelative, StringConstant.RegexSpecialCharacter) ||
                                Regex.IsMatch(parentalRelative, StringConstant.RegexDigits))
                            {
                                var error = ErrorDescription.Error["E1100"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }
                        }

                        if (contactAddress != null)
                        {
                            contactAddress = Regex.Replace(contactAddress, StringConstant.RegexWhiteSpaces, " ");
                            contactAddress = contactAddress.Replace(" ' ", "'").Trim();
                            if (Regex.IsMatch(contactAddress, StringConstant.RegexSpecialCharacterForAddress))
                            {
                                var error = ErrorDescription.Error["E1101"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }
                        }

                        if (highSchool != null)
                        {
                            highSchool = Regex.Replace(highSchool, StringConstant.RegexWhiteSpaces, " ");
                            highSchool = highSchool.Replace(" ' ", "'").Trim();
                            if (Regex.IsMatch(highSchool, StringConstant.RegexSpecialCharacterForSchool))
                            {
                                var error = ErrorDescription.Error["E1103"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }
                        }

                        if (university != null)
                        {
                            university = Regex.Replace(university, StringConstant.RegexWhiteSpaces, " ");
                            university = university.Replace(" ' ", "'").Trim();
                            if (Regex.IsMatch(university, StringConstant.RegexSpecialCharacterForSchool))
                            {
                                var error = ErrorDescription.Error["E1104"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }
                        }

                        if (workingCompany != null)
                        {
                            workingCompany = Regex.Replace(workingCompany, StringConstant.RegexWhiteSpaces, " ");
                            workingCompany = workingCompany.Replace(" ' ", "'").Trim();
                            if (Regex.IsMatch(workingCompany,
                                    StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
                            {
                                var error = ErrorDescription.Error["E1105"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }
                        }

                        if (companyPosition != null)
                        {
                            companyPosition = Regex.Replace(companyPosition, StringConstant.RegexWhiteSpaces, " ");
                            if (Regex.IsMatch(companyPosition,
                                    StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
                            {
                                var error = ErrorDescription.Error["E1106"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }
                        }

                        if (companyAddress != null)
                        {
                            companyAddress = Regex.Replace(companyAddress, StringConstant.RegexWhiteSpaces, " ");
                            if (Regex.IsMatch(companyAddress, StringConstant.RegexSpecialCharacterForAddress))
                            {
                                var error = ErrorDescription.Error["E1107"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }
                        }

                        if (mobilePhone != null)
                        {
                            if (IsMobilePhoneExists(mobilePhone))
                            {
                                var error = ErrorDescription.Error["E1078"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }

                            if (!Regex.IsMatch(mobilePhone, StringConstant.RegexMobilePhone))
                            {
                                var error = ErrorDescription.Error["E1079"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }
                        }

                        if (contactPhone != null && !Regex.IsMatch(contactPhone, StringConstant.RegexMobilePhone))
                        {
                            var error = ErrorDescription.Error["E1079_1"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + " at student no " + studentNo,
                                error.Type));
                        }

                        if (homePhone != null && !Regex.IsMatch(homePhone, StringConstant.RegexMobilePhone))
                        {
                            var error = ErrorDescription.Error["E1079_2"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + " at student no " + studentNo,
                                error.Type));
                        }

                        if (parentalPhone != null && !Regex.IsMatch(parentalPhone, StringConstant.RegexMobilePhone))
                        {
                            var error = ErrorDescription.Error["E1079_3"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + " at student no " + studentNo,
                                error.Type));
                        }

                        if (email != null && emailOrganization != null)
                        {
                            if (IsEmailExists(email))
                            {
                                var error = ErrorDescription.Error["E1080"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }

                            if (IsEmailOrganizationExists(email))
                            {
                                var error = ErrorDescription.Error["E1081"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }

                            if (!Regex.IsMatch(email, StringConstant.RegexEmailCopilot))
                            {
                                var error = ErrorDescription.Error["E1096"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }

                            if (IsEmailOrganizationExists(emailOrganization))
                            {
                                var error = ErrorDescription.Error["E1082"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }

                            if (IsEmailExists(emailOrganization))
                            {
                                var error = ErrorDescription.Error["E1081_1"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }

                            if (!Regex.IsMatch(emailOrganization, StringConstant.RegexEmailCopilot))
                            {
                                var error = ErrorDescription.Error["E1097"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }

                            if (email == emailOrganization)
                            {
                                var error = ErrorDescription.Error["E1083"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }
                        }

                        if (identityCardNo != null)
                        {
                            if (IsCitizenIdentityCardNoExists(identityCardNo))
                            {
                                var error = ErrorDescription.Error["E1084"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }

                            if (!Regex.IsMatch(identityCardNo, StringConstant.RegexCitizenIdCardNo))
                            {
                                var error = ErrorDescription.Error["E1085"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }
                        }

                        if (identityCardPublishedPlace != null)
                        {
                            identityCardPublishedPlace = Regex.Replace(identityCardPublishedPlace,
                                StringConstant.RegexWhiteSpaces, " ");
                            identityCardPublishedPlace =
                                identityCardPublishedPlace.Replace(" ' ", "'").Trim();
                            if (Regex.IsMatch(identityCardPublishedPlace,
                                    StringConstant.RegexSpecialCharacterForAddress))
                            {
                                var error = ErrorDescription.Error["E1102"];
                                return BadRequest(CustomResponse.BadRequest(
                                    error.Message + " at student no " + studentNo,
                                    error.Type));
                            }
                        }

                        if (!IsAddressExists(provinceId, districtId, wardId))
                        {
                            var error = ErrorDescription.Error["E1091"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + " at student no " + studentNo,
                                error.Type));
                        }

                        if (!IsGenderExists(genderId))
                        {
                            var error = ErrorDescription.Error["E1092"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + " at student no " + studentNo,
                                error.Type));
                        }

                        if (Convert.ToInt32(feePlan) < 0)
                        {
                            var error = ErrorDescription.Error["E1108"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + " at student no " + studentNo,
                                error.Type));
                        }

                        if (Convert.ToInt32(promotion) < 0)
                        {
                            var error = ErrorDescription.Error["E1109"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + " at student no " + studentNo,
                                error.Type));
                        }

                        if (companySalary < 0)
                        {
                            var error = ErrorDescription.Error["E1110"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + " at student no " + studentNo,
                                error.Type));
                        }

                        if (newBirthday.Date >= DateTime.Now.Date)
                        {
                            var error = ErrorDescription.Error["E1128"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + " at student no " + studentNo,
                                error.Type));
                        }

                        if (newIdentityCardPublishedDate.Date >= DateTime.Now.Date)
                        {
                            var error = ErrorDescription.Error["E1136"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + " at student no " + studentNo,
                                error.Type));
                        }

                        var user = new User()
                        {
                            FirstName = firstName!.Trim(),
                            LastName = lastName!.Trim(),
                            MobilePhone = mobilePhone!.Trim(),
                            Email = email!.Trim(),
                            EmailOrganization = emailOrganization!.Trim(),
                            Birthday = newBirthday.Date,
                            ProvinceId = provinceId,
                            DistrictId = districtId,
                            WardId = wardId,
                            CitizenIdentityCardNo = identityCardNo!.Trim(),
                            CitizenIdentityCardPublishedDate = newIdentityCardPublishedDate.Date,
                            CitizenIdentityCardPublishedPlace = identityCardPublishedPlace!.Trim(),
                            RoleId = RoleIdStudent,
                            CenterId = _user.CenterId,
                            GenderId = genderId,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            IsActive = true,
                            Student = new Student()
                            {
                                EnrollNumber = enrollNumber!.Trim(),
                                CourseCode = courseCode!.Trim(),
                                Status = 1,
                                StatusDate = newStatusDate.Date,
                                HomePhone = homePhone,
                                ContactPhone = contactPhone!.Trim(),
                                ParentalName = parentalName!.Trim(),
                                ParentalRelationship = parentalRelative!.Trim(),
                                ContactAddress = contactAddress!.Trim(),
                                ParentalPhone = parentalPhone!.Trim(),
                                ApplicationDate = newApplicationDate.Date,
                                ApplicationDocument = applicationDocuments,
                                HighSchool = highSchool,
                                University = university,
                                FacebookUrl = facebookUrl,
                                PortfolioUrl = portfolio,
                                WorkingCompany = workingCompany,
                                CompanySalary = companySalary,
                                CompanyPosition = companyPosition,
                                CompanyAddress = companyAddress,
                                FeePlan = Convert.ToInt32(feePlan),
                                Promotion = Convert.ToInt32(promotion),
                                IsDraft = true,
                                StudentsClasses = new List<StudentClass>()
                                {
                                    new StudentClass()
                                    {
                                        ClassId = id,
                                        IsActive = true,
                                        CreatedAt = DateTime.Now,
                                        UpdatedAt = DateTime.Now,
                                    }
                                }
                            }
                        };
                        _context.Users.Add(user);
                        _context.Students.Add(user.Student);
                        _context.StudentsClasses.Add(user.Student.StudentsClasses.First());
                        if (studentNo == MaxNumberStudentInClass) break;
                    }

                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        var error = ErrorDescription.Error["E1069"];
                        return BadRequest(CustomResponse.BadRequest(error.Message + " at Student No " + studentNo,
                            error.Type));
                    }

                    return Ok(CustomResponse.Ok("Import students successfully", null!));
                }
            }
        }
        catch (Exception e)
        {
            return BadRequest(CustomResponse.BadRequest(e.Message + " at Student No " + studentNo,
                e.GetType().ToString()));
        }
    }

    // export students in class
    [HttpGet]
    [Route("api/classes/{id:int}/export-students")]
    [Authorize(Roles = "sro")]
    public IActionResult DownloadStudents(int id)
    {
        // get class by id
        var classInfo = GetAllClassesInThisCenterByContext().FirstOrDefault(c => c.Id == id);
        if (classInfo == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found in this center"));
        }

        var students = GetAllStudentsByClassId(id);

        if (students.Count == 0)
        {
            return NotFound(CustomResponse.NotFound("No student in this class"));
        }

        var fileName = $"{classInfo.Name}.xlsx";
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("students");

            // row 1
            worksheet.Cell(1, 1).Value = "1";
            worksheet.Cell(1, 2).Value = "2";
            worksheet.Cell(1, 3).Value = "3";
            worksheet.Cell(1, 4).Value = "4";
            worksheet.Cell(1, 5).Value = "5";
            worksheet.Cell(1, 6).Value = "6";
            worksheet.Cell(1, 7).Value = "7";
            worksheet.Cell(1, 8).Value = "8";
            worksheet.Cell(1, 9).Value = "9";
            worksheet.Cell(1, 10).Value = "10";
            worksheet.Cell(1, 11).Value = "11";
            worksheet.Cell(1, 12).Value = "12";
            worksheet.Cell(1, 13).Value = "13";
            worksheet.Cell(1, 14).Value = "14";
            worksheet.Cell(1, 15).Value = "15";
            worksheet.Cell(1, 16).Value = "16";
            worksheet.Cell(1, 17).Value = "17";
            worksheet.Cell(1, 18).Value = "18";
            worksheet.Cell(1, 19).Value = "19";
            worksheet.Cell(1, 20).Value = "20";
            worksheet.Cell(1, 21).Value = "21";
            worksheet.Cell(1, 22).Value = "22";
            worksheet.Cell(1, 23).Value = "23";
            worksheet.Cell(1, 24).Value = "24";
            worksheet.Cell(1, 25).Value = "25";
            worksheet.Cell(1, 26).Value = "26";
            worksheet.Cell(1, 27).Value = "27";
            worksheet.Cell(1, 28).Value = "28";
            worksheet.Cell(1, 29).Value = "29";
            worksheet.Cell(1, 30).Value = "30";
            worksheet.Cell(1, 31).Value = "31";
            worksheet.Cell(1, 32).Value = "32";
            worksheet.Cell(1, 33).Value = "33";
            worksheet.Cell(1, 34).Value = "34";
            worksheet.Cell(1, 35).Value = "35";
            worksheet.Cell(1, 36).Value = "36";
            worksheet.Cell(1, 37).Value = "37";
            worksheet.Cell(1, 38).Value = "38";

            // row 2
            worksheet.Row(2).Style.Fill.BackgroundColor = XLColor.FromHtml("#46E0DB");
            worksheet.Cell(2, 1).Value = "No.";
            worksheet.Cell(2, 2).Value = "Mã số sinh viên";
            worksheet.Cell(2, 3).Value = "Họ và tên đệm";
            worksheet.Cell(2, 4).Value = "Tên";
            worksheet.Cell(2, 5).Value = "Tên đầy đủ";
            worksheet.Cell(2, 6).Value = "Trạng thái";
            worksheet.Cell(2, 7).Value = "Ngày cập nhật trạng thái";
            worksheet.Cell(2, 8).Value = "Giới tính";
            worksheet.Cell(2, 9).Value = "Ngày sinh";
            worksheet.Cell(2, 10).Value = "Số điện thoại di động";
            worksheet.Cell(2, 11).Value = "Số điện thoại nhà";
            worksheet.Cell(2, 12).Value = "Số điện thoại liên hệ";
            worksheet.Cell(2, 13).Value = "Email";
            worksheet.Cell(2, 14).Value = "Email của trung tâm";
            worksheet.Cell(2, 15).Value = "CCCD/CMND";
            worksheet.Cell(2, 16).Value = "Ngày cấp CCCD/CMND";
            worksheet.Cell(2, 17).Value = "Nơi cấp CCCD/CMND";
            worksheet.Cell(2, 18).Value = "Địa chỉ liên hệ";
            worksheet.Cell(2, 19).Value = "Phường/Xã";
            worksheet.Cell(2, 20).Value = "Quận/Huyện";
            worksheet.Cell(2, 21).Value = "Tỉnh/Thành phố";
            worksheet.Cell(2, 22).Value = "Phụ huynh hoặc Người bảo trợ của học viên";
            worksheet.Cell(2, 23).Value = "Quan hệ với học viên";
            worksheet.Cell(2, 24).Value = "Số điện thoại phụ huynh hoặc người bảo trợ";
            worksheet.Cell(2, 25).Value = "Ngày đăng ký";
            worksheet.Cell(2, 26).Value = "Hồ sơ đăng ký";
            worksheet.Cell(2, 27).Value = "Mã khoá học";
            worksheet.Cell(2, 28).Value = "Mã chương trình học";
            worksheet.Cell(2, 29).Value = "Trường THPT";
            worksheet.Cell(2, 30).Value = "Trường đại học";
            worksheet.Cell(2, 31).Value = "Facebook";
            worksheet.Cell(2, 32).Value = "Portfolio";
            worksheet.Cell(2, 33).Value = "Công ty đang làm việc";
            worksheet.Cell(2, 34).Value = "Mức lương hiện tại";
            worksheet.Cell(2, 35).Value = "Vị trí trong công ty";
            worksheet.Cell(2, 36).Value = "Địa chỉ công ty";
            worksheet.Cell(2, 37).Value = "Kế hoạch học phí (Fee plan)";
            worksheet.Cell(2, 38).Value = "Học bổng";

            for (var i = 3; i < students.Count + 3; i++)
            {
                worksheet.Cell(i, 1).Value = i - 2;
                worksheet.Cell(i, 2).Value = students[i - 3].EnrollNumber;
                worksheet.Cell(i, 3).Value = students[i - 3].FirstName;
                worksheet.Cell(i, 4).Value = students[i - 3].LastName;
                var fullName = $"{students[i - 3].FirstName} {students[i - 3].LastName}";
                worksheet.Cell(i, 5).Value = fullName;
                var learningStatus = students[i - 3].Status switch
                {
                    1 => "Studying",
                    2 => "Delay",
                    3 => "Dropout",
                    4 => "ClassQueue",
                    5 => "Transfer",
                    6 => "Upgrade",
                    7 => "Finished",
                    _ => "Error"
                };
                worksheet.Cell(i, 6).Value = learningStatus;
                worksheet.Cell(i, 7).Value = students[i - 3].StatusDate;
                worksheet.Cell(i, 8).Value = students[i - 3].Gender?.Value;
                worksheet.Cell(i, 9).Value = students[i - 3].Birthday;

                worksheet.Cell(i, 10).Style.NumberFormat.Format = "@";
                worksheet.Cell(i, 10).Value = students[i - 3].MobilePhone;
                worksheet.Cell(i, 11).Style.NumberFormat.Format = "@";
                worksheet.Cell(i, 11).Value = students[i - 3].HomePhone;
                worksheet.Cell(i, 12).Style.NumberFormat.Format = "@";
                worksheet.Cell(i, 12).Value = students[i - 3].ContactPhone;

                worksheet.Cell(i, 13).Value = students[i - 3].Email;
                worksheet.Cell(i, 14).Value = students[i - 3].EmailOrganization;

                worksheet.Cell(i, 15).Style.NumberFormat.Format = "@";
                worksheet.Cell(i, 15).Value = students[i - 3].CitizenIdentityCardNo;

                worksheet.Cell(i, 16).Value = students[i - 3].CitizenIdentityCardPublishedDate;
                worksheet.Cell(i, 17).Value = students[i - 3].CitizenIdentityCardPublishedPlace;
                worksheet.Cell(i, 18).Value = students[i - 3].ContactAddress;
                worksheet.Cell(i, 19).Value = students[i - 3].Ward?.Name;
                worksheet.Cell(i, 20).Value = students[i - 3].District?.Name;
                worksheet.Cell(i, 21).Value = students[i - 3].Province?.Name;
                worksheet.Cell(i, 22).Value = students[i - 3].ParentalName;
                worksheet.Cell(i, 23).Value = students[i - 3].ParentalRelationship;

                worksheet.Cell(i, 24).Style.NumberFormat.Format = "@";
                worksheet.Cell(i, 24).Value = students[i - 3].ParentalPhone;

                worksheet.Cell(i, 25).Value = students[i - 3].ApplicationDate;
                worksheet.Cell(i, 26).Value = students[i - 3].ApplicationDocument;
                worksheet.Cell(i, 27).Value = students[i - 3].CourseCode;
                worksheet.Cell(i, 28).Value = students[i - 3].Course?.CourseFamily?.Code;
                worksheet.Cell(i, 29).Value = students[i - 3].HighSchool;
                worksheet.Cell(i, 30).Value = students[i - 3].University;
                worksheet.Cell(i, 31).Value = students[i - 3].FacebookUrl;
                worksheet.Cell(i, 32).Value = students[i - 3].PortfolioUrl;
                worksheet.Cell(i, 33).Value = students[i - 3].WorkingCompany;
                worksheet.Cell(i, 34).Value = students[i - 3].CompanySalary;
                worksheet.Cell(i, 35).Value = students[i - 3].CompanyPosition;
                worksheet.Cell(i, 36).Value = students[i - 3].CompanyAddress;
                worksheet.Cell(i, 37).Value = students[i - 3].FeePlan;
                worksheet.Cell(i, 38).Value = students[i - 3].Promotion;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }

    // add new student to class
    [HttpPost]
    [Route("api/classes/{id:int}/students")]
    [Authorize(Roles = "sro")]
    public IActionResult AddStudentToClass(int id, [FromBody] AddStudentToClassRequest request)
    {
        //is class exists
        var userId = Int32.Parse(_userService.GetUserId());
        var centerId = _context.Users.FirstOrDefault(u => u.Id == userId)!.CenterId;
        var existedClassInCenter = _context.Classes
            .Include(c => c.Center)
            .Any(c => c.Id == id && c.CenterId == centerId);
        if (!existedClassInCenter)
        {
            var error = ErrorDescription.Error["E1073"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // get number of student in a class
        var numberOfStudentInClass = _context.StudentsClasses
            .Include(sc => sc.Class)
            .Include(sc => sc.Student)
            .Count(sc => sc.ClassId == id && sc.IsActive);
        if (numberOfStudentInClass >= MaxNumberStudentInClass)
        {
            var error = ErrorDescription.Error["E1116"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.FirstName = request.FirstName.Trim();
        request.LastName = request.LastName.Trim();
        request.Email = request.Email.Trim();
        request.EmailOrganization = request.EmailOrganization.Trim();
        request.CourseCode = request.CourseCode.ToUpper().Trim();
        request.ParentalName = request.ParentalName.Trim();
        request.ParentalRelationship = request.ParentalRelationship.Trim();
        request.ContactAddress = request.ContactAddress.Trim();
        request.CitizenIdentityCardPublishedPlace = request.CitizenIdentityCardPublishedPlace.Trim();
        request.EnrollNumber = request.EnrollNumber.Trim();
        request.CitizenIdentityCardNo = request.CitizenIdentityCardNo.Trim();

        if (string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName) ||
            string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.EmailOrganization) ||
            string.IsNullOrEmpty(request.CourseCode) || string.IsNullOrEmpty(request.ParentalName) ||
            string.IsNullOrEmpty(request.ParentalRelationship) || string.IsNullOrEmpty(request.ContactAddress) ||
            string.IsNullOrEmpty(request.CitizenIdentityCardPublishedPlace) ||
            string.IsNullOrEmpty(request.EnrollNumber))
        {
            var error = ErrorDescription.Error["E1093"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // is course code exists
        var course = _context.Courses.FirstOrDefault(c => c.Code == request.CourseCode && c.IsActive);
        if (course == null)
        {
            var error = ErrorDescription.Error["E1098"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // is enroll number exists
        var existedEnrollNumber =
            _context.Students.Any(s => string.Equals(s.EnrollNumber.ToLower(), request.EnrollNumber.ToLower()));
        if (existedEnrollNumber)
        {
            var error = ErrorDescription.Error["E1115"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.FirstName = Regex.Replace(request.FirstName, StringConstant.RegexWhiteSpaces, " ");
        // function replace string ex: H ' Hen Nie => H'Hen Nie
        request.FirstName = request.FirstName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.FirstName, StringConstant.RegexSpecialCharsNotAllowForPersonName)
            || Regex.IsMatch(request.FirstName, StringConstant.RegexDigits))
        {
            var error = ErrorDescription.Error["E1076"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.LastName = Regex.Replace(request.LastName, StringConstant.RegexWhiteSpaces, " ");
        request.LastName = request.LastName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.LastName, StringConstant.RegexSpecialCharsNotAllowForPersonName) ||
            Regex.IsMatch(request.LastName, StringConstant.RegexDigits))
        {
            var error = ErrorDescription.Error["E1077"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.ParentalName = Regex.Replace(request.ParentalName, StringConstant.RegexWhiteSpaces, " ");
        request.ParentalName = request.ParentalName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ParentalName, StringConstant.RegexSpecialCharsNotAllowForPersonName) ||
            Regex.IsMatch(request.ParentalName, StringConstant.RegexDigits))
        {
            var error = ErrorDescription.Error["E1099"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.ParentalRelationship =
            Regex.Replace(request.ParentalRelationship, StringConstant.RegexWhiteSpaces, " ");
        if (Regex.IsMatch(request.ParentalRelationship, StringConstant.RegexSpecialCharacter) ||
            Regex.IsMatch(request.ParentalRelationship, StringConstant.RegexDigits))
        {
            var error = ErrorDescription.Error["E1100"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.ContactAddress = Regex.Replace(request.ContactAddress, StringConstant.RegexWhiteSpaces, " ");
        request.ContactAddress = request.ContactAddress.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.ContactAddress, StringConstant.RegexSpecialCharacterForAddress))
        {
            var error = ErrorDescription.Error["E1101"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.HighSchool != null)
        {
            request.HighSchool = Regex.Replace(request.HighSchool, StringConstant.RegexWhiteSpaces, " ");
            request.HighSchool = request.HighSchool.Replace(" ' ", "'").Trim();
            if (Regex.IsMatch(request.HighSchool, StringConstant.RegexSpecialCharacterForSchool))
            {
                var error = ErrorDescription.Error["E1103"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }
        }

        if (request.University != null)
        {
            request.University = Regex.Replace(request.University, StringConstant.RegexWhiteSpaces, " ");
            request.University = request.University.Replace(" ' ", "'").Trim();
            if (Regex.IsMatch(request.University, StringConstant.RegexSpecialCharacterForSchool))
            {
                var error = ErrorDescription.Error["E1104"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }
        }

        if (request.WorkingCompany != null)
        {
            request.WorkingCompany = Regex.Replace(request.WorkingCompany, StringConstant.RegexWhiteSpaces, " ");
            request.WorkingCompany = request.WorkingCompany.Replace(" ' ", "'").Trim();
            if (Regex.IsMatch(request.WorkingCompany, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
            {
                var error = ErrorDescription.Error["E1105"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }
        }

        if (request.CompanyPosition != null)
        {
            request.CompanyPosition = Regex.Replace(request.CompanyPosition, StringConstant.RegexWhiteSpaces, " ");
            if (Regex.IsMatch(request.CompanyPosition, StringConstant.RegexSpecialCharacterWithDashUnderscoreSpaces))
            {
                var error = ErrorDescription.Error["E1106"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }
        }

        if (request.CompanyAddress != null)
        {
            request.CompanyAddress = Regex.Replace(request.CompanyAddress, StringConstant.RegexWhiteSpaces, " ");
            if (Regex.IsMatch(request.CompanyAddress, StringConstant.RegexSpecialCharacterForAddress))
            {
                var error = ErrorDescription.Error["E1107"];
                return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            }
        }

        if (IsMobilePhoneExists(request.MobilePhone))
        {
            var error = ErrorDescription.Error["E1078"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.MobilePhone, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E1079"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.ContactPhone, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E1079_1"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.HomePhone != null && !Regex.IsMatch(request.HomePhone, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E1079_2"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.ParentalPhone, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E1079_3"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailExists(request.Email))
        {
            var error = ErrorDescription.Error["E1080"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailOrganizationExists(request.Email))
        {
            var error = ErrorDescription.Error["E1081"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.Email, StringConstant.RegexEmailCopilot))
        {
            var error = ErrorDescription.Error["E1096"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailOrganizationExists(request.EmailOrganization))
        {
            var error = ErrorDescription.Error["E1082"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailExists(request.EmailOrganization))
        {
            var error = ErrorDescription.Error["E1081_1"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.EmailOrganization, StringConstant.RegexEmailCopilot))
        {
            var error = ErrorDescription.Error["E1097"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Email == request.EmailOrganization)
        {
            var error = ErrorDescription.Error["E1083"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsCitizenIdentityCardNoExists(request.CitizenIdentityCardNo))
        {
            var error = ErrorDescription.Error["E1084"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.CitizenIdentityCardPublishedPlace = Regex.Replace(request.CitizenIdentityCardPublishedPlace,
            StringConstant.RegexWhiteSpaces, " ");
        request.CitizenIdentityCardPublishedPlace =
            request.CitizenIdentityCardPublishedPlace.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.CitizenIdentityCardPublishedPlace, StringConstant.RegexSpecialCharacterForAddress))
        {
            var error = ErrorDescription.Error["E1102"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.CitizenIdentityCardNo, StringConstant.RegexCitizenIdCardNo))
        {
            var error = ErrorDescription.Error["E1085"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!IsProvinceExists(request.ProvinceId))
        {
            var error = ErrorDescription.Error["E1088"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!IsDistrictExists(request.DistrictId))
        {
            var error = ErrorDescription.Error["E1089"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!IsWardExists(request.WardId))
        {
            var error = ErrorDescription.Error["E1090"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!IsAddressExists(request.ProvinceId, request.DistrictId, request.WardId))
        {
            var error = ErrorDescription.Error["E1091"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!IsGenderExists(request.GenderId))
        {
            var error = ErrorDescription.Error["E1092"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.FeePlan < 0)
        {
            var error = ErrorDescription.Error["E1108"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Promotion < 0)
        {
            var error = ErrorDescription.Error["E1109"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.CompanySalary is < 0)
        {
            var error = ErrorDescription.Error["E1110"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Birthday.Date >= DateTime.Now.Date)
        {
            var error = ErrorDescription.Error["E1128"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.CitizenIdentityCardPublishedDate.Date >= DateTime.Now.Date)
        {
            var error = ErrorDescription.Error["E1136"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // create user from request
        var user = new User()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            MobilePhone = request.MobilePhone,
            Email = request.Email,
            EmailOrganization = request.EmailOrganization,
            Birthday = request.Birthday.Date,
            ProvinceId = request.ProvinceId,
            DistrictId = request.DistrictId,
            WardId = request.WardId,
            CitizenIdentityCardNo = request.CitizenIdentityCardNo,
            CitizenIdentityCardPublishedDate = request.CitizenIdentityCardPublishedDate,
            CitizenIdentityCardPublishedPlace = request.CitizenIdentityCardPublishedPlace,
            RoleId = RoleIdStudent,
            CenterId = centerId,
            GenderId = request.GenderId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = true,
            Student = new Student()
            {
                EnrollNumber = request.EnrollNumber,
                CourseCode = request.CourseCode,
                Status = 1,
                StatusDate = DateTime.Now.Date,
                HomePhone = request.HomePhone,
                ContactPhone = request.ContactPhone,
                ParentalName = request.ParentalName,
                ParentalRelationship = request.ParentalRelationship,
                ContactAddress = request.ContactAddress,
                ParentalPhone = request.ParentalPhone,
                ApplicationDate = request.ApplicationDate,
                ApplicationDocument = request.ApplicationDocument,
                HighSchool = request.HighSchool,
                University = request.University,
                FacebookUrl = request.FacebookUrl,
                PortfolioUrl = request.PortfolioUrl,
                WorkingCompany = request.WorkingCompany,
                CompanySalary = request.CompanySalary,
                CompanyPosition = request.CompanyPosition,
                CompanyAddress = request.CompanyAddress,
                FeePlan = request.FeePlan,
                Promotion = request.Promotion,
                IsDraft = true,
                StudentsClasses = new List<StudentClass>()
                {
                    new StudentClass()
                    {
                        ClassId = id,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    }
                }
            }
        };
        _context.Users.Add(user);
        _context.Students.Add(user.Student);
        _context.StudentsClasses.Add(user.Student.StudentsClasses.First());

        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1113"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Add student to class successfully", null!));
    }

    // save student from draft
    [HttpPatch]
    [Route("api/classes/{id:int}/students")]
    [Authorize(Roles = "sro")]
    public IActionResult SaveStudentsToClassFromDraft(int id)
    {
        var existedClass = _context.Classes.Any(c => c.Id == id);
        if (!existedClass)
        {
            var error = ErrorDescription.Error["E1073"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var students = _context.Students
            .Include(s => s.StudentsClasses)
            .Where(s => s.StudentsClasses.Any(sc => sc.ClassId == id))
            .ToList();
        foreach (var student in students)
        {
            student.IsDraft = false;
            _context.Students.Update(student);
        }

        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1072"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Save students to class successfully", null!));
    }

    // delete draft student
    [HttpDelete]
    [Route("api/classes/{id:int}/students-draft")]
    [Authorize(Roles = "sro")]
    public IActionResult DeleteDraftStudents(int id)
    {
        var existedClass = _context.Classes.Any(c => c.Id == id);
        if (!existedClass)
        {
            var error = ErrorDescription.Error["E1073"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var users = _context.Users
            .Include(u => u.Student)
            .Include(u => u.Student.StudentsClasses)
            .Where(u => u.Student.StudentsClasses.Any(sc => sc.ClassId == id) && u.Student.IsDraft)
            .ToList();

        if (users.Count == 0)
        {
            return Ok(CustomResponse.Ok("No student in class", null!));
        }

        foreach (var user in users)
        {
            _context.StudentsClasses.Remove(
                user.Student.StudentsClasses.First(sc => sc.StudentId == user.Student.UserId));
            _context.Students.Remove(user.Student);
            _context.Users.Remove(user);
        }

        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1074"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Clear draft students successfully", null!));
    }

    // get students in class
    [HttpGet]
    [Route("api/classes/{id:int}/students")]
    [Authorize(Roles = "sro, teacher")]
    public IActionResult GetStudentsInClass(int id)
    {
        // is class exist
        var existedClass = _context.Classes.Any(c => c.Id == id);
        if (!existedClass)
        {
            var error = ErrorDescription.Error["E1073"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var students = GetAllStudentsByClassId(id);
        return Ok(CustomResponse.Ok("Get students in class successfully", students));
    }

    // get undraft students in class
    [HttpGet]
    [Route("api/classes/{id:int}/un-draft-students")]
    [Authorize(Roles = "teacher")]
    public IActionResult GetUnDraftStudentsInClass(int id)
    {
        // is class exist
        var existedClass = _context.Classes.Any(c => c.Id == id);
        if (!existedClass)
        {
            var error = ErrorDescription.Error["E1073"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var students = GetUnDraftStudentsByClassId(id);
        return Ok(CustomResponse.Ok("Get students in class successfully", students));
    }

    // get status list module of class
    [HttpGet]
    [Route("api/classes/{id:int}/modules")]
    [Authorize(Roles = "sro")]
    public IActionResult GetStatusListModuleOfClass(int id)
    {
        // is class exist
        var existedClass = _context.Classes.Find(id);
        if (existedClass == null)
        {
            var error = ErrorDescription.Error["E1073"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var listModule = _context.Modules
            .Include(m => m.ClassSchedules)
            .ThenInclude(cs => cs.Class)
            .Include(m => m.CoursesModulesSemesters)
            .ThenInclude(cms => cms.Course)
            .Where(m =>
                m.CoursesModulesSemesters.First().Course.CourseFamilyCode == existedClass.CourseFamilyCode &&
                existedClass.CenterId == m.CenterId);

        var list = new List<ModuleStatusResponse>();

        listModule.ToList().ForEach(m =>
        {
            var findSchedule = m.ClassSchedules.Where(cs => cs.ClassId == id && cs.ModuleId == m.Id);
            var moduleStatus = new ModuleStatusResponse()
            {
                Module = new ModuleResponse()
                {
                    Id = m.Id,
                    ModuleName = m.ModuleName,
                    ModuleType = m.ModuleType,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                },
                ScheduleId = findSchedule.ToList().Count > 0 ? m.ClassSchedules.First().Id : 0,
                ScheduleStartTime = findSchedule.ToList().Count > 0 ? m.ClassSchedules.First().StartDate : null,
                Status = findSchedule.ToList().Count > 0,
            };
            list.Add(moduleStatus);
        });

        return Ok(CustomResponse.Ok("Get status list module of class successfully", list));
    }

    // delete all students in class
    // [NOTE] this method is only used for testing
    [HttpDelete]
    [Route("api/classes/{id:int}/students")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult DeleteAllStudents(int id)
    {
        var existedClass = _context.Classes.Any(c => c.Id == id);
        if (!existedClass)
        {
            var error = ErrorDescription.Error["E1073"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var users = _context.Users
            .Include(u => u.Student)
            .Include(u => u.Student.StudentsClasses)
            .Where(u => u.Student.StudentsClasses.Any(sc => sc.ClassId == id))
            .ToList();

        if (users.Count == 0)
        {
            return Ok(CustomResponse.Ok("No student in class", null!));
        }


        foreach (var user in users)
        {
            _context.StudentsClasses.Remove(
                user.Student.StudentsClasses.First(sc => sc.StudentId == user.Student.UserId));
            _context.Students.Remove(user.Student);
            _context.Users.Remove(user);
        }

        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1074"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Delete students successfully", null!));
    }

    [HttpGet]
    [Route("api/classes/{id:int}/number-of-students")]
    [Authorize(Roles = "sro")]
    public IActionResult GetNumberOfStudentsInClass(int id)
    {
        var existedClass = _context.Classes
            .Include(c => c.Center)
            .Any(c => c.Id == id && c.CenterId == _user.CenterId);
        if (!existedClass)
        {
            var error = ErrorDescription.Error["E1073"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var numberOfStudents = _context.StudentsClasses.Count(sc => sc.ClassId == id && sc.IsActive);
        return Ok(CustomResponse.Ok("Get number of students successfully", numberOfStudents));
    }

    // get list class can merge
    [HttpGet]
    [Route("api/classes/{id:int}/available-to-merge")]
    [Authorize(Roles = "sro")]
    public IActionResult GetAvailableClassesToMerge(int id)
    {
        var currentClass = GetAllClassesInThisCenterByContext()
            .FirstOrDefault(c => c.Id == id);

        if (currentClass == null)
        {
            return NotFound(CustomResponse.NotFound("Not Found Class with id: " + id + " in this center"));
        }

        var availableClasses = GetAvailableClasses(currentClass);
        return Ok(CustomResponse.Ok("Get available classes for student successfully", availableClasses));
    }

    // merge 1 class in to this class
    [HttpPut]
    [Route("api/classes/{id:int}/merge")]
    [Authorize(Roles = "sro")]
    public IActionResult MergeClass(int id, [FromBody] MergeClassRequest request)
    {
        var firstClass = _context.Classes
            .Include(c => c.Center)
            .Any(c => c.Id == id && c.CenterId == _user.CenterId);
        if (!firstClass)
        {
            var error = ErrorDescription.Error["E1130"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var secondClass = _context.Classes
            .Include(c => c.Center)
            .Any(c => c.Id == request.ClassId && c.CenterId == _user.CenterId);
        if (!secondClass)
        {
            var error = ErrorDescription.Error["E1131"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // get list student in first class
        var listStudentsInFirstClass = _context.StudentsClasses
            .Include(sc => sc.Student)
            .Where(sc => sc.ClassId == id && sc.IsActive)
            .ToList();

        // get list student in second class
        var listStudentsInSecondClass = _context.StudentsClasses
            .Include(sc => sc.Student)
            .Where(sc => sc.ClassId == request.ClassId && sc.IsActive)
            .ToList();

        if (listStudentsInFirstClass.Count == 0)
        {
            var error = ErrorDescription.Error["E1134"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (listStudentsInSecondClass.Count == 0)
        {
            var error = ErrorDescription.Error["E1135"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (listStudentsInFirstClass.Count + listStudentsInSecondClass.Count > MaxNumberStudentInClass)
        {
            var error = ErrorDescription.Error["E1133"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        foreach (var student in listStudentsInFirstClass)
        {
            // if student in second class is not in student class table, add it and change is_active first class to false
            if (!_context.StudentsClasses.Any(sc => sc.StudentId == student.StudentId && sc.ClassId == request.ClassId))
            {
                _context.StudentsClasses.Add(new StudentClass()
                {
                    StudentId = student.StudentId,
                    ClassId = request.ClassId,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
                student.IsActive = false;
                student.UpdatedAt = DateTime.Now;
            }
            else
            {
                // if student in second class is in student class table, change is_active first class to false
                var studentClass = _context.StudentsClasses
                    .FirstOrDefault(sc => sc.StudentId == student.StudentId && sc.ClassId == request.ClassId);
                if (studentClass != null)
                {
                    studentClass.IsActive = true;
                    studentClass.UpdatedAt = DateTime.Now;
                }

                student.IsActive = false;
                student.UpdatedAt = DateTime.Now;
            }
        }

        var firstClassChange = _context.Classes.FirstOrDefault(c => c.Id == id);
        if (firstClassChange != null)
        {
            firstClassChange.ClassStatusId = StatusMerged;
            firstClassChange.UpdatedAt = DateTime.Now;
        }

        var secondClassChange = _context.Classes.FirstOrDefault(c => c.Id == request.ClassId);
        if (secondClassChange != null)
        {
            secondClassChange.UpdatedAt = DateTime.Now;
        }

        // delete session and delete class schedule of first class
        var listSessionOfFirstClass = _context.Sessions
            .Include(s => s.ClassSchedule)
            .Where(s => s.ClassSchedule.ClassId == id && s.ClassSchedule.StartDate.Date > DateTime.Now.Date)
            .ToList();
        _context.Sessions.RemoveRange(listSessionOfFirstClass);
        _context.ClassSchedules.RemoveRange(listSessionOfFirstClass.Select(s => s.ClassSchedule));

        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1132"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Merge class successfully", null!));
    }

    // merge class 2
    [HttpPut]
    [Route("api/classes/merge")]
    [Authorize(Roles = "sro")]
    public IActionResult MergeClass2([FromBody] MergeClassRequest2 request)
    {
        var userId = Int32.Parse(_userService.GetUserId());
        var centerId = _context.Users.FirstOrDefault(u => u.Id == userId)!.CenterId;
        var firstClass = _context.Classes
            .Include(c => c.Center)
            .Any(c => c.Id == request.FirstClassId && c.CenterId == centerId);
        if (!firstClass)
        {
            var error = ErrorDescription.Error["E1130"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var secondClass = _context.Classes
            .Include(c => c.Center)
            .Any(c => c.Id == request.SecondClassId && c.CenterId == centerId);
        if (!secondClass)
        {
            var error = ErrorDescription.Error["E1131"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // get list student in first class
        var listStudentsInFirstClass = _context.StudentsClasses
            .Include(sc => sc.Student)
            .Where(sc => sc.ClassId == request.FirstClassId && sc.IsActive)
            .ToList();

        // get list student in second class
        var listStudentsInSecondClass = _context.StudentsClasses
            .Include(sc => sc.Student)
            .Where(sc => sc.ClassId == request.SecondClassId && sc.IsActive)
            .ToList();

        if (listStudentsInFirstClass.Count == 0)
        {
            var error = ErrorDescription.Error["E1134"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (listStudentsInSecondClass.Count == 0)
        {
            var error = ErrorDescription.Error["E1135"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (listStudentsInFirstClass.Count + listStudentsInSecondClass.Count > MaxNumberStudentInClass)
        {
            var error = ErrorDescription.Error["E1133"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        foreach (var student in listStudentsInFirstClass)
        {
            // if student in second class is not in student class table, add it and change is_active first class to false
            if (!_context.StudentsClasses.Any(sc =>
                    sc.StudentId == student.StudentId && sc.ClassId == request.SecondClassId))
            {
                _context.StudentsClasses.Add(new StudentClass()
                {
                    StudentId = student.StudentId,
                    ClassId = request.SecondClassId,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
                student.IsActive = false;
                student.UpdatedAt = DateTime.Now;
            }
            else
            {
                // if student in second class is in student class table, change is_active first class to false
                var studentClass = _context.StudentsClasses
                    .FirstOrDefault(sc => sc.StudentId == student.StudentId && sc.ClassId == request.SecondClassId);
                if (studentClass != null)
                {
                    studentClass.IsActive = true;
                    studentClass.UpdatedAt = DateTime.Now;
                }

                student.IsActive = false;
                student.UpdatedAt = DateTime.Now;
            }
        }

        var firstClassChange = _context.Classes.FirstOrDefault(c => c.Id == request.FirstClassId);
        if (firstClassChange != null)
        {
            firstClassChange.ClassStatusId = StatusMerged;
            firstClassChange.UpdatedAt = DateTime.Now;
        }

        var secondClassChange = _context.Classes.FirstOrDefault(c => c.Id == request.SecondClassId);
        if (secondClassChange != null)
        {
            secondClassChange.UpdatedAt = DateTime.Now;
        }

        // delete session and delete class schedule of first class
        var listSessionOfFirstClass = _context.Sessions
            .Include(s => s.ClassSchedule)
            .Where(s => s.ClassSchedule.ClassId == request.FirstClassId &&
                        s.ClassSchedule.StartDate.Date > DateTime.Now.Date)
            .ToList();
        _context.Sessions.RemoveRange(listSessionOfFirstClass);
        _context.ClassSchedules.RemoveRange(listSessionOfFirstClass.Select(s => s.ClassSchedule));

        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1132"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Merge class successfully", null!));
    }

    private IQueryable<ClassResponse> GetAvailableClasses(ClassResponse currentClass)
    {
        var availableClasses = _context.Classes
            .Include(c => c.Center)
            .Include(c => c.ClassSchedules)
            .Include(c => c.ClassStatus)
            .Include(c => c.CourseFamily)
            .Include(c => c.StudentsClasses)
            .ThenInclude(sc => sc.Student)
            .Where(c => c.Center.Id == _user.CenterId &&
                        c.Id != currentClass.Id && c.ClassStatusId != StatusMerged &&
                        c.StudentsClasses.Count(sc => sc.IsActive && !sc.Student.IsDraft) < MaxNumberStudentInClass &&
                        c.StudentsClasses.Any(sc => sc.ClassId == c.Id && sc.IsActive && !sc.Student.IsDraft))
            .Select(c => new ClassResponse()
            {
                Id = c.Id,
                Name = c.Name,
                CenterId = c.CenterId,
                CourseFamilyCode = c.CourseFamilyCode,
                ClassDaysId = c.ClassDaysId,
                ClassStatusId = c.ClassStatusId,
                StartDate = c.StartDate,
                CompletionDate = c.CompletionDate,
                GraduationDate = c.GraduationDate,
                ClassHourStart = c.ClassHourStart,
                ClassHourEnd = c.ClassHourEnd,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                Center = new CenterResponse()
                {
                    Id = c.Center.Id,
                    Name = c.Center.Name,
                    CreatedAt = c.Center.CreatedAt,
                    UpdatedAt = c.Center.UpdatedAt,
                    Province = new ProvinceResponse()
                    {
                        Id = c.Center.Province.Id,
                        Code = c.Center.Province.Code,
                        Name = c.Center.Province.Name,
                    },
                    District = new DistrictResponse()
                    {
                        Id = c.Center.District.Id,
                        Name = c.Center.District.Name,
                        Prefix = c.Center.District.Prefix
                    },
                    Ward = new WardResponse()
                    {
                        Id = c.Center.Ward.Id,
                        Name = c.Center.Ward.Name,
                        Prefix = c.Center.Ward.Prefix
                    }
                },
                CourseFamily = new CourseFamilyResponse()
                {
                    Code = c.CourseFamily.Code,
                    Name = c.CourseFamily.Name,
                    IsActive = c.CourseFamily.IsActive,
                    PublishedYear = c.CourseFamily.PublishedYear,
                    CreatedAt = c.CourseFamily.CreatedAt,
                    UpdatedAt = c.CourseFamily.UpdatedAt
                },
                ClassDays = new ClassDaysResponse()
                {
                    Id = c.ClassDays.Id,
                    Value = c.ClassDays.Value
                },
                ClassStatus = new ClassStatusResponse()
                {
                    Id = c.ClassStatus.Id,
                    Value = c.ClassStatus.Value
                },
                SroId = c.Sro.UserId,
                SroFirstName = c.Sro.User.FirstName,
                SroLastName = c.Sro.User.LastName
            });
        return availableClasses;
    }

    private List<StudentResponse> GetAllStudentsByClassId(int id)
    {
        var students = _context.Users.Include(u => u.Student)
            .Include(u => u.Student.StudentsClasses)
            .Include(u => u.Student.Course)
            .Include(u => u.Student.Course.CourseFamily)
            .Include(u => u.Province)
            .Include(u => u.District)
            .Include(u => u.Ward)
            .Include(u => u.Center)
            .Include(u => u.Role)
            .Include(u => u.Gender)
            .Where(u => u.RoleId == RoleIdStudent &&
                        u.Student.StudentsClasses.Any(sc => sc.ClassId == id && sc.IsActive))
            .Select(u => new StudentResponse()
            {
                UserId = u.Student.UserId, Promotion = u.Student.Promotion, Status = u.Student.Status,
                University = u.Student.University, ApplicationDate = u.Student.ApplicationDate,
                ApplicationDocument = u.Student.ApplicationDocument, CompanyAddress = u.Student.CompanyAddress,
                CompanyPosition = u.Student.CompanyPosition, CompanySalary = u.Student.CompanySalary,
                ContactAddress = u.Student.ContactAddress, ContactPhone = u.Student.ContactPhone,
                CourseCode = u.Student.CourseCode, EnrollNumber = u.Student.EnrollNumber,
                FacebookUrl = u.Student.FacebookUrl, FeePlan = u.Student.FeePlan, HighSchool = u.Student.HighSchool,
                HomePhone = u.Student.HomePhone, ParentalName = u.Student.ParentalName,
                ParentalRelationship = u.Student.ParentalRelationship, ParentalPhone = u.Student.ParentalPhone,
                PortfolioUrl = u.Student.PortfolioUrl, StatusDate = u.Student.StatusDate,
                WorkingCompany = u.Student.WorkingCompany, IsDraft = u.Student.IsDraft, Avatar = u.Avatar,

                FirstName = u.FirstName, LastName = u.LastName, Birthday = u.Birthday, Email = u.Email,
                MobilePhone = u.MobilePhone, CenterId = u.CenterId, EmailOrganization = u.EmailOrganization,
                CenterName = u.Center.Name, CreatedAt = u.CreatedAt, UpdatedAt = u.UpdatedAt,
                CitizenIdentityCardNo = u.CitizenIdentityCardNo,
                CitizenIdentityCardPublishedDate = u.CitizenIdentityCardPublishedDate,
                CitizenIdentityCardPublishedPlace = u.CitizenIdentityCardPublishedPlace,

                Course = new CourseResponse()
                {
                    Code = u.Student.Course.Code, Name = u.Student.Course.Name,
                    SemesterCount = u.Student.Course.SemesterCount,
                    CourseFamilyCode = u.Student.Course.CourseFamilyCode, IsActive = u.Student.Course.IsActive,
                    CreatedAt = u.Student.Course.CreatedAt,
                    UpdatedAt = u.Student.Course.UpdatedAt, CourseFamily = new CourseFamilyResponse()
                    {
                        Code = u.Student.Course.CourseFamily.Code, Name = u.Student.Course.CourseFamily.Name,
                        IsActive = u.Student.Course.CourseFamily.IsActive,
                        PublishedYear = u.Student.Course.CourseFamily.PublishedYear,
                        CreatedAt = u.Student.Course.CourseFamily.CreatedAt,
                        UpdatedAt = u.Student.Course.CourseFamily.UpdatedAt
                    }
                },
                Province = new ProvinceResponse()
                {
                    Id = u.Province.Id, Name = u.Province.Name, Code = u.Province.Code
                },
                District = new DistrictResponse()
                {
                    Id = u.District.Id, Name = u.District.Name, Prefix = u.District.Prefix
                },
                Ward = new WardResponse()
                {
                    Id = u.Ward.Id, Name = u.Ward.Name, Prefix = u.Ward.Prefix
                },
                Gender = new GenderResponse()
                {
                    Id = u.Gender.Id, Value = u.Gender.Value
                },
                Role = new RoleResponse()
                {
                    Id = u.Role.Id, Value = u.Role.Value
                }
            })
            .Where(s => s.CenterId == _user.CenterId)
            .ToList();
        return students;
    }

    private List<StudentResponse> GetUnDraftStudentsByClassId(int id)
    {
        var students = _context.Users.Include(u => u.Student)
            .Include(u => u.Student.StudentsClasses)
            .Include(u => u.Student.Course)
            .Include(u => u.Student.Course.CourseFamily)
            .Include(u => u.Province)
            .Include(u => u.District)
            .Include(u => u.Ward)
            .Include(u => u.Center)
            .Include(u => u.Role)
            .Include(u => u.Gender)
            .Where(u => u.RoleId == RoleIdStudent && !u.Student.IsDraft &&
                        u.Student.StudentsClasses.Any(sc => sc.ClassId == id && sc.IsActive))
            .Select(u => new StudentResponse()
            {
                UserId = u.Student.UserId, Promotion = u.Student.Promotion, Status = u.Student.Status,
                University = u.Student.University, ApplicationDate = u.Student.ApplicationDate,
                ApplicationDocument = u.Student.ApplicationDocument, CompanyAddress = u.Student.CompanyAddress,
                CompanyPosition = u.Student.CompanyPosition, CompanySalary = u.Student.CompanySalary,
                ContactAddress = u.Student.ContactAddress, ContactPhone = u.Student.ContactPhone,
                CourseCode = u.Student.CourseCode, EnrollNumber = u.Student.EnrollNumber,
                FacebookUrl = u.Student.FacebookUrl, FeePlan = u.Student.FeePlan, HighSchool = u.Student.HighSchool,
                HomePhone = u.Student.HomePhone, ParentalName = u.Student.ParentalName,
                ParentalRelationship = u.Student.ParentalRelationship, ParentalPhone = u.Student.ParentalPhone,
                PortfolioUrl = u.Student.PortfolioUrl, StatusDate = u.Student.StatusDate,
                WorkingCompany = u.Student.WorkingCompany, IsDraft = u.Student.IsDraft, Avatar = u.Avatar,

                FirstName = u.FirstName, LastName = u.LastName, Birthday = u.Birthday, Email = u.Email,
                MobilePhone = u.MobilePhone, CenterId = u.CenterId, EmailOrganization = u.EmailOrganization,
                CenterName = u.Center.Name, CreatedAt = u.CreatedAt, UpdatedAt = u.UpdatedAt,
                CitizenIdentityCardNo = u.CitizenIdentityCardNo,
                CitizenIdentityCardPublishedDate = u.CitizenIdentityCardPublishedDate,
                CitizenIdentityCardPublishedPlace = u.CitizenIdentityCardPublishedPlace,

                Course = new CourseResponse()
                {
                    Code = u.Student.Course.Code, Name = u.Student.Course.Name,
                    SemesterCount = u.Student.Course.SemesterCount,
                    CourseFamilyCode = u.Student.Course.CourseFamilyCode, IsActive = u.Student.Course.IsActive,
                    CreatedAt = u.Student.Course.CreatedAt,
                    UpdatedAt = u.Student.Course.UpdatedAt, CourseFamily = new CourseFamilyResponse()
                    {
                        Code = u.Student.Course.CourseFamily.Code, Name = u.Student.Course.CourseFamily.Name,
                        IsActive = u.Student.Course.CourseFamily.IsActive,
                        PublishedYear = u.Student.Course.CourseFamily.PublishedYear,
                        CreatedAt = u.Student.Course.CourseFamily.CreatedAt,
                        UpdatedAt = u.Student.Course.CourseFamily.UpdatedAt
                    }
                },
                Province = new ProvinceResponse()
                {
                    Id = u.Province.Id, Name = u.Province.Name, Code = u.Province.Code
                },
                District = new DistrictResponse()
                {
                    Id = u.District.Id, Name = u.District.Name, Prefix = u.District.Prefix
                },
                Ward = new WardResponse()
                {
                    Id = u.Ward.Id, Name = u.Ward.Name, Prefix = u.Ward.Prefix
                },
                Gender = new GenderResponse()
                {
                    Id = u.Gender.Id, Value = u.Gender.Value
                },
                Role = new RoleResponse()
                {
                    Id = u.Role.Id, Value = u.Role.Value
                }
            })
            .Where(s => s.CenterId == _user.CenterId)
            .ToList();
        return students;
    }

    // download template
    [HttpGet]
    [Route("api/classes/download-template-import-students")]
    [Authorize(Roles = "sro")]
    public IActionResult DownloadTemplateStudents()
    {
        // get location of file Template1.xlsx
        var path = Path.Combine(Directory.GetCurrentDirectory(), "TemplateExcel/Template_Student.xlsx");
        using (var workbook = new XLWorkbook(path))
        {
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Template_Student.xlsx");
            }
        }
    }

    private bool IsMobilePhoneExists(string mobilePhone)
    {
        return _context.Users.Any(u => u.MobilePhone.Trim() == mobilePhone.Trim());
    }

    private bool IsEmailExists(string email)
    {
        return _context.Users.Any(e => e.Email.ToLower().Trim() == email.ToLower().Trim());
    }

    private bool IsEmailOrganizationExists(string emailOrganization)
    {
        return _context.Users.Any(e => e.EmailOrganization.ToLower().Trim() == emailOrganization.ToLower().Trim());
    }

    private bool IsCitizenIdentityCardNoExists(string citizenIdentityCardNo)
    {
        return _context.Users.Any(e => e.CitizenIdentityCardNo == citizenIdentityCardNo);
    }

    private bool IsProvinceExists(int provinceId)
    {
        return _context.Provinces.Any(e => e.Id == provinceId);
    }

    private bool IsDistrictExists(int districtId)
    {
        return _context.Districts.Any(e => e.Id == districtId);
    }

    private bool IsWardExists(int wardId)
    {
        return _context.Wards.Any(e => e.Id == wardId);
    }

    private bool IsAddressExists(int provinceId, int districtId, int wardId)
    {
        return _context.Provinces
            .Include(p => p.Districts)
            .Include(p => p.Wards)
            .Any(p => p.Id == provinceId && p.Districts.Any(d => d.Id == districtId && d.Province.Id == provinceId) &&
                      p.Wards.Any(w =>
                          w.Id == wardId && w.District.Id == districtId && w.Province.Id == provinceId));
    }

    private bool IsGenderExists(int genderId)
    {
        return _context.Genders.Any(g => g.Id == genderId);
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

    // convert to unsigned string
    private string ConvertToUnsignedString(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();
        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormD).Trim().ToLower();
    }
}