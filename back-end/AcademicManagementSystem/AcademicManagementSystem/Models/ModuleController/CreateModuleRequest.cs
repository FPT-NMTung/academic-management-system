using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.ModuleController;

public class CreateModuleRequest
{
    [JsonPropertyName("center_id")]
    public int CenterId { get; set; }
    
    [JsonPropertyName("semester_name_portal")]
    public string SemesterNamePortal { get; set; }
    
    [JsonPropertyName("module_name")]
    public string ModuleName { get; set; }
    
    [JsonPropertyName("module_exam_name_portal")]
    public string ModuleExamNamePortal { get; set; }
    
    [JsonPropertyName("module_type")]
    public int ModuleType { get; set; }
    
    [JsonPropertyName("max_theory_grade")]
    public int? MaxTheoryGrade { get; set; }
    
    [JsonPropertyName("max_practical_grade")]
    public int? MaxPracticalGrade { get; set; }
    
    [JsonPropertyName("hours")]
    public int Hours { get; set; }
    
    [JsonPropertyName("days")]
    public int Days { get; set; }
    
    [JsonPropertyName("exam_type")]
    public int ExamType { get; set; }
    
    [JsonPropertyName("course_code")]
    public List<string> CourseCode { get; set; }
    
    [JsonPropertyName("semester_id")]
    public int SemesterId { get; set; }
}
