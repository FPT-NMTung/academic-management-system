using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.AuthController.RefreshTokenModel;

public class RefreshTokenRequest
{
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}