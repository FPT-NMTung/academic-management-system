using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.CenterController;

public class CenterCheckDeleteResponse
{
    [JsonPropertyName("can_delete")]
    public bool CanDelete { get; set; }
}