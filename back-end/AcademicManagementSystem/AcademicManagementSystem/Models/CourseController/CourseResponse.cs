using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.CourseFamilyController;
using AcademicManagementSystem.Models.CourseModuleSemester;

namespace AcademicManagementSystem.Models.CourseController;

public class CourseResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("course_family_code")]
    public string? CourseFamilyCode { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("semester_count")]
    public int? SemesterCount { get; set; }
    
    [JsonPropertyName("is_active")]
    public bool? IsActive { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
    
    [JsonPropertyName("course_family")]
    public CourseFamilyResponse? CourseFamily { get; set; }
}