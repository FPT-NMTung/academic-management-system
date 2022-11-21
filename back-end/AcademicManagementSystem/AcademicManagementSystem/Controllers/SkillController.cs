using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Handlers;
using AcademicManagementSystem.Models.TeacherSkillController;
using AcademicManagementSystem.Models.TeacherSkillController.Skill;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class SkillController : ControllerBase
{
    private readonly AmsContext _context;

    public SkillController(AmsContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("api/skills")]
    [Authorize(Roles = "admin")]
    public IActionResult GetSkills()
    {
        var skills = _context.Skills.Select(s => new SkillResponse()
        {
            Id = s.Id,
            Name = s.Name
        }).ToList();
        return Ok(CustomResponse.Ok("Skills retrieved successfully", skills));
    }

    // search skill by name
    [HttpGet]
    [Route("api/skills/search")]
    [Authorize(Roles = "admin")]
    public IActionResult SearchSkills([FromQuery] string name)
    {
        var skills = _context.Skills
            .Where(s => s.Name.ToLower().Contains(name.ToLower()))
            .Select(s => new SkillResponse()
            {
                Id = s.Id,
                Name = s.Name
            });
        return Ok(CustomResponse.Ok("Skills searched successfully", skills));
    }

    [HttpPost]
    [Route("api/skills")]
    [Authorize(Roles = "admin")]
    public IActionResult CreateSkill([FromBody] CreateSkillRequest request)
    {
        request.Name = request.Name.Trim();

        //check module name
        request.Name = Regex.Replace(request.Name, StringConstant.RegexWhiteSpaces, " ");
        request.Name = request.Name.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.Name, StringConstant.RegexSpecialCharacterNotAllowForSkillName))
        {
            var error = ErrorDescription.Error["E1062"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Name.Length > 255)
        {
            var error = ErrorDescription.Error["E1063"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        // Check if skill already exists
        if (IsSkillNameExists(request.Name))
        {
            var error = ErrorDescription.Error["E1064"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        var skill = new Skill()
        {
            Name = request.Name
        };
        try
        {
            _context.Skills.Add(skill);
            _context.SaveChanges();
        }
        catch (Exception)
        {
            var error = ErrorDescription.Error["E1066_1"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Skill created successfully", new SkillResponse()
        {
            Id = skill.Id,
            Name = skill.Name
        }));
    }

    // update skill
    [HttpPut]
    [Route("api/skills/{id:int}")]
    [Authorize(Roles = "admin")]
    public IActionResult UpdateSkill([FromRoute] int id, [FromBody] UpdateSkillRequest request)
    {
        request.Name = request.Name.Trim();

        var skill = _context.Skills.FirstOrDefault(s => s.Id == id);
        if (skill == null)
        {
            var error = ErrorDescription.Error["E1065"];
            return NotFound(CustomResponse.NotFound(error.Message));
        }

        //check skill name
        request.Name = Regex.Replace(request.Name, StringConstant.RegexWhiteSpaces, " ");
        request.Name = request.Name.Replace(" ' ", "'").Trim();
        if (Regex.IsMatch(request.Name, StringConstant.RegexSpecialCharacterNotAllowForSkillName))
        {
            var error = ErrorDescription.Error["E1062"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (request.Name.Length > 255)
        {
            var error = ErrorDescription.Error["E1063"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        if (IsSkillNameWithDifferentIdExists(id, request.Name))
        {
            var error = ErrorDescription.Error["E1066"];
            return NotFound(CustomResponse.NotFound(error.Message));
        }

        try
        {
            skill.Name = request.Name;
            _context.Skills.Update(skill);
            _context.SaveChanges();
        }
        catch (Exception)
        {
            var error = ErrorDescription.Error["E1066_1"];
            return BadRequest(CustomResponse.BadRequest(error.Message, error.Type));
        }

        return Ok(CustomResponse.Ok("Skill updated successfully", new SkillResponse()
        {
            Id = skill.Id,
            Name = skill.Name
        }));
    }

    // is skill name exists
    private bool IsSkillNameExists(string name)
    {
        return _context.Skills.Any(s => s.Name.ToLower().Equals(name.ToLower()));
    }

    // is skill name with different id exists
    private bool IsSkillNameWithDifferentIdExists(int id, string name)
    {
        return _context.Skills.Any(s => string.Equals(s.Name.ToLower(), name.ToLower()) && s.Id != id);
    }
}