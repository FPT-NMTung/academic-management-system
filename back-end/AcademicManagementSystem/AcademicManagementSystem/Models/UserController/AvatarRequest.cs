using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.UserController;

public class AvatarRequest
{
    [JsonPropertyName("image")]
    public string Image { get; set; }
}