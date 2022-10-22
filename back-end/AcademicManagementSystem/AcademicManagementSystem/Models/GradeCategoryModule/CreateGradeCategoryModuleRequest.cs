using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.GradeCategoryModule;

public class CreateGradeCategoryModuleRequest
{
    [JsonPropertyName("grade_category_details")]
    public List<CreateGradeCategoryDetailRequest>? GradeCategoryDetails { get; set; }
}

public class CreateGradeCategoryDetailRequest
{
    [JsonPropertyName("grade_category_id")]
    public int GradeCategoryId { get; set; }
    
    [JsonPropertyName("total_weight")]
    public int TotalWeight { get; set; }
    
    [JsonPropertyName("quantity_grade_item")]
    public int QuantityGradeItem { get; set; }
}