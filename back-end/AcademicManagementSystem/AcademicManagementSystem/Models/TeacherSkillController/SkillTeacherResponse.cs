using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.TeacherSkillController.Skill;

namespace AcademicManagementSystem.Models.TeacherSkillController;

public class SkillTeacherResponse
{
    [JsonPropertyName("skill")]
    public SkillResponse Skill { get; set; }
    
    [JsonPropertyName("teachers")]
    public List<BasicTeacherInformationResponse>? Teachers { get; set; }
}