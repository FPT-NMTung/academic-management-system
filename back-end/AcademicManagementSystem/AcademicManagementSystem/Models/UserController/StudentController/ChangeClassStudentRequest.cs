using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.UserController.StudentController;

public class ChangeClassStudentRequest
{
    [JsonPropertyName("current_class_id")]
    public int CurrentClassId { get; set; }
    
    [JsonPropertyName("new_class_id")]
    public int NewClassId { get; set; }
}