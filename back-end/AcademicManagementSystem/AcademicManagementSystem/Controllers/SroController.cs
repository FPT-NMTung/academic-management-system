using System.Text;
using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.GenderController;
using AcademicManagementSystem.Models.RoleController;
using AcademicManagementSystem.Models.UserController.SroController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class SroController : ControllerBase
{
    private readonly AmsContext _context;
    private const int SroRoleId = 2;
    private const string RegexSpecialCharacters = StringConstant.RegexSpecialCharsNotAllowForPersonName;
    private const string Digits = StringConstant.RegexDigits;

    public SroController(AmsContext context)
    {
        _context = context;
    }

    //get all sro
    [HttpGet]
    [Route("api/sros")]
    [Authorize(Roles = "admin")]
    public IActionResult GetAllSro()
    {
        var allSro = GetAllUserRoleSro();
        return Ok(CustomResponse.Ok("Get All Sro successfully", allSro));
    }

    //get all sro by centerId
    [HttpGet]
    [Route("api/centers/{centerId:int}/sros")]
    [Authorize(Roles = "admin")]
    public IActionResult GetAllSroByCenterId(int centerId)
    {
        if (!IsCenterExists(centerId))
        {
            var error = ErrorDescription.Error["E0016"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var listSro = GetAllUserRoleSro().Where(sro => sro.CenterId == centerId);

        return Ok(CustomResponse.Ok("Get All Sro By centerId successfully", listSro));
    }

    //get sro by id
    [HttpGet]
    [Route("api/sros/{id:int}")]
    [Authorize(Roles = "admin")]
    public IActionResult GetSroById(int id)
    {
        var sro = GetAllUserRoleSro().FirstOrDefault(s => s.UserId == id);

        if (sro == null)
        {
            return NotFound(CustomResponse.NotFound("Sro not found"));
        }

        return Ok(CustomResponse.Ok("Get sro by Id successfully", sro));
    }

    // search sro by firstName, lastName, mobilePhone, email, emailOrganization
    [HttpGet]
    [Route("api/sros/search")]
    [Authorize(Roles = "admin")]
    public IActionResult SearchSro(string? firstName, string? lastName,
        string? mobilePhone, string? email, string? emailOrganization)
    {
        var sFirstName = firstName == null ? string.Empty : RemoveDiacritics(firstName.Trim().ToLower());
        var sLastName = lastName == null ? string.Empty : RemoveDiacritics(lastName.Trim().ToLower());
        var sMobilePhone = mobilePhone == null ? string.Empty : RemoveDiacritics(mobilePhone.Trim().ToLower());
        var sEmail = email == null ? string.Empty : RemoveDiacritics(email.Trim().ToLower());
        var sEmailOrganization = emailOrganization == null
            ? string.Empty
            : RemoveDiacritics(emailOrganization.Trim().ToLower());

        var listSro = GetAllUserRoleSro();

        if (sFirstName == string.Empty && sLastName == string.Empty && sMobilePhone == string.Empty
            && sEmail == string.Empty && sEmailOrganization == string.Empty)
        {
            return Ok(CustomResponse.Ok("Search sros successfully", listSro));
        }

        var sroResponse = new List<SroResponse>();
        foreach (var s in listSro)
        {
            var s1 = RemoveDiacritics(s.FirstName!.ToLower());
            var s2 = RemoveDiacritics(s.LastName!.ToLower());
            var s3 = RemoveDiacritics(s.MobilePhone!.ToLower());
            var s4 = RemoveDiacritics(s.Email!.ToLower());
            var s5 = RemoveDiacritics(s.EmailOrganization!.ToLower());

            if (s1.Contains(sFirstName)
                && s2.Contains(sLastName)
                && s3.Contains(sMobilePhone)
                && s4.Contains(sEmail)
                && s5.Contains(sEmailOrganization))
            {
                sroResponse.Add(s);
            }
        }

        return Ok(CustomResponse.Ok("Search SRO successfully", sroResponse));
    }

    // create sro
    [HttpPost]
    [Route("api/sros")]
    [Authorize(Roles = "admin")]
    public IActionResult CreateSro([FromBody] CreateSroRequest request)
    {
        if (CheckSroNameForCreate(request, out var badRequest)) return badRequest;
        
        if(request.Birthday.Date >= DateTime.Now.Date)
        {
            var error = ErrorDescription.Error["E0022_3"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (CheckMobilePhoneForCreate(request, out var badRequestObjectResult)) return badRequestObjectResult;

        if (CheckEmailAndEmailOrganizationForCreate(request, out var actionResult1)) return actionResult1;

        if (IsCitizenIdentityCardNoExists(request.CitizenIdentityCardNo, false, 0))
        {
            var error = ErrorDescription.Error["E0027"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }
        
        if (request.CitizenIdentityCardPublishedDate >= DateTime.Now.Date)
        {
            var error = ErrorDescription.Error["E0022_4"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.CitizenIdentityCardNo, StringConstant.RegexCitizenIdCardNo))
        {
            var error = ErrorDescription.Error["E0033"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var user = new User()
        {
            ProvinceId = request.ProvinceId,
            DistrictId = request.DistrictId,
            WardId = request.WardId,
            CenterId = request.CenterId,
            GenderId = request.GenderId,
            RoleId = SroRoleId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Avatar = request.Avatar,
            MobilePhone = request.MobilePhone,
            Email = request.Email,
            EmailOrganization = request.EmailOrganization,
            Birthday = request.Birthday,
            CitizenIdentityCardNo = request.CitizenIdentityCardNo,
            CitizenIdentityCardPublishedDate = request.CitizenIdentityCardPublishedDate,
            CitizenIdentityCardPublishedPlace = request.CitizenIdentityCardPublishedPlace,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = true
        };

        /*
         * if sro has another parameters, refer to create teacher 
         */
        _context.Users.Add(user);
        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0037"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var sro = new Sro()
        {
            UserId = user.Id,
        };

        _context.Sros.Add(sro);
        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0038"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var sroResponse = GetAllUserRoleSro().FirstOrDefault(s => s.UserId == sro.UserId);
        return Ok(CustomResponse.Ok("Create SRO successfully", sroResponse!));
    }

    // update sro
    [HttpPut]
    [Route("api/sros/{id:int}")]
    [Authorize(Roles = "admin")]
    public IActionResult UpdateSro([FromRoute] int id, [FromBody] UpdateSroRequest request)
    {
        var sro = _context.Sros.FirstOrDefault(s => s.UserId == id);

        if (sro == null)
        {
            var error = ErrorDescription.Error["E0017"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var user = _context.Users.FirstOrDefault(u => u.Id == sro.UserId);
        if (user == null)
        {
            var error = ErrorDescription.Error["E0036"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (CheckSroNameForUpdate(request, out var badRequest)) return badRequest;
        
        if(request.Birthday.Date >= DateTime.Now.Date)
        {
            var error = ErrorDescription.Error["E0022_3"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (CheckMobilePhoneForUpdate(id, request, out var updateSro)) return updateSro;

        if (CheckEmailAndEmailOrganizationForUpdate(id, request, out var badRequestObjectResult))
            return badRequestObjectResult;

        if (IsCitizenIdentityCardNoExists(request.CitizenIdentityCardNo, true, id))
        {
            var error = ErrorDescription.Error["E0027"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }
        
        if (request.CitizenIdentityCardPublishedDate >= DateTime.Now.Date)
        {
            var error = ErrorDescription.Error["E0022_4"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.CitizenIdentityCardNo, StringConstant.RegexCitizenIdCardNo))
        {
            var error = ErrorDescription.Error["E0033"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        user.ProvinceId = request.ProvinceId;
        user.DistrictId = request.DistrictId;
        user.WardId = request.WardId;
        user.GenderId = request.GenderId;
        user.RoleId = SroRoleId;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.MobilePhone = request.MobilePhone;
        user.Email = request.Email;
        user.EmailOrganization = request.EmailOrganization;
        user.Birthday = request.Birthday;
        user.CitizenIdentityCardNo = request.CitizenIdentityCardNo;
        user.CitizenIdentityCardPublishedDate = request.CitizenIdentityCardPublishedDate;
        user.CitizenIdentityCardPublishedPlace = request.CitizenIdentityCardPublishedPlace;
        user.UpdatedAt = DateTime.Now;

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0038"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var sroResponse = GetAllUserRoleSro().FirstOrDefault(s => s.UserId == sro.UserId);
        return Ok(CustomResponse.Ok("Update SRO successfully", sroResponse!));
    }

    // Can delete sro
    [HttpGet]
    [Route("api/sros/{id:int}/can-delete")]
    [Authorize(Roles = "admin")]
    public IActionResult CanDeleteSro(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id && u.RoleId == SroRoleId);
        if (user == null)
        {
            return NotFound(CustomResponse.NotFound("Not Found Sro"));
        }

        var canDelete = CanDelete(id);

        return Ok(CustomResponse.Ok("Can delete this sro", new CheckSroCanDeleteResponse()
        {
            CanDelete = canDelete
        }));
    }

    // delete sro
    [HttpDelete]
    [Route("api/sros/{id:int}")]
    [Authorize(Roles = "admin")]
    public IActionResult DeleteSro(int id)
    {
        var user = _context.Users
            .Include(u => u.Sro)
            .FirstOrDefault(u => u.Id == id && u.RoleId == SroRoleId);
        if (user == null)
        {
            return NotFound(CustomResponse.NotFound("Not Found Sro"));
        }

        try
        {
            _context.Sros.Remove(user.Sro);
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E1119"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Sro deleted successfully", null!));
    }

    // change activate sro
    [HttpPatch]
    [Route("api/sros/{id:int}/change-active")]
    [Authorize(Roles = "admin")]
    public IActionResult ChangeActivateSro(int id)
    {
        var sro = _context.Sros.Include(s => s.User).FirstOrDefault(s => s.UserId == id);
        if (sro == null)
        {
            var error = ErrorDescription.Error["E0017"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        try
        {
            sro.User.IsActive = !sro.User.IsActive;
            sro.User.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
        }
        catch (Exception)
        {
            var error = ErrorDescription.Error["E2057"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Change active successfully", null!));
    }

    private bool IsCenterExists(int centerId)
    {
        return _context.Centers.Any(e => e.Id == centerId);
    }

    private bool IsMobilePhoneExists(string? mobilePhone, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e => e.MobilePhone == mobilePhone && e.Id != userId)
            : _context.Users.Any(e => e.MobilePhone == mobilePhone);
    }

    private bool IsEmailExists(string email, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e => e.Email.Trim().ToLower().Equals(email.Trim().ToLower()) && e.Id != userId)
            : _context.Users.Any(e => e.Email.Trim().ToLower().Equals(email.Trim().ToLower()));
    }

    private bool IsEmailOrganizationExists(string emailOrganization, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e =>
                e.EmailOrganization.Trim().ToUpper().Equals(emailOrganization.Trim().ToUpper()) && e.Id != userId)
            : _context.Users.Any(e => e.EmailOrganization.Trim().ToUpper().Equals(emailOrganization.Trim().ToUpper()));
    }

    private bool IsCitizenIdentityCardNoExists(string? citizenIdentityCardNo, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e => e.CitizenIdentityCardNo == citizenIdentityCardNo && e.Id != userId)
            : _context.Users.Any(e => e.CitizenIdentityCardNo == citizenIdentityCardNo);
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

    private IQueryable<SroResponse> GetAllUserRoleSro()
    {
        var allSro = _context.Users
            .Include(u => u.Province)
            .Include(u => u.District)
            .Include(u => u.Ward)
            .Include(u => u.Role)
            .Include(u => u.Gender)
            .Include(u => u.Center)
            .Include(u => u.Sro)
            .Where(u => u.RoleId == SroRoleId)
            .Select(s => new SroResponse()
            {
                UserId = s.Id,
                Role = new RoleResponse()
                {
                    Id = s.Role.Id,
                    Value = s.Role.Value
                },
                FirstName = s.FirstName,
                LastName = s.LastName,
                MobilePhone = s.MobilePhone,
                Email = s.Email,
                EmailOrganization = s.EmailOrganization,
                Avatar = s.Avatar,
                Province = new ProvinceResponse()
                {
                    Id = s.Province.Id,
                    Code = s.Province.Code,
                    Name = s.Province.Name
                },
                District = new DistrictResponse()
                {
                    Id = s.District.Id,
                    Name = s.District.Name,
                    Prefix = s.District.Prefix
                },
                Ward = new WardResponse()
                {
                    Id = s.Ward.Id,
                    Name = s.Ward.Name,
                    Prefix = s.Ward.Prefix
                },
                Gender = new GenderResponse()
                {
                    Id = s.Gender.Id,
                    Value = s.Gender.Value
                },
                Birthday = s.Birthday,
                CenterId = s.CenterId,
                CenterName = s.Center.Name,
                CitizenIdentityCardNo = s.CitizenIdentityCardNo,
                CitizenIdentityCardPublishedDate = s.CitizenIdentityCardPublishedDate,
                CitizenIdentityCardPublishedPlace = s.CitizenIdentityCardPublishedPlace,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            });
        return allSro;
    }

    private bool CanDelete(int id)
    {
        var user = _context.Users
            .Include(u => u.Sro)
            .Include(u => u.Sro.Classes)
            .Include(u => u.ActiveRefreshTokens)
            .FirstOrDefault(u => u.Id == id && u.RoleId == SroRoleId && u.ActiveRefreshTokens.Count == 0);

        if (user == null)
        {
            return false;
        }
        

        return !user.Sro.Classes.Any();
    }

    private bool CheckEmailAndEmailOrganizationForCreate(CreateSroRequest request, out IActionResult actionResult1)
    {
        if (IsEmailExists(request.Email, false, 0))
        {
            var error = ErrorDescription.Error["E0021"];
            actionResult1 = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // email organization cannot be the same as email
        if (IsEmailExists(request.EmailOrganization, false, 0))
        {
            var error = ErrorDescription.Error["E0022_1"];
            actionResult1 = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (!Regex.IsMatch(request.Email, StringConstant.RegexEmailCopilot))
        {
            var error = ErrorDescription.Error["E0031"];
            actionResult1 = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (IsEmailOrganizationExists(request.EmailOrganization, false, 0))
        {
            var error = ErrorDescription.Error["E0022"];
            actionResult1 = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // email cannot be the same as emailOrganization
        if (IsEmailOrganizationExists(request.Email, false, 0))
        {
            var error = ErrorDescription.Error["E0021_1"];
            actionResult1 = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (request.Email.ToLower() == request.EmailOrganization.ToLower())
        {
            var error = ErrorDescription.Error["E0022_2"];
            actionResult1 = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (!Regex.IsMatch(request.EmailOrganization, StringConstant.RegexEmailCopilot))
        {
            var error = ErrorDescription.Error["E0032"];
            actionResult1 = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }
        
        actionResult1 = null!;
        return false;
    }

    private bool CheckMobilePhoneForCreate(CreateSroRequest request, out IActionResult badRequestObjectResult)
    {
        if (IsMobilePhoneExists(request.MobilePhone, false, 0))
        {
            var error = ErrorDescription.Error["E0020"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (!Regex.IsMatch(request.MobilePhone, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E0030"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        badRequestObjectResult = null!;
        return false;
    }

    private bool CheckSroNameForCreate(CreateSroRequest request, out IActionResult badRequest)
    {
        if (request.FirstName.Trim().Equals(string.Empty) || request.LastName.Trim().Equals(string.Empty))
        {
            badRequest =
                BadRequest(CustomResponse.BadRequest("firstName, lastName cannot be empty", "error-sro-01"));
            return true;
        }

        request.FirstName = Regex.Replace(request.FirstName, StringConstant.RegexWhiteSpaces, " ");
        // function replace string ex: H ' Hen Nie => H'Hen Nie
        request.FirstName = request.FirstName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.FirstName, RegexSpecialCharacters)
            || Regex.IsMatch(request.FirstName, Digits))
        {
            var error = ErrorDescription.Error["E0034"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        request.LastName = Regex.Replace(request.LastName, StringConstant.RegexWhiteSpaces, " ");
        request.LastName = request.LastName.Replace(" ' ", "'").Trim();

        if (Regex.IsMatch(request.LastName, RegexSpecialCharacters) ||
            Regex.IsMatch(request.LastName, Digits))
        {
            var error = ErrorDescription.Error["E0035"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        badRequest = null!;
        return false;
    }

    private bool CheckEmailAndEmailOrganizationForUpdate(int id, UpdateSroRequest request,
        out IActionResult badRequestObjectResult)
    {
        if (IsEmailExists(request.Email, true, id))
        {
            var error = ErrorDescription.Error["E0021"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // email organization cannot be the same as email
        if (IsEmailExists(request.EmailOrganization, true, id))
        {
            var error = ErrorDescription.Error["E0022_1"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (!Regex.IsMatch(request.Email, StringConstant.RegexEmailCopilot))
        {
            var error = ErrorDescription.Error["E0031"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;        
        }

        if (IsEmailOrganizationExists(request.EmailOrganization, true, id))
        {
            var error = ErrorDescription.Error["E0022"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        // email cannot be the same as email organization
        if (IsEmailOrganizationExists(request.Email, true, id))
        {
            var error = ErrorDescription.Error["E0021_1"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (request.Email.ToLower() == request.EmailOrganization.ToLower())
        {
            var error = ErrorDescription.Error["E0022_2"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (!Regex.IsMatch(request.EmailOrganization, StringConstant.RegexEmailCopilot))
        {
            var error = ErrorDescription.Error["E0032"];
            badRequestObjectResult = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;   
        }
        badRequestObjectResult = null!;
        return false;
    }

    private bool CheckMobilePhoneForUpdate(int id, UpdateSroRequest request, out IActionResult updateSro)
    {
        if (IsMobilePhoneExists(request.MobilePhone, true, id))
        {
            var error = ErrorDescription.Error["E0020"];
            updateSro = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        if (!Regex.IsMatch(request.MobilePhone, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E0030"];
            updateSro = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        updateSro = null!;
        return false;
    }

    private bool CheckSroNameForUpdate(UpdateSroRequest request, out IActionResult badRequest)
    {
        if (request.FirstName.Trim().Equals(string.Empty) || request.LastName.Trim().Equals(string.Empty))
        {
            badRequest = BadRequest(CustomResponse.BadRequest("firstName, lastName cannot be empty", "error-sro-01"));
            return true;
        }

        request.FirstName = Regex.Replace(request.FirstName, StringConstant.RegexWhiteSpaces, " ");
        // function replace string ex: H ' Hen Nie => H'Hen Nie
        request.FirstName = request.FirstName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.FirstName, RegexSpecialCharacters) ||
            Regex.IsMatch(request.FirstName, Digits))
        {
            var error = ErrorDescription.Error["E0034"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        request.LastName = Regex.Replace(request.LastName, StringConstant.RegexWhiteSpaces, " ");
        request.LastName = request.LastName.Replace(" ' ", "'").Trim();

        if (Regex.IsMatch(request.LastName, RegexSpecialCharacters) ||
            Regex.IsMatch(request.LastName, Digits))
        {
            var error = ErrorDescription.Error["E0035"];
            badRequest = BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
            return true;
        }

        badRequest = null!;
        return false;
    }
}