using AcademicManagementSystem.Context;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.TeacherSkillController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
public class TeacherSkillController : ControllerBase
{
    private readonly AmsContext _context;

    public TeacherSkillController(AmsContext context)
    {
        _context = context;
    }

    // get all teacher with skills
    [HttpGet]
    [Route("api/teacher-skills")]
    public IActionResult GetTeacherSkills()
    {
        var teacherSkills = _context.TeachersSkills.Include(ts => ts.Teacher)
            .Include(ts => ts.Skill)
            .Include(ts => ts.Teacher.User)
            .Select(ts => new TeacherSkillResponse()
            {
                TeacherId = ts.TeacherId, SkillId = ts.SkillId,
                TeacherName = ts.Teacher.User.FirstName + " " + ts.Teacher.User.LastName, SkillName = ts.Skill.Name
            })
            .ToList();
        return Ok(CustomResponse.Ok("Get all teachers skills successfully", teacherSkills));
    }

    // get skill of teacher id
    [HttpGet]
    [Route("api/teachers/{id:int}/skills")]
    public IActionResult GetSkillsByTeacherId(int id)
    {
        var teacherSkills = _context.TeachersSkills.Include(ts => ts.Teacher)
            .Include(ts => ts.Skill)
            .Include(ts => ts.Teacher.User)
            .Where(ts => ts.TeacherId == id)
            .Select(ts => new TeacherSkillResponse()
            {
                TeacherId = ts.TeacherId, SkillId = ts.SkillId,
                TeacherName = ts.Teacher.User.FirstName + " " + ts.Teacher.User.LastName, SkillName = ts.Skill.Name
            })
            .ToList();
        if (teacherSkills.Any())
        {
            return Ok(CustomResponse.Ok("Get teacher skills by TeacherId successfully", teacherSkills));
        }

        var error = ErrorDescription.Error["E1067"];
        return NotFound(CustomResponse.NotFound(error.Message));
    }

    // get teacher have skill id
    [HttpGet]
    [Route("api/skills/{skillId:int}/teachers")]
    public IActionResult GetSkillsBySkillId(int skillId)
    {
        var teacherSkills = _context.TeachersSkills.Include(ts => ts.Teacher)
            .Include(ts => ts.Skill)
            .Include(ts => ts.Teacher.User)
            .Where(ts => ts.SkillId == skillId)
            .Select(ts => new TeacherSkillResponse()
            {
                TeacherId = ts.TeacherId, SkillId = ts.SkillId,
                TeacherName = ts.Teacher.User.FirstName + " " + ts.Teacher.User.LastName, SkillName = ts.Skill.Name
            })
            .ToList();
        if (teacherSkills.Any())
        {
            return Ok(CustomResponse.Ok("Get teacher skills by SkillId successfully", teacherSkills));
        }

        var error = ErrorDescription.Error["E1068"];
        return NotFound(CustomResponse.NotFound(error.Message));
    }
}