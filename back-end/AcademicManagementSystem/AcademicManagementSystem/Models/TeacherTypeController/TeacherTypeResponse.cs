using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.TeacherTypeController;

public class TeacherTypeResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; }
}