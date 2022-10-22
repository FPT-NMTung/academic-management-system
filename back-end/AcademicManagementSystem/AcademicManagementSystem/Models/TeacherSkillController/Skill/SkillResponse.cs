using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.TeacherSkillController.Skill;

public class SkillResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("teachers")]
    public List<TeacherSkillInformation> Teachers { get; set; }
}

public class TeacherSkillInformation
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }
    
    [JsonPropertyName("last_name")]
    public string LastName { get; set; }
}