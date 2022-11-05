using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.ModuleController;

public class CheckModuleCanDeleteResponse
{
    [JsonPropertyName("can_delete")]
    public bool CanDelete { get; set; }
}