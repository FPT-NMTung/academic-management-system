using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models;

public class LoginResponse
{
    [JsonPropertyName("user_id")]
    public int? UserId { get; set; }
    
    [JsonPropertyName("role_id")]
    public int? RoleId { get; set; }
    
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}