using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models;

public class LoginRequest
{
    [JsonPropertyName("token_google")]
    public string TokenGoogle { get; set; }
}