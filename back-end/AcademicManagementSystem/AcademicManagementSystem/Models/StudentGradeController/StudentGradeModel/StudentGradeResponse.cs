using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;

namespace AcademicManagementSystem.Models.StudentGradeController.StudentGradeModel;

public class StudentGradeResponse
{
    [JsonPropertyName("class")]
    public BasicClassResponse Class { get; set; }

    [JsonPropertyName("module")]
    public BasicModuleResponse Module { get; set; }

    [JsonPropertyName("student")]
    public StudentInfoAndGradeResponse Student { get; set; }
}

public class StudentInfoAndGradeResponse
{
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
    
    [JsonPropertyName("enroll_number")]
    public string EnrollNumber { get; set; }
    
    [JsonPropertyName("email_organization")]
    public string EmailOrganization { get; set; }
    
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }
    
    [JsonPropertyName("last_name")]
    public string LastName { get; set; }
    
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }
    
    [JsonPropertyName("grade_categories")]
    public List<GradeCategoryWithItemsResponse> GradeCategories { get; set; }
}