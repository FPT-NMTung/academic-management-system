using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.GpaController;

public class ModuleGpaResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("module_name")]
    public string? ModuleName { get; set; }
}