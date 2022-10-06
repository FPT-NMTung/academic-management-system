using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.RoleController;

public class RoleResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; }
}