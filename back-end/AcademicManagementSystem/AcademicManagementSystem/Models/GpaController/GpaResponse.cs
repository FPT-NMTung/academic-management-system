using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.GpaController;

public class GpaResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("class")] 
    public string Class { get; set; }
    
    [JsonPropertyName("student")]
    public string Student { get; set; }
    
    [JsonPropertyName("teacher")]
    public string Teacher { get; set; }
    
    [JsonPropertyName("module")]
    public string Module { get; set; }
    
    [JsonPropertyName("session")]
    public string Session { get; set; }

    [JsonPropertyName("answers")]
    public List<string> Answers { get; set; }
    
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}