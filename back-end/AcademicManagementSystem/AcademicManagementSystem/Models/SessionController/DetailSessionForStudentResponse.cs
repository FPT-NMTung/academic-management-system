using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.AttendanceController.AttendanceModel;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.RoomController.RoomModel;
using AcademicManagementSystem.Models.SessionTypeController.SessionTypeModel;

namespace AcademicManagementSystem.Models.SessionController;

public class DetailSessionForStudentResponse
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
    
    [JsonPropertyName("can_take_gpa")]
    public bool CanTakeGpa { get; set; }
    
    [JsonPropertyName("session_type")]
    public SessionTypeResponse SessionType { get; set; }
    
    [JsonPropertyName("attendance")]
    public StudentAttendanceResponse Attendance { get; set; }
    
    [JsonPropertyName("class")]
    public BasicClassResponse Class { get; set; }
    
    [JsonPropertyName("module")]
    public BasicModuleResponse Module { get; set; }
    
    [JsonPropertyName("teacher")]
    public BasicTeacherInformationResponse Teacher { get; set; }

    [JsonPropertyName("room")]
    public RoomResponse Room { get; set; }
}