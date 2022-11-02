using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.CourseController;

public class CheckCourseCanDeleteResponse
{
    [JsonPropertyName("can_delete")]
    public bool CanDelete { get; set; }
}