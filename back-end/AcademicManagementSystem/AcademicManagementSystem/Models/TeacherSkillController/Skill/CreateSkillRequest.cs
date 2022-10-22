using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.TeacherSkillController.Skill;

public class CreateSkillRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}