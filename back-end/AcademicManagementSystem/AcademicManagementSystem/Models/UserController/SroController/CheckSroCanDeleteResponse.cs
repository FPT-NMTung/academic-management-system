using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.UserController.SroController;

public class CheckSroCanDeleteResponse
{
    [JsonPropertyName("can_delete")]
    public bool CanDelete { get; set; }
}