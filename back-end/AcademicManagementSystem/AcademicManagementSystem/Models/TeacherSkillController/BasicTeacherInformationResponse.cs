using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.TeacherSkillController;

public class BasicTeacherInformationResponse
{
    [JsonPropertyName("id")] 
    public int Id { get; set; }

    [JsonPropertyName("first_name")] 
    public string FirstName { get; set; }

    [JsonPropertyName("last_name")] 
    public string LastName { get; set; }
}