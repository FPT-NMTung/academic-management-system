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
using AcademicManagementSystem.Models.CourseFamilyController;
using AcademicManagementSystem.Models.UserController;
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

    public ClassController(AmsContext context, IUserService userService)
    {
        _context = context;
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
        request.Name = Regex.Replace(request.Name, StringConstant.RegexWhiteSpaces, " ").Trim();

        var errorCode = GetCodeIfOccuredErrorWhenCreate(request);

        if (errorCode != null)
        {
            var error = ErrorDescription.Error[errorCode];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var newClass = new Class()
        {
            CenterId = _user.CenterId,
            CourseFamilyCode = request.CourseFamilyCode,
            ClassDaysId = request.ClassDaysId,
            ClassStatusId = request.ClassStatusId,
            SroId = _user.Id,
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

        var errorCode = GetCodeIfOccuredErrorWhenUpdate(classId, request);

        if (errorCode != null)
        {
            var error = ErrorDescription.Error[errorCode];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var classToUpdate = _context.Classes.FirstOrDefault(c => c.Id == classId && c.CenterId == _user.CenterId);

        if (classToUpdate == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found in this center"));
        }

        classToUpdate.CourseFamilyCode = request.CourseFamilyCode;
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

        // allow special characters: ()-_
        if (Regex.IsMatch(request.Name, StringConstant.RegexSpecialCharactersNotAllowForClassName))
        {
            return "E0069";
        }

        if (request.ClassHourStart >= request.ClassHourEnd)
        {
            return "E0072";
        }

        return IsClassExist(request.Name, _user.CenterId, false, 0) ? "E0070" : null;
    }

    private string? GetCodeIfOccuredErrorWhenUpdate(int classId, UpdateClassRequest request)
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

        if (request.ClassHourStart >= request.ClassHourEnd)
        {
            return "E0072";
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
            }).Where(c => c.CenterId == _user.CenterId);
    }


    // import student from excel file
    [HttpPost]
    [Route("api/classes/{id:int}/students-from-excel")]
    [Authorize(Roles = "admin, sro")]
    public ActionResult ImportStudentFromExcel(int id)
    {
        try
        {
            var file = Request.Form.Files[0];
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    //format date time from excel
                    var startDate = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    // get all rows with value
                    var rows = worksheet.RowsUsed();
                    foreach (var row in rows)
                    {
                        // skip the first and second row (number and column name)
                        if (row.RowNumber() == 1 || row.RowNumber() == 2) continue;
                        //define
                        var enrollNumber = row.Cell(2).Value.ToString();
                        var firstName = row.Cell(3).Value.ToString();
                        var lastName = row.Cell(4).Value.ToString();
                        var status = row.Cell(6).Value.ToString();
                        var statusDate = row.Cell(7).Value.ToString();
                        var gender = row.Cell(8).Value.ToString();
                        var birthday = row.Cell(9).Value.ToString();
                        var mobilePhone = row.Cell(10).Value.ToString();
                        var homePhone = row.Cell(11).Value.ToString();
                        var contactPhone = row.Cell(12).Value.ToString();
                        var email = row.Cell(13).Value.ToString();
                        var emailOrganization = row.Cell(14).Value.ToString();
                        var identityCardNo = row.Cell(15).Value.ToString();
                        var identityCardPublishedDate = row.Cell(16).Value.ToString();
                        var identityCardPublishedPlace = row.Cell(17).Value.ToString();
                        var contactAddress = row.Cell(19).Value.ToString();
                        var ward = row.Cell(20).Value.ToString();
                        var district = row.Cell(21).Value.ToString();
                        var province = row.Cell(22).Value.ToString();
                        var parentalName = row.Cell(23).Value.ToString();
                        var parentalRelative = row.Cell(24).Value.ToString();
                        var parentalPhone = row.Cell(25).Value.ToString();
                        var applicationDate = row.Cell(26).Value.ToString();
                        var applicationDocuments = row.Cell(27).Value.ToString();
                        var courseCode = row.Cell(29).Value.ToString();
                        var highSchool = row.Cell(31).Value.ToString();
                        var university = row.Cell(32).Value.ToString();
                        var facebookUrl = row.Cell(36).Value.ToString();
                        var portfolio = row.Cell(38).Value.ToString();
                        var workingCompany = row.Cell(39).Value.ToString();
                        var companySalary = row.Cell(40).Value;
                        var companyPosition = row.Cell(41).Value.ToString();
                        var companyAddress = row.Cell(43).Value.ToString();
                        var feePlan = row.Cell(47).Value.ToString();
                        var promotion = row.Cell(48).Value.ToString();

                        // check if user exist
                        var existedUser = _context.Users.FirstOrDefault(u => u.CitizenIdentityCardNo == identityCardNo);
                        if (existedUser != null)
                        {
                            var error = ErrorDescription.Error["E1071"];
                            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                        }

                        // check if students exist
                        var existedStudent = _context.Students.FirstOrDefault(u =>
                            string.Equals(u.EnrollNumber.ToLower(), enrollNumber!.ToLower()));
                        if (existedStudent != null)
                        {
                            var error = ErrorDescription.Error["E1070"];
                            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                        }

                        var provinceId = _context.Provinces.FirstOrDefault(p =>
                            string.Equals(p.Name.ToLower(), province!.ToLower()))?.Id == null
                            ? 1
                            : _context.Provinces.FirstOrDefault(p => p.Name == province)!.Id;

                        var districtId = _context.Districts.FirstOrDefault(d =>
                            string.Equals(d.Name.ToLower(), district!.ToLower()))?.Id == null
                            ? 1
                            : _context.Districts.FirstOrDefault(d => d.Name == district)!.Id;

                        var wardId = _context.Wards.FirstOrDefault(w =>
                            string.Equals(w.Name.ToLower(), ward!.ToLower()))?.Id == null
                            ? 1
                            : _context.Wards.FirstOrDefault(w => w.Name == ward)!.Id;

                        var centerId = _context.Classes.Include(c => c.Center)
                            .FirstOrDefault(c => c.Id == id)?.Center.Id!;

                        var newBirthday = startDate.AddDays(Convert.ToDouble(birthday)).ToLocalTime();
                        var newIdentityCardPublishedDate =
                            startDate.AddDays(Convert.ToDouble(identityCardPublishedDate)).ToLocalTime();
                        var newStatusDate = startDate.AddDays(Convert.ToDouble(statusDate)).ToLocalTime();
                        var newApplicationDate = startDate.AddDays(Convert.ToDouble(applicationDate)).ToLocalTime();

                        var genderId = gender switch
                        {
                            "Male" => 1,
                            "Female" => 2,
                            "Not Known" => 3,
                            "Not Applicable" => 4,
                            _ => 4
                        };

                        var learningStatus = status switch
                        {
                            "Studying" => 1,
                            "Delay" => 2,
                            "Dropout" => 3,
                            "ClassQueue" => 4,
                            "Transfer" => 5,
                            "Upgrade" => 6,
                            "Finished" => 7,
                            _ => 0
                        };

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
                            CenterId = (int)centerId,
                            GenderId = genderId,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            Student = new Student()
                            {
                                EnrollNumber = enrollNumber!.Trim(),
                                CourseCode = courseCode!.Trim(),
                                Status = learningStatus,
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
                                CompanySalary = companySalary as int?,
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
                                        CreatedAt = DateTime.Now,
                                        UpdatedAt = DateTime.Now,
                                    }
                                }
                            }
                        };
                        _context.Users.Add(user);
                        _context.Students.Add(user.Student);
                        _context.StudentsClasses.Add(user.Student.StudentsClasses.First());
                    }

                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        var error = ErrorDescription.Error["E1069"];
                        return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                    }

                    return Ok(CustomResponse.Ok("Import file successfully", null!));
                }
            }
        }
        catch (Exception e)
        {
            return BadRequest(CustomResponse.BadRequest(e.Message, e.GetType().ToString()));
        }
    }

    // save imported file
    [HttpPut]
    [Route("api/classes/{id:int}/students-from-excel")]
    public IActionResult SaveImportFile(int id)
    {
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