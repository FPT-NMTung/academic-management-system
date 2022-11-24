using System.ComponentModel;
using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.AttendanceController.AttendanceModel;

public class StudentAttendanceRequest
{
    [JsonPropertyName("student_id")]
    public int StudentId { get; set; }
    
    // absent id: 2
    [DefaultValue(2)]
    [JsonPropertyName("attendance_status_id")]
    public int AttendanceStatusId { get; set; }
    
    [JsonPropertyName("note")]
    public string? Note { get; set; }
}