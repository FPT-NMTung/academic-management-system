using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.GpaController;

public class ClassGpaResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}