using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.TeacherSkillController.Skill;

public class SkillResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
}