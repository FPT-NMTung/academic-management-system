using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.CenterController;
using AcademicManagementSystem.Models.ClassDaysController;
using AcademicManagementSystem.Models.ClassStatusController;
using AcademicManagementSystem.Models.CourseController;
using AcademicManagementSystem.Models.CourseFamilyController;
using AcademicManagementSystem.Models.UserController.SroController;

namespace AcademicManagementSystem.Models.ClassController;

public class ClassResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("center_id")]
    public int CenterId { get; set; }
    
    [JsonPropertyName("course_family_code")]
    public string? CourseFamilyCode{ get; set; }
    
    [JsonPropertyName("class_days_id")]
    public int ClassDaysId { get; set; }
    
    [JsonPropertyName("class_status_id")]
    public int ClassStatusId { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("start_date")]
    public DateTime StartDate { get; set; }
    
    [JsonPropertyName("completion_date")]
    public DateTime CompletionDate { get; set; }
    
    [JsonPropertyName("graduation_date")]
    public DateTime GraduationDate { get; set; }
    
    [JsonPropertyName("class_hour_start")]
    public TimeSpan ClassHourStart { get; set; }
    
    [JsonPropertyName("class_hour_end")]
    public TimeSpan ClassHourEnd { get; set; }
    
    [JsonPropertyName("sro_id")]
    public int SroId{ get; set; }
    
    [JsonPropertyName("sro_first_name")]
    public string? SroFirstName { get; set; }
    
    [JsonPropertyName("sro_last_name")]
    public string? SroLastName { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    [JsonPropertyName("center")]
    public CenterResponse? Center { get; set; }
    
    [JsonPropertyName("course_family")]
    public CourseFamilyResponse? CourseFamily { get; set; }
    
    [JsonPropertyName("class_days")]
    public ClassDaysResponse? ClassDays { get; set; }
    
    [JsonPropertyName("class_status")]
    public ClassStatusResponse? ClassStatus { get; set; }
}