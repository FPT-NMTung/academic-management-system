using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.BasicResponse;

public class BasicClassScheduleResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("class")] 
    public BasicClassResponse Class { get; set; }
    
    [JsonPropertyName("module")]
    public BasicModuleResponse Module { get; set; }
}