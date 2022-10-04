using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.CourseFamilyController;

public class CreateCourseFamilyRequest
{
    [JsonPropertyName("code")]
    public string Code { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("published_year")]
    public int PublishedYear { get; set; }
    
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }
}