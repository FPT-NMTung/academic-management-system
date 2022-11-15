using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.DayOffController.DayOffModel;

public class CreateDayOffRequest
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
   
    [JsonPropertyName("teacher_id")]
    public int? TeacherId { get; set; }
    
    [JsonPropertyName("working_time_ids")]
    public int[] WorkingTimeIds { get; set; }
}