using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.GradeItemController;

namespace AcademicManagementSystem.Models.StudentGradeController.StudentGradeModel.GradeItem;

public class GradeItemWithStudentScoreResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("grade")]
    public double? Grade { get; set; }
    
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}