using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.CourseFamilyController;

public class CheckCourseFamilyCanDeleteResponse
{
    [JsonPropertyName("can_delete")]
    public bool CanDelete { get; set; }
}