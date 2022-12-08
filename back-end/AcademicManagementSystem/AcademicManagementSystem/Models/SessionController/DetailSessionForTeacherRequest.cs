using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.SessionController;

public class DetailSessionForTeacherRequest
{
    [JsonPropertyName("teach_date")]
    public DateTime TeachDate { get; set; }
}