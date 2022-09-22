using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models;

public class LoginRequest
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}