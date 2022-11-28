using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.GradeCategoryController;
using AcademicManagementSystem.Models.StudentGradeController.StudentGradeModel.GradeItem;

namespace AcademicManagementSystem.Models.StudentGradeController.StudentGradeModel;

public class GradeCategoryWithItemsResponse
{
    [JsonPropertyName("grade_category")]
    public GradeCategoryResponse GradeCategory { get; set; }
    
    [JsonPropertyName("total_weight")]
    public int TotalWeight { get; set; }
    
    [JsonPropertyName("quantity_grade_item")]
    public int QuantityGradeItem { get; set; }

    [JsonPropertyName("grade_items")]
    public List<GradeItemWithStudentScoreResponse> GradeItems { get; set; }
}