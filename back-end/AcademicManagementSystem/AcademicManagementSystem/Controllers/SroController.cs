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
        var allSro = _context.Users
            .Include(u => u.Province)
            .Include(u => u.District)
            .Include(u => u.Ward)
            .Include(u => u.Role)
            .Include(u => u.Gender)
            .Include(u => u.Center)
            .Include(u => u.Sro)
            .Where(u => u.RoleId == SroRoleId)
            .ToList()
            .Select(s => new SroResponse()
            {
                UserId = s.Id,
                Role = new RoleResponse()
                {
                    RoleId = s.Role.Id,
                    RoleValue = s.Role.Value
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
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            });

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

        var listSro = _context.Users
            .Include(u => u.Province)
            .Include(u => u.District)
            .Include(u => u.Ward)
            .Include(u => u.Role)
            .Include(u => u.Gender)
            .Include(u => u.Center)
            .Include(u => u.Sro)
            .Where(u => u.RoleId == SroRoleId && u.CenterId == centerId)
            .ToList()
            .Where(s => s.CenterId == centerId)
            .Select(s => new SroResponse()
            {
                UserId = s.Id,
                Role = new RoleResponse()
                {
                    RoleId = s.Role.Id,
                    RoleValue = s.Role.Value
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
                CitizenIdentityCardPublishedPlace = s.CitizenIdentityCardPublishedPlace
            });

        return Ok(!listSro.Any()
            ? CustomResponse.Ok("This center don't have any SRO", listSro)
            : CustomResponse.Ok("Get All Sro By centerId successfully", listSro));
    }

    //get sro by id
    [HttpGet]
    [Route("api/sros/{id:int}")]
    [Authorize(Roles = "admin")]
    public IActionResult GetSroById(int id)
    {
        if (!IsSroExists(id))
        {
            var error = ErrorDescription.Error["E0017"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var sro = GetSroResponse(id);

        return Ok(CustomResponse.Ok("Get sro by Id successfully", sro));
    }

    // search sro by firstName, lastName, mobilePhone, email, emailOrganization
    [HttpGet]
    [Route("api/sros/search")]
    [Authorize(Roles = "admin")]
    public IActionResult SearchSro(string firstName = "", string lastName = "",
        string mobilePhone = "", string email = "",
        string emailOrganization = "")
    {
        var sFirstName = RemoveDiacritics(firstName!.Trim().ToLower());
        var sLastName = RemoveDiacritics(lastName!.Trim().ToLower());
        var sMobilePhone = RemoveDiacritics(mobilePhone!.Trim().ToLower());
        var sEmail = RemoveDiacritics(email!.Trim().ToLower());
        var sEmailOrganization = RemoveDiacritics(emailOrganization!.Trim().ToLower());

        var listSro = _context.Users
            .Include(u => u.Province)
            .Include(u => u.District)
            .Include(u => u.Ward)
            .Include(u => u.Role)
            .Include(u => u.Gender)
            .Include(u => u.Center)
            .Include(u => u.Sro)
            .Where(u => u.RoleId == SroRoleId)
            .ToList()
            .Select(s => new SroResponse()
            {
                UserId = s.Id,
                Role = new RoleResponse()
                {
                    RoleId = s.Role.Id,
                    RoleValue = s.Role.Value
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
                CitizenIdentityCardPublishedPlace = s.CitizenIdentityCardPublishedPlace
            });

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
        request.FirstName = Regex.Replace(request.FirstName!, StringConstant.RegexWhiteSpaces, " ");
        // function replace string ex: H ' Hen Nie => H'Hen Nie
        request.FirstName = request.FirstName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.FirstName, RegexSpecialCharacters))
        {
            var error = ErrorDescription.Error["E0034"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.LastName = Regex.Replace(request.LastName!, StringConstant.RegexWhiteSpaces, " ");
        request.LastName = request.LastName.Replace(" ' ", "'").Trim();

        if (Regex.IsMatch(request.LastName, RegexSpecialCharacters))
        {
            var error = ErrorDescription.Error["E0035"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsMobilePhoneExists(request.MobilePhone, false, 0))
        {
            var error = ErrorDescription.Error["E0020"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.MobilePhone!, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E0030"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailExists(request.Email, false, 0))
        {
            var error = ErrorDescription.Error["E0021"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // if (!Regex.IsMatch(request.Email!, StringConstant.RegexEmail))
        // {
        //     var error = ErrorDescription.Error["E0031"];
        //     return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        // }

        if (IsEmailOrganizationExists(request.EmailOrganization, false, 0))
        {
            var error = ErrorDescription.Error["E0022"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // if (!Regex.IsMatch(request.EmailOrganization!, StringConstant.RegexEmail))
        // {
        //     var error = ErrorDescription.Error["E0032"];
        //     return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        // }

        if (IsCitizenIdentityCardNoExists(request.CitizenIdentityCardNo, false, 0))
        {
            var error = ErrorDescription.Error["E0027"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.CitizenIdentityCardNo!, StringConstant.RegexCitizenIdCardNo))
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
            FirstName = request.FirstName!,
            LastName = request.LastName!,
            Avatar = request.Avatar,
            MobilePhone = request.MobilePhone!,
            Email = request.Email!,
            EmailOrganization = request.EmailOrganization!,
            Birthday = request.Birthday,
            CitizenIdentityCardNo = request.CitizenIdentityCardNo!,
            CitizenIdentityCardPublishedDate = request.CitizenIdentityCardPublishedDate,
            CitizenIdentityCardPublishedPlace = request.CitizenIdentityCardPublishedPlace!,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

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
        catch (Exception e)
        {
            var error = ErrorDescription.Error["E0037"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var sroResponse = GetSroResponse(sro.UserId);
        return Ok(CustomResponse.Ok("Create SRO successfully", sroResponse));
    }

    // update sro
    [HttpPut]
    [Route("api/sros/{id:int}")]
    [Authorize(Roles = "admin")]
    public IActionResult UpdateSro([FromRoute] int id, [FromBody] UpdateSroRequest request)
    {
        request.FirstName = Regex.Replace(request.FirstName!, StringConstant.RegexWhiteSpaces, " ");
        // function replace string ex: H ' Hen Nie => H'Hen Nie
        request.FirstName = request.FirstName.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.FirstName, RegexSpecialCharacters))
        {
            var error = ErrorDescription.Error["E0034"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        request.LastName = Regex.Replace(request.LastName!, StringConstant.RegexWhiteSpaces, " ");
        request.LastName = request.LastName.Replace(" ' ", "'").Trim();

        if (Regex.IsMatch(request.LastName, RegexSpecialCharacters))
        {
            var error = ErrorDescription.Error["E0035"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsMobilePhoneExists(request.MobilePhone, true, id))
        {
            var error = ErrorDescription.Error["E0020"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (!Regex.IsMatch(request.MobilePhone!, StringConstant.RegexMobilePhone))
        {
            var error = ErrorDescription.Error["E0030"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsEmailExists(request.Email, true, id))
        {
            var error = ErrorDescription.Error["E0021"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // if (!Regex.IsMatch(request.Email!, StringConstant.RegexEmail))
        // {
        //     var error = ErrorDescription.Error["E0031"];
        //     return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        // }

        if (IsEmailOrganizationExists(request.EmailOrganization, true, id))
        {
            var error = ErrorDescription.Error["E0022"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // if (!Regex.IsMatch(request.EmailOrganization!, StringConstant.RegexEmail))
        // {
        //     var error = ErrorDescription.Error["E0032"];
        //     return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        // }

        if (!Regex.IsMatch(request.CitizenIdentityCardNo!, StringConstant.RegexCitizenIdCardNo))
        {
            var error = ErrorDescription.Error["E0033"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

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

        user.ProvinceId = request.ProvinceId;
        user.DistrictId = request.DistrictId;
        user.WardId = request.WardId;
        user.CenterId = request.CenterId;
        user.GenderId = request.GenderId;
        user.RoleId = SroRoleId;
        user.FirstName = request.FirstName!;
        user.LastName = request.LastName!;
        user.Avatar = request.Avatar;
        user.MobilePhone = request.MobilePhone!;
        user.Email = request.Email!;
        user.EmailOrganization = request.EmailOrganization!;
        user.Birthday = request.Birthday;
        user.CitizenIdentityCardNo = request.CitizenIdentityCardNo!;
        user.CitizenIdentityCardPublishedDate = request.CitizenIdentityCardPublishedDate;
        user.CitizenIdentityCardPublishedPlace = request.CitizenIdentityCardPublishedPlace!;
        user.UpdatedAt = DateTime.Now;
        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            var error = ErrorDescription.Error["E0037"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var sroResponse = GetSroResponse(sro.UserId);
        return Ok(CustomResponse.Ok("Update SRO successfully", sroResponse));
    }

    private bool IsCenterExists(int centerId)
    {
        return _context.Centers.Any(e => e.Id == centerId);
    }

    private bool IsSroExists(int id)
    {
        return _context.Users.Any(e => e.Id == id && e.RoleId == SroRoleId);
    }

    private bool IsMobilePhoneExists(string? mobilePhone, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e => e.MobilePhone == mobilePhone && e.Id != userId)
            : _context.Users.Any(e => e.MobilePhone == mobilePhone && e.RoleId == SroRoleId);
    }

    private bool IsEmailExists(string? email, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e => e.Email == email && e.Id != userId)
            : _context.Users.Any(e => e.Email == email && e.RoleId == SroRoleId);
    }


    private bool IsEmailOrganizationExists(string? emailOrganization, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e => e.EmailOrganization == emailOrganization && e.Id != userId)
            : _context.Users.Any(e => e.EmailOrganization == emailOrganization && e.RoleId == SroRoleId);
    }

    private bool IsCitizenIdentityCardNoExists(string? citizenIdentityCardNo, bool isUpdate, int userId)
    {
        return isUpdate
            ? _context.Users.Any(e => e.CitizenIdentityCardNo == citizenIdentityCardNo && e.Id != userId)
            : _context.Users.Any(e => e.CitizenIdentityCardNo == citizenIdentityCardNo && e.RoleId == SroRoleId);
    }

    private IQueryable<SroResponse> GetSroResponse(int id)
    {
        return _context.Users
            .Include(u => u.Province)
            .Include(u => u.District)
            .Include(u => u.Ward)
            .Include(u => u.Role)
            .Include(u => u.Gender)
            .Include(u => u.Center)
            .Include(u => u.Sro)
            .Where(u => u.Sro.UserId == id && u.RoleId == SroRoleId)
            .Select(u => new SroResponse()
            {
                UserId = u.Id,
                Role = new RoleResponse()
                {
                    RoleId = u.Role.Id,
                    RoleValue = u.Role.Value
                },
                FirstName = u.FirstName,
                LastName = u.LastName,
                MobilePhone = u.MobilePhone,
                Email = u.Email,
                EmailOrganization = u.EmailOrganization,
                Avatar = u.Avatar,
                Province = new ProvinceResponse()
                {
                    Id = u.Province.Id,
                    Code = u.Province.Code,
                    Name = u.Province.Name
                },
                District = new DistrictResponse()
                {
                    Id = u.District.Id,
                    Name = u.District.Name,
                    Prefix = u.District.Prefix
                },
                Ward = new WardResponse()
                {
                    Id = u.Ward.Id,
                    Name = u.Ward.Name,
                    Prefix = u.Ward.Prefix
                },
                Gender = new GenderResponse()
                {
                    Id = u.Gender.Id,
                    Value = u.Gender.Value
                },
                Birthday = u.Birthday,
                CenterId = u.CenterId,
                CenterName = u.Center.Name,
                CitizenIdentityCardNo = u.CitizenIdentityCardNo,
                CitizenIdentityCardPublishedDate = u.CitizenIdentityCardPublishedDate,
                CitizenIdentityCardPublishedPlace = u.CitizenIdentityCardPublishedPlace,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            });
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