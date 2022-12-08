using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;

namespace AcademicManagementSystem.Models.GpaController;

public class SessionBasicInfoResponse
{
    [JsonPropertyName("session_id")]
    public int SessionId { get; set; }
    
    [JsonPropertyName("session_title")]
    public string SessionTitle { get; set; }
    
    [JsonPropertyName("learning_date")]
    public DateTime LearningDate { get; set; }

    [JsonPropertyName("class")] 
    public BasicClassResponse Class { get; set; }

    [JsonPropertyName("module")]
    public BasicModuleResponse Module { get; set; }
    
    [JsonPropertyName("teacher")]
    public BasicTeacherInformationResponse Teacher { get; set; }
}