using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.GpaController;

public class FormResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}