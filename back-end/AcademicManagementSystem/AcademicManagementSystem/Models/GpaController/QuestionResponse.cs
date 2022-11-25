using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.GpaController;

public class QuestionResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("content")]
    public string Content { get; set; }
    
    [JsonPropertyName("answer")]
    public List<AnswerResponse> Answer { get; set; }
}