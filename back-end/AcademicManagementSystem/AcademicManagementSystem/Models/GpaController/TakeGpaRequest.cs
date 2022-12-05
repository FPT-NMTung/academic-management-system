using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.GpaController;

public class TakeGpaRequest
{
    [JsonPropertyName("class_id")]
    public int ClassId { get; set; }
    
    [JsonPropertyName("teacher_id")]
    public int TeacherId { get; set; }
    
    [JsonPropertyName("module_id")]
    public int ModuleId { get; set; }
    
    [JsonPropertyName("session_id")]
    public int SessionId { get; set; }
    
    [JsonPropertyName("student_id")]
    public int StudentId { get; set; }
    
    [JsonPropertyName("form_id")]
    public int FormId { get; set; }
    
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
    
    [JsonPropertyName("answer_id")]
    public int AnswerId { get; set; }
}