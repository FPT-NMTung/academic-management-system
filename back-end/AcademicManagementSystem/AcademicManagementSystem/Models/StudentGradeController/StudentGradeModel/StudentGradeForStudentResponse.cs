using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.StudentGradeController.StudentGradeModel.GradeItem;

namespace AcademicManagementSystem.Models.StudentGradeController.StudentGradeModel;

public class StudentGradeForStudentResponse
{
    [JsonPropertyName("grade_category_id")]
    public int GradeCategoryId { get; set; }
    
    [JsonPropertyName("grade_category_name")]
    public string GradeCategoryName { get; set; }
    
    [JsonPropertyName("total_weight")]
    public int TotalWeight { get; set; }
    
    [JsonPropertyName("quantity_grade_item")]
    public int QuantityGradeItem { get; set; }

    [JsonPropertyName("grade_item")]
    public GradeItemWithStudentScoreResponse GradeItem { get; set; }
}