using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;

namespace AcademicManagementSystem.Models.SessionController;

public class SessionDuplicateTeacherResponse
{
    [JsonPropertyName("learning_date")]
    public DateTime LearningDate { get; set; }
    
    [JsonPropertyName("working_time_id")]
    public int WorkingTimeId { get; set; }
    
    [JsonPropertyName("teacher")]
    public BasicTeacherInformationResponse Teacher { get; set; }
    
    [JsonPropertyName("sessions")]
    public List<BasicSessionDuplicateResponse> Sessions { get; set; }
    
}