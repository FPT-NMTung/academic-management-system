using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.CourseController;

public class UpdateCourseRequest
{
    [JsonPropertyName("course_family_code")]
    public string CourseFamilyCode { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("semester_count")]
    public int SemesterCount { get; set; }
    
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }
}