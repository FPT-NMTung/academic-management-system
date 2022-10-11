using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.GradeCategoryModule;

namespace AcademicManagementSystem.Models.GradeItemController;

public class GradeItemResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }

}