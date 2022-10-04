using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.RoleController;

public class RoleResponse
{
    [JsonPropertyName("role_id")]
    public int RoleId { get; set; }
    
    [JsonPropertyName("role_value")]
    public string RoleValue { get; set; }
}