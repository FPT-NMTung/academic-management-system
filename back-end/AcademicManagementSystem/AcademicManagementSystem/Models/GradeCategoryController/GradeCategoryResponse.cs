using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.GradeCategoryController;

public class GradeCategoryResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
}