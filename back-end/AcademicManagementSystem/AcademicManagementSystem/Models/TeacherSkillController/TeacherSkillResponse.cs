using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.TeacherSkillController.Skill;
using AcademicManagementSystem.Models.UserController.TeacherController;

namespace AcademicManagementSystem.Models.TeacherSkillController;

public class TeacherSkillResponse
{
    [JsonPropertyName("teacher")]
    public BasicTeacherInformationResponse Teacher { get; set; }
    
    [JsonPropertyName("skills")]
    public List<SkillResponse>? Skills { get; set; }
    
}