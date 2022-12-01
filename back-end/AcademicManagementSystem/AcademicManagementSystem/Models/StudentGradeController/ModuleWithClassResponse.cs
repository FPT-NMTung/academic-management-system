using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;

namespace AcademicManagementSystem.Models.StudentGradeController;

public class ModuleWithClassResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("class")]
    public BasicClassResponse Class { get; set; }
}