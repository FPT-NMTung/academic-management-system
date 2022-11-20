using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.DayOffController.DayOffModel;

public class DetailDayOffResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("teacher_id")]
    public int? TeacherId { get; set; }
    
    [JsonPropertyName("teacher_first_name")]
    public string? TeacherFirstName { get; set; }
    
    [JsonPropertyName("teacher_last_name")]
    public string? TeacherLastName { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("working_time_id")]
    public int WorkingTimeId { get; set; }
}