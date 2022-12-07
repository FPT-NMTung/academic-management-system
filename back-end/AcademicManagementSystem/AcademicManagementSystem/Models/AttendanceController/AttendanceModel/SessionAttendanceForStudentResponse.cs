using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.AttendanceStatusController.AttendanceStatusModel;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.SessionTypeController.SessionTypeModel;

namespace AcademicManagementSystem.Models.AttendanceController.AttendanceModel;

public class SessionAttendanceForStudentResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("learning_date")]
    public DateTime LearningDate { get; set; }
    
    [JsonPropertyName("start_time")]
    public TimeSpan StartTime { get; set; }
    
    [JsonPropertyName("end_time")]
    public TimeSpan EndTime { get; set; }

    [JsonPropertyName("attendance_status")]
    public AttendanceStatusResponse? AttendanceStatus { get; set; }
    
    [JsonPropertyName("note")]
    public string? Note { get; set; }
    
    [JsonPropertyName("session_type")]
    public SessionTypeResponse SessionType { get; set; }
    
    [JsonPropertyName("room")]
    public BasicRoomResponse Room { get; set; }

    [JsonPropertyName("class")] 
    public BasicClassResponse Class { get; set; }
    
    [JsonPropertyName("module")]
    public BasicModuleResponse Module { get; set; }
    
    [JsonPropertyName("teacher")]
    public BasicTeacherInformationResponse Teacher { get; set; }
}