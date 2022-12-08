using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.SessionTypeController.SessionTypeModel;

namespace AcademicManagementSystem.Models.SessionController;

public class DetailSessionForTeacherResponse
{
    [JsonPropertyName("session_id")]
    public int SessionId { get; set; }
    
    [JsonPropertyName("session_title")]
    public string SessionTitle { get; set; }
    
    [JsonPropertyName("start_time")]
    public TimeSpan StartTime { get; set; }
    
    [JsonPropertyName("end_time")]
    public TimeSpan EndTime { get; set; }

    [JsonPropertyName("session_type")]
    public SessionTypeResponse SessionType { get; set; }    
    
    [JsonPropertyName("class")]
    public BasicClassResponse Class { get; set; }
    
    [JsonPropertyName("module")]
    public BasicModuleResponse Module { get; set; }
    
    [JsonPropertyName("room")]
    public BasicRoomResponse Room { get; set; }
}
