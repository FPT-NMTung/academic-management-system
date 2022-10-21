using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.ClassController;

public class UpdateClassRequest
{
    [JsonPropertyName("course_family_code")]
    public string CourseFamilyCode{ get; set; }
    
    [JsonPropertyName("class_days_id")]
    public int ClassDaysId { get; set; }
    
    [JsonPropertyName("class_status_id")]
    public int ClassStatusId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
    
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
}