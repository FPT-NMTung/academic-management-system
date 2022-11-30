using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.SessionTypeController.SessionTypeModel;

public class SessionTypeResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; }
}