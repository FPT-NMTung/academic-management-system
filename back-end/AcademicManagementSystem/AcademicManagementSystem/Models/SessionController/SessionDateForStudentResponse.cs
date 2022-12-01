using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.SessionController;

public class SessionDateForStudentResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("learning_date")]
    public DateTime LearningDate { get; set; }

    [JsonPropertyName("session_type")]
    public int SessionType { get; set; }
}