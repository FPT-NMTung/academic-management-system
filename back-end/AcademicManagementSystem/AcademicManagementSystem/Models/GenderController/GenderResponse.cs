using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.GenderController;

public class GenderResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; }
}