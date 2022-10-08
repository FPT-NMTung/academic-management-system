using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.CourseController;
using AcademicManagementSystem.Models.ModuleController;
using AcademicManagementSystem.Models.SemesterController;

namespace AcademicManagementSystem.Models.CourseModuleSemester;

public class CourseModuleSemesterResponse
{
    [JsonPropertyName("course_code")]
    public string? CourseCode { get; set; }
    
    [JsonPropertyName("module_id")]
    public int? ModuleId { get; set; }
    
    [JsonPropertyName("semester_id")]
    public int? SemesterId { get; set; }
    
    [JsonPropertyName("module")]
    public ModuleResponse? Module { get; set; }
    
    [JsonPropertyName("semester")]
    public SemesterResponse? Semester { get; set; }
    
    [JsonPropertyName("course")]
    public CourseResponse? Course { get; set; }
}