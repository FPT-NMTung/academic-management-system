using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.RoleController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class RoleController : ControllerBase
{
    private readonly AmsContext _context;

    public RoleController(AmsContext context)
    {
        _context = context;
    }

    //get all roles
    [HttpGet]
    [Route("api/roles")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetRoles()
    {
        var roles = _context.Roles.Select(r => new RoleResponse()
        {
            Id = r.Id,
            Value = r.Value,
        });
        return Ok(CustomResponse.Ok("get all roles successfully", roles));
    }
}