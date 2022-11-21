using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.TeacherSkillController;
using AcademicManagementSystem.Models.WorkingTime;

namespace AcademicManagementSystem.Models.DayOffController.DayOffModel;

public class DayOffResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
    
    [JsonPropertyName("working_time_id")]
    public int WorkingTimeId { get; set; }
    
    [JsonPropertyName("teacher")]
    public BasicTeacherInformationResponse Teacher { get; set; }
    
    
}