using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.GradeCategoryController;
using AcademicManagementSystem.Models.GradeItemController;

namespace AcademicManagementSystem.Models.GradeCategoryModule;

public class GradeCategoryModuleResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("grade_category")]
    public GradeCategoryResponse GradeCategory { get; set; }
    
    [JsonPropertyName("total_weight")]
    public int TotalWeight { get; set; }
    
    [JsonPropertyName("quantity_grade_item")]
    public int QuantityGradeItem { get; set; }
    
    [JsonPropertyName("grade_items")]
    public List<GradeItemResponse> GradeItems { get; set; }
}