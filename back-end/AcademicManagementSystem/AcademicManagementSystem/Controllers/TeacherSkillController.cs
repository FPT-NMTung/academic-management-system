using AcademicManagementSystem.Context;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class TeacherSkillController: ControllerBase
{
    private readonly AmsContext _context;
    
    public TeacherSkillController(AmsContext context)
    {
        _context = context;
    }
    
    // [HttpGet]
    // [Route("api/teachers/{id:int}/skills")]
    
}