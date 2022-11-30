using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.RoomController.RoomModel;

namespace AcademicManagementSystem.Models.SessionController;

public class SessionDuplicateRoomResponse
{
    [JsonPropertyName("learning_date")]
    public DateTime LearningDate { get; set; }
    
    [JsonPropertyName("working_time_id")]
    public int WorkingTimeId { get; set; }
    
    [JsonPropertyName("room")]
    public RoomResponse Room { get; set; }
    
    [JsonPropertyName("sessions")]
    public List<BasicSessionDuplicateResponse> Sessions { get; set; }
}