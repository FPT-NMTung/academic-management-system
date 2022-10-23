using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.TeacherSkillController.Skill;

namespace AcademicManagementSystem.Models.TeacherSkillController;

public class CreateTeacherSkillRequest
{
    [JsonPropertyName("skills")]
    public List<CreateSkillRequest>? Skills { get; set; }
}