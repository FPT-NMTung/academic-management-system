using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.AttendanceStatusController.AttendanceStatusModel;

public class AttendanceStatusResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; }
}