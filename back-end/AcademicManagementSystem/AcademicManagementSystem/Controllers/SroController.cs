using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.UserController.SroController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class SroController : ControllerBase
{
    private readonly AmsContext _context;

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
        var listSro = _context.Sros.ToList().Select(s => new SroResponse()
        {
            //create new object SroResponse and fill it with data from database 
            Id = s.UserId,
            RoleId = _context.Users.FirstOrDefault(u => u.Id == s.UserId)!.RoleId,
            RoleValue = _context.Roles.FirstOrDefault(r => r.Id == _context.Users
                .FirstOrDefault(u => u.Id == s.UserId)!.RoleId)!.Value,
            FirstName = _context.Users.FirstOrDefault(u => u.Id == s.UserId)!.FirstName,
            LastName = _context.Users.FirstOrDefault(u => u.Id == s.UserId)!.LastName,
            MobilePhone = _context.Users.FirstOrDefault(u => u.Id == s.UserId)!.MobilePhone,
            Email = _context.Users.FirstOrDefault(u => u.Id == s.UserId)!.Email,
            EmailOrganization = _context.Users.FirstOrDefault(u => u.Id == s.UserId)!.EmailOrganization,
            Avatar = _context.Users.FirstOrDefault(u => u.Id == s.UserId)!.Avatar,
            ProvinceId = _context.Users.FirstOrDefault(i => i.Id == s.UserId)!.ProvinceId,
            ProvinceCode = _context.Provinces.FirstOrDefault(p => p.Id == _context.Users
                .FirstOrDefault(u => u.Id == s.UserId)!.ProvinceId)!.Code,
            ProvinceName = _context.Provinces.FirstOrDefault(p => p.Id == _context.Users
                .FirstOrDefault(u => u.Id == s.UserId)!.ProvinceId)!.Name,
            DistrictId = _context.Users.FirstOrDefault(u => u.Id == s.UserId)!.DistrictId,
            DistrictName = _context.Districts.FirstOrDefault(d => d.Id == _context.Users
                .FirstOrDefault(u => u.Id == s.UserId)!.DistrictId)!.Name,
            DistrictPrefix = _context.Districts.FirstOrDefault(d => d.Id == _context.Users
                .FirstOrDefault(u => u.Id == s.UserId)!.DistrictId)!.Prefix,
            WardId = _context.Users.FirstOrDefault(u => u.Id == s.UserId)!.WardId,
            WardName = _context.Wards.FirstOrDefault(w => w.Id == _context.Users
                .FirstOrDefault(u => u.Id == s.UserId)!.WardId)!.Name,
            WardPrefix = _context.Wards.FirstOrDefault(w => w.Id == _context.Users
                .FirstOrDefault(u => u.Id == s.UserId)!.WardId)!.Prefix,
            GenderId = _context.Users.FirstOrDefault(u => u.Id == s.UserId)!.GenderId,
            GenderValue = _context.Genders.FirstOrDefault(g => g.Id == _context.Users
                .FirstOrDefault(u => u.Id == s.UserId)!.GenderId)!.Value,
            Birthday = _context.Users.FirstOrDefault(u => u.Id == s.UserId)!.Birthday,
            CenterId = _context.Users.FirstOrDefault(u => u.Id == s.UserId)!.CenterId,
            CenterName = _context.Centers.FirstOrDefault(c => c.Id == _context.Users
                .FirstOrDefault(u => u.Id == s.UserId)!.CenterId)!.Name,
            CitizenIdentityCardNo = _context.Users.FirstOrDefault(u => u.Id == s.UserId)!.CitizenIdentityCardNo,
            CitizenIdentityCardPublishedDate = _context.Users.FirstOrDefault(u => u.Id == s.UserId)!.CitizenIdentityCardPublishedDate,
            CitizenIdentityCardPublishedPlace = _context.Users.FirstOrDefault(u => u.Id == s.UserId)!.CitizenIdentityCardPublishedPlace,
        });
        return Ok(CustomResponse.Ok("Get All Sro Success", listSro));
    }

    //get all sro by centerId
    [HttpGet]
    [Route("api/center/{centerId:int}/list-sro")]
    public IActionResult Get(int centerId)
    {
        return Ok("Hello World");
    }

    private bool IsCenterExists(int centerId)
    {
        return _context.Centers.Any(e => e.Id == centerId);
    }
}