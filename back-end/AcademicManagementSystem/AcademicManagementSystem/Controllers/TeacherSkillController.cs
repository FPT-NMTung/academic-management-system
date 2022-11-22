using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.TeacherSkillController;
using AcademicManagementSystem.Models.TeacherSkillController.Skill;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class TeacherSkillController : ControllerBase
{
    private readonly AmsContext _context;

    public TeacherSkillController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/skills/{skillId:int}/teachers")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetTeachersHasSkillBySkillId(int skillId)
    {
        var skillExists = _context.Skills.Any(s => s.Id == skillId);

        if (!skillExists)
        {
            return NotFound(CustomResponse.NotFound("Skill not found"));
        }

        var skillTeachers = _context.Skills.Include(s => s.Teachers)
            .Select(s => new SkillTeacherResponse()
            {
                Skill = new SkillResponse()
                {
                    Id = s.Id,
                    Name = s.Name,
                },
                Teachers = s.Teachers.Select(t => new BasicTeacherInformationResponse()
                {
                    Id = t.User.Id,
                    FirstName = t.User.FirstName,
                    LastName = t.User.LastName,
                    EmailOrganization = t.User.EmailOrganization
                }).ToList()
            }).Where(s => s.Skill.Id == skillId).ToList();

        return Ok(CustomResponse.Ok("Get teachers by skill retrieved successfully", skillTeachers));
    }

    [HttpGet]
    [Route("api/teachers/{teacherId:int}/skills")]
    [Authorize(Roles = "admin, sro")]
    public IActionResult GetSkillsOfTeacherByTeacherId(int teacherId)
    {
        var teacherExists = _context.Teachers.Any(t => t.User.Id == teacherId);
        if (!teacherExists)
        {
            return NotFound(CustomResponse.NotFound("Teacher not found"));
        }

        var teacherSkills = GetTeacherSkillResponses(teacherId).ToList();

        return Ok(CustomResponse.Ok("Get skills by teacher retrieved successfully", teacherSkills));
    }

    [HttpPost]
    [Route("api/teachers/{teacherId:int}/skills")]
    [Authorize(Roles = "admin")]
    public IActionResult AddOrUpdateSkillToTeacher(int teacherId, [FromBody] CreateTeacherSkillRequest request)
    {
        // get teacher in database
        var teacher = _context.Teachers.Include(t => t.Skills)
            .FirstOrDefault(t => t.User.Id == teacherId);

        if (teacher == null)
        {
            return NotFound(CustomResponse.NotFound("Teacher not found"));
        }

        var requestSkills = request.Skills;

        // clear skills of teacher
        teacher.Skills.Clear();

        // re-add skills
        if (requestSkills != null && requestSkills.Count != 0)
        {
            foreach (var skill in requestSkills)
            {
                if (string.IsNullOrWhiteSpace(skill.Name))
                {
                    var error = ErrorDescription.Error["E1066_2"];
                    return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                }

                skill.Name = Regex.Replace(skill.Name, StringConstant.RegexWhiteSpaces, " ").Trim();
                skill.Name = skill.Name.Replace(" ' ", "'").Trim();

                if (Regex.IsMatch(skill.Name, StringConstant.RegexSpecialCharacterNotAllowForSkillName))
                {
                    var error = ErrorDescription.Error["E1062"];
                    return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
                }

                var existedSkill = GetSkillByName(skill.Name);

                // add new skill if not exists
                if (existedSkill == null)
                {
                    var newSkill = new Skill()
                    {
                        Name = skill.Name,
                    };

                    _context.Skills.Add(newSkill);
                    teacher.Skills.Add(newSkill);
                    _context.SaveChanges();
                }
                else if (!teacher.Skills.Contains(existedSkill))
                {
                    teacher.Skills.Add(existedSkill);
                }
            }
        }

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            var error = ErrorDescription.Error["E0069_1"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var teacherSkillResponses = GetTeacherSkillResponses(teacherId);

        return Ok(CustomResponse.Ok("Skills added to teacher successfully", teacherSkillResponses));
    }

    private IQueryable<TeacherSkillResponse> GetTeacherSkillResponses(int teacherId)
    {
        return _context.Teachers.Include(t => t.Skills)
            .Select(t => new TeacherSkillResponse()
            {
                Teacher = new BasicTeacherInformationResponse()
                {
                    Id = t.User.Id,
                    FirstName = t.User.FirstName,
                    LastName = t.User.LastName,
                    EmailOrganization = t.User.EmailOrganization
                },
                Skills = t.Skills.Select(s => new SkillResponse()
                {
                    Id = s.Id,
                    Name = s.Name,
                }).ToList()
            }).Where(t => t.Teacher.Id == teacherId);
    }

    private Skill? GetSkillByName(string name)
    {
        return _context.Skills.FirstOrDefault(s => s.Name.ToLower().Equals(name.ToLower()));
    }
}