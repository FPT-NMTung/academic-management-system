using AcademicManagementSystem.Context;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.GenderController;
using AcademicManagementSystem.Models.RoleController;
using AcademicManagementSystem.Models.TeacherTypeController;
using AcademicManagementSystem.Models.UserController.TeacherController;
using AcademicManagementSystem.Models.WorkingTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class TeacherController : ControllerBase
{
    private readonly AmsContext _context;
    private const int RoleIdTeacher = 3;
    private const string RegexSpecialCharacters = StringConstant.RegexSpecialCharsNotAllowForPersonName;


    public TeacherController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/teachers")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetAllTeachers()
    {
        var allUserRoleTeacher = _context.Users
            .Include(u => u.Province)
            .Include(u => u.District)
            .Include(u => u.Ward)
            .Include(u => u.Role)
            .Include(u => u.Gender)
            .Include(u => u.Center)
            .Include(u => u.Teacher)
                .ThenInclude(t => t.TeacherType)
            .Include(u => u.Teacher)
                .ThenInclude(t => t.WorkingTime)
            .Where(u => u.RoleId == RoleIdTeacher)
            .ToList()
            .Select(u => new TeacherResponse()
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
                TeacherType = new TeacherTypeResponse()
                {
                    Id = u.Teacher.TeacherType.Id,
                    Value = u.Teacher.TeacherType.Value
                },
                WorkingTime = new WorkingTimeResponse()
                {
                    Id = u.Teacher.WorkingTime.Id,
                    Value = u.Teacher.WorkingTime.Value
                },
                Nickname = u.Teacher.Nickname,
                CompanyAddress = u.Teacher.CompanyAddress,
                StartWorkingDate = u.Teacher.StartWorkingDate,
                Salary = u.Teacher.Salary,
                TaxCode = u.Teacher.TaxCode,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            });

        return Ok(CustomResponse.Ok("Get all teachers successfully", allUserRoleTeacher));
    }
}