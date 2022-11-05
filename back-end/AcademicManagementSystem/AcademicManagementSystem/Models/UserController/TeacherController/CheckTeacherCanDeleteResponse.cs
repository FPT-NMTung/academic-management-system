using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.UserController.TeacherController;

public class CheckTeacherCanDeleteResponse
{
    [JsonPropertyName("can_delete")]
    public bool CanDelete { get; set; }
}