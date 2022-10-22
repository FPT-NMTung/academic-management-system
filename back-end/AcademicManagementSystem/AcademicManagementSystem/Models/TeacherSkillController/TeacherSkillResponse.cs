using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.TeacherSkillController.Skill;
using AcademicManagementSystem.Models.UserController.TeacherController;

namespace AcademicManagementSystem.Models.TeacherSkillController;

public class TeacherSkillResponse
{
    [JsonPropertyName("teacher_id")]
    public int TeacherId { get; set; }

    [JsonPropertyName("teacher_name")]
    public string? TeacherName { get; set; }
    
    [JsonPropertyName("skills")]
    public List<SkillResponse> Skills { get; set; }
    
}