using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.CourseController;

namespace AcademicManagementSystem.Models.CourseFamilyController;

public class CourseFamilyResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("published_year")]
    public int? PublishedYear { get; set; }
    
    [JsonPropertyName("is_active")]
    public bool? IsActive { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}