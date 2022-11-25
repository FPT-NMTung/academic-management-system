using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.GpaController;

public class AnswerResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("question_id")]
    public int QuestionId { get; set; }
    
    [JsonPropertyName("answer_no")]
    public int AnswerNo { get; set; }
    
    [JsonPropertyName("content")]
    public string Content { get; set; }
}