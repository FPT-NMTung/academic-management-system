using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.TeacherSkillController.Skill;
using AcademicManagementSystem.Models.UserController.TeacherController;

namespace AcademicManagementSystem.Models.TeacherSkillController;

public class TeacherSkillResponse
{
    [JsonPropertyName("teacher_id")]
    public int TeacherId { get; set; }
    
    [JsonPropertyName("skill_id")]
    public int SkillId { get; set; }
    
    [JsonPropertyName("teacher")]
    public TeacherResponse? Teacher { get; set; }
    
    [JsonPropertyName("skill")]
    public SkillResponse? Skill { get; set; }
}