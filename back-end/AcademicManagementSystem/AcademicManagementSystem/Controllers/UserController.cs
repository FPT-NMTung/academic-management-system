using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.GenderController;
using AcademicManagementSystem.Models.RoleController;
using AcademicManagementSystem.Models.TeacherTypeController;
using AcademicManagementSystem.Models.UserController;
using AcademicManagementSystem.Models.WorkingTime;
using AcademicManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly AmsContext _context;
    private readonly IUserService _userService;

    public UserController(AmsContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpGet]
    [Route("api/users/information")]
    [Authorize(Roles = "admin, sro, teacher, student")]
    public IActionResult GetUsersInformation()
    {
        var id = _userService.GetUserId();
        var user = getAllUsers().FirstOrDefault(u => u.UserId.ToString() == id);
        return Ok(CustomResponse.Ok("Get user by id successfully", user));
    }

    private IQueryable<UserResponse> getAllUsers()
    {
        return _context.Users.Select(u => new UserResponse()
        {
            UserId = u.Id,
            Role = new RoleResponse()
            {
                Id = u.Role.Id,
                Value = u.Role.Value
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
}