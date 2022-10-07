using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.CourseController;

namespace AcademicManagementSystem.Models.CourseModuleSemester;

public class CourseModuleSemesterResponse
{
    [JsonPropertyName("course_code")]
    public string? CourseCode { get; set; }
    
    [JsonPropertyName("module_id")]
    public int? ModuleId { get; set; }
    
    [JsonPropertyName("semester_id")]
    public int? SemesterId { get; set; }
}