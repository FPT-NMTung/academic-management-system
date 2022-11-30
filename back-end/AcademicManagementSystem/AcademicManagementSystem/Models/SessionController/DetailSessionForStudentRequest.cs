
using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.SessionController;

public class DetailSessionForStudentRequest
{
    [JsonPropertyName("learning_date")]
    public DateTime LearningDate { get; set; }
}