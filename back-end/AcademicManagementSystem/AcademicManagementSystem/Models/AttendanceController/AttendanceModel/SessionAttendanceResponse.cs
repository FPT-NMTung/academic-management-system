using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.AttendanceStatusController.AttendanceStatusModel;
using AcademicManagementSystem.Models.BasicResponse;

namespace AcademicManagementSystem.Models.AttendanceController.AttendanceModel;

public class SessionAttendanceResponse
{
    [JsonPropertyName("id")]
    public int SessionId { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("learning_date")]
    public DateTime LearningDate { get; set; }
    
    [JsonPropertyName("class_schedule")]
    public BasicClassScheduleResponse ClassSchedule { get; set; }
    
    [JsonPropertyName("attendances")]
    public List<StudentAttendanceResponse> StudentAttendances { get; set; }
}

public class StudentAttendanceResponse
{
    [JsonPropertyName("student")]
    public BasicStudentResponse Student { get; set; }
    
    [JsonPropertyName("attendance_status")]
    public AttendanceStatusResponse? AttendanceStatus { get; set; }
    
    [JsonPropertyName("note")]
    public string? Note { get; set; }
}
