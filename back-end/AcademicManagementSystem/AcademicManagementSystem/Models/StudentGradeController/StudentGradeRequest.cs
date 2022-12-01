using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.StudentGradeController;

public class StudentGradeRequest
{
    [JsonPropertyName("student_id")]
    public int StudentId { get; set; }
    
    [JsonPropertyName("grade_item_id")]
    public int GradeItemId { get; set; }
    
    [JsonPropertyName("grade")]
    public double? Grade { get; set; }
    
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}