using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.ModuleController;

namespace AcademicManagementSystem.Models.ClassController;

public class ModuleStatusResponse
{
    [JsonPropertyName("module")]
    public ModuleResponse Module { get; set; }
    
    [JsonPropertyName("schedule_id")]
    public int ScheduleId { get; set; }
    
    [JsonPropertyName("schedule_start_time")]
    public DateTime? ScheduleStartTime { get; set; }
    
    [JsonPropertyName("status")]
    public bool Status { get; set; }
}