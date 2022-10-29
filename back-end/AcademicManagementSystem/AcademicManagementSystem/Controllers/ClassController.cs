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
            ClassStatusId = NotScheduleYet,
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

        var classToUpdate = _context.Classes.FirstOrDefault(c => c.Id == classId && c.CenterId == _user.CenterId);

        if (classToUpdate == null)
        {
            return NotFound(CustomResponse.NotFound("Class not found in this center"));
        }

        var errorCode = GetCodeIfOccuredErrorWhenUpdate(classId, request, classToUpdate.StartDate);

        if (errorCode != null)
        {
            var error = ErrorDescription.Error[errorCode];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
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

        if (request.ClassHourEnd - request.ClassHourStart < TimeSpan.FromHours(1)
            || request.ClassHourEnd - request.ClassHourStart > TimeSpan.FromHours(4))
        {
            return "E0072";
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
        //is class exists
        var existedClass = _context.Classes.Any(c => c.Id == id);
        if (!existedClass)
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

        //format date time from excel
        var startDate = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
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
                        var contactAddress = row.Cell(18).Value.ToString();
                        var ward = row.Cell(19).Value.ToString();
                        var district = row.Cell(20).Value.ToString();
                        var province = row.Cell(21).Value.ToString();
                        var parentalName = row.Cell(22).Value.ToString();
                        var parentalRelative = row.Cell(23).Value.ToString();
                        var parentalPhone = row.Cell(24).Value.ToString();
                        var applicationDate = row.Cell(25).Value.ToString();
                        var applicationDocuments = row.Cell(26).Value.ToString();
                        var courseCode = row.Cell(27).Value.ToString();
                        var highSchool = row.Cell(29).Value.ToString();
                        var university = row.Cell(30).Value.ToString();
                        var facebookUrl = row.Cell(31).Value.ToString();
                        var portfolio = row.Cell(32).Value.ToString();
                        var workingCompany = row.Cell(33).Value.ToString();
                        var companySalary = row.Cell(34).Value.ToString() == ""
                            ? 0
                            : Convert.ToInt32(row.Cell(34).Value.ToString());
                        var companyPosition = row.Cell(35).Value.ToString();
                        var companyAddress = row.Cell(36).Value.ToString();
                        var feePlan = row.Cell(37).Value.ToString();
                        var promotion = row.Cell(38).Value.ToString();

                        // check if user exist
                        var existedUser = _context.Users.FirstOrDefault(u =>
                            u.Email == email || u.EmailOrganization == emailOrganization ||
                            u.MobilePhone == mobilePhone ||
                            u.CitizenIdentityCardNo == identityCardNo);
                        if (existedUser != null)
                        {
                            var error = ErrorDescription.Error["E1071"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + studentNo, error.Type));
                        }

                        // check if students exist
                        var existedStudent = _context.Students.FirstOrDefault(u =>
                            string.Equals(u.EnrollNumber.ToLower(), enrollNumber!.ToLower()));
                        if (existedStudent != null)
                        {
                            var error = ErrorDescription.Error["E1070"];
                            return BadRequest(CustomResponse.BadRequest(error.Message + studentNo, error.Type));
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
                            CenterId = _user.CenterId,
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
                        return BadRequest(CustomResponse.BadRequest(error.Message + " at Student No " + studentNo,
                            error.Type));
                    }

                    return Ok(CustomResponse.Ok("Import file successfully", null!));
                }
            }
        }
        catch (Exception e)
        {
            return BadRequest(CustomResponse.BadRequest(e.Message + " at Student No " + studentNo,
                e.GetType().ToString()));
        }
    }

    // add new student to class
    [HttpPost]
    [Route("api/classes/{id:int}/students")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult AddStudentToClass(int id, [FromBody] AddStudentToClassRequest request)
    {
        //is class exists
        var existedClass = _context.Classes.Any(c => c.Id == id);
        if (!existedClass)
        {
            var error = ErrorDescription.Error["E1073"];
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

        if (IsEmailExists(request.EmailOrganization))
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

        if (IsEmailOrganizationExists(request.Email))
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

        if (request.Status is < 1 or > 7)
        {
            var error = ErrorDescription.Error["E1094"];
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
            CenterId = _user.CenterId,
            GenderId = request.GenderId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Student = new Student()
            {
                EnrollNumber = request.EnrollNumber,
                CourseCode = request.CourseCode,
                Status = request.Status,
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
    [Authorize(Roles = "admin, sro")]
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
    [Authorize(Roles = "admin, sro")]
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
    [Authorize(Roles = "admin, sro")]
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

    private List<StudentResponse> GetAllStudentsByClassId(int id)
    {
        var students = _context.Users.Include(u => u.Student)
            .Include(u => u.Student.Course)
            .Include(u => u.Student.Course.CourseFamily)
            .Include(u => u.Province)
            .Include(u => u.District)
            .Include(u => u.Ward)
            .Include(u => u.Center)
            .Include(u => u.Role)
            .Include(u => u.Gender)
            .Where(u => u.RoleId == RoleIdStudent && u.Student.StudentsClasses.Any(sc => sc.ClassId == id))
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
            .Where(c => c.CenterId == _user.CenterId)
            .ToList();
        return students;
    }

    // download template
    [HttpGet]
    [Route("api/classes/download-template-import-students")]
    [Authorize(Roles = "admin, sro")]
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
                          w.Id == wardId && w.District.Id == districtId && w.District.Province.Id == provinceId));
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
}