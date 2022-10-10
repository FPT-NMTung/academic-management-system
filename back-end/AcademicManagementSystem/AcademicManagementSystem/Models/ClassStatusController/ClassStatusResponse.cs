using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.ClassStatusController;

public class ClassStatusResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("Value")]
    public string Value { get; set; }
}