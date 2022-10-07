using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.WorkingTime;

public class WorkingTimeResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; }
}