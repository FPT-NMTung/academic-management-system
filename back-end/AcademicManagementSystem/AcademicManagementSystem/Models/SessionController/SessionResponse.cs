using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.RoomController.RoomModel;

namespace AcademicManagementSystem.Models.SessionController;

public class SessionResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("learning_date")]
    public DateTime LearningDate { get; set; }
    
    [JsonPropertyName("start_time")]
    public TimeSpan StartTime { get; set; }
    
    [JsonPropertyName("end_time")]
    public TimeSpan EndTime { get; set; }
    
    [JsonPropertyName("room")]
    public RoomResponse Room { get; set; }
    
    [JsonPropertyName("session_type")]
    public int SessionType { get; set; }
}