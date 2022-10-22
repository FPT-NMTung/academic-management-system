using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.TeacherSkillController.Skill;

public class UpdateSkillRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}