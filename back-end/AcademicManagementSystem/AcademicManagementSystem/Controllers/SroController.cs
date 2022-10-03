using AcademicManagementSystem.Context;
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
    private const int SroId = 2;

    public SroController(AmsContext context)
    {
        _context = context;
    }

    //get all sro
    [HttpGet]
    [Route("api/all-sro")]
    [Authorize(Roles = "admin, sro, teacher, student")]
    public IActionResult GetAllSro()
    {
        var allSro = _context.Users
            .Include(u => u.Province)
            .Include(u => u.District)
            .Include(u => u.Ward)
            .Include(u => u.Role)
            .Include(u => u.Gender)
            .Include(u => u.Center)
            .Where(u => u.RoleId == SroId)
            .ToList()
            .Select(s => new SroResponse()
            {
                Id = s.Id,
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
                Birthday = s.Birthday.ToString("dd/MM/yyyy"),
                CenterId = s.CenterId,
                CenterName = s.Center.Name,
                CitizenIdentityCardNo = s.CitizenIdentityCardNo,
                CitizenIdentityCardPublishedDate = s.CitizenIdentityCardPublishedDate,
                CitizenIdentityCardPublishedPlace = s.CitizenIdentityCardPublishedPlace
            });

        return Ok(CustomResponse.Ok("Get All Sro successfully", allSro));
    }

    //get all sro by centerId
    [HttpGet]
    [Route("api/center/{centerId:int}/list-sro")]
    [Authorize(Roles = "admin, sro, teacher, student")]
    public IActionResult GetAllSroByCenterId(int centerId)
    {
        if (!IsCenterExists(centerId))
        {
            var error = ErrorDescription.Error["E0016"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));        }

        var allSro = _context.Users
            .Include(u => u.Province)
            .Include(u => u.District)
            .Include(u => u.Ward)
            .Include(u => u.Role)
            .Include(u => u.Gender)
            .Include(u => u.Center)
            .Where(u => u.RoleId == SroId && u.CenterId == centerId)
            .ToList()
            .Where(s => s.CenterId == centerId)
            .Select(s => new SroResponse()
            {
                Id = s.Id,
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
                Birthday = s.Birthday.ToString("dd/MM/yyyy"),
                CenterId = s.CenterId,
                CenterName = s.Center.Name,
                CitizenIdentityCardNo = s.CitizenIdentityCardNo,
                CitizenIdentityCardPublishedDate = s.CitizenIdentityCardPublishedDate,
                CitizenIdentityCardPublishedPlace = s.CitizenIdentityCardPublishedPlace
            });

        return Ok(!allSro.Any()
            ? CustomResponse.Ok("This center don't have any SRO", allSro)
            : CustomResponse.Ok("Get All Sro By centerId successfully", allSro));
    }

    //get sro by id
    [HttpGet]
    [Route("api/sro/{id:int}")]
    [Authorize(Roles = "admin, sro, teacher, student")]
    public IActionResult GetSroById(int id)
    {
        
        if (!IsSroExists(id))
        {
            var error = ErrorDescription.Error["E0017"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }
        
        var sro = _context.Users
            .Include(u => u.Province)
            .Include(u => u.District)
            .Include(u => u.Ward)
            .Include(u => u.Role)
            .Include(u => u.Gender)
            .Include(u => u.Center)
            .Include(u => u.Sro)
            .Where(u => u.Sro.UserId == id && u.RoleId == SroId)
            .Select(u => new SroResponse()
            {
                Id = u.Id,
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
                Birthday = u.Birthday.ToString("dd/MM/yyyy"),
                CenterId = u.CenterId,
                CenterName = u.Center.Name,
                CitizenIdentityCardNo = u.CitizenIdentityCardNo,
                CitizenIdentityCardPublishedDate = u.CitizenIdentityCardPublishedDate,
                CitizenIdentityCardPublishedPlace = u.CitizenIdentityCardPublishedPlace
            });

        return Ok(CustomResponse.Ok("Get sro by Id successfully", sro));
    }

    private bool IsCenterExists(int centerId)
    {
        return _context.Centers.Any(e => e.Id == centerId);
    }
    
    private bool IsSroExists(int sroId)
    {
        return _context.Users.Any(e => e.Id == sroId && e.RoleId == SroId);
    }
}