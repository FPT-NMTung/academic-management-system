using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.BasicResponse;

public class BasicModuleResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
}