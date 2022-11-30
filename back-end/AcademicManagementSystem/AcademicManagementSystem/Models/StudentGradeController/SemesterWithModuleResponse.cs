using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;

namespace AcademicManagementSystem.Models.StudentGradeController;

public class SemesterWithModuleResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("modules")]
    public List<ModuleWithClassResponse> Modules { get; set; }
}