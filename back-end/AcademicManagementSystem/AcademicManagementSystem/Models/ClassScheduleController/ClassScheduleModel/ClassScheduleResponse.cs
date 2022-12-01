using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.ClassDaysController;
using AcademicManagementSystem.Models.ClassStatusController;
using AcademicManagementSystem.Models.RoomController.RoomModel;
using AcademicManagementSystem.Models.SessionController;
using AcademicManagementSystem.Models.TeacherSkillController;

namespace AcademicManagementSystem.Models.ClassScheduleController.ClassScheduleModel;

public class ClassScheduleResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("class_id")]
    public int ClassId { get; set; }
    
    [JsonPropertyName("class_name")]
    public string ClassName { get; set; }
    
    [JsonPropertyName("module_id")]
    public int ModuleId { get; set; }
    
    [JsonPropertyName("module_name")]
    public string ModuleName { get; set; }
    
    [JsonPropertyName("teacher")]
    public BasicTeacherInformationResponse Teacher { get; set; }
    
    [JsonPropertyName("class_days")]
    public ClassDaysResponse ClassDays { get; set; }
    
    [JsonPropertyName("class_status")]
    public ClassStatusResponse ClassStatus { get; set; }
    
    [JsonPropertyName("theory_room_id")]
    public int? TheoryRoomId { get; set; }
    
    [JsonPropertyName("theory_room_name")]
    public string? TheoryRoomName { get; set; }
    
    [JsonPropertyName("lab_room_id")]
    public int? LabRoomId { get; set; }
    
    [JsonPropertyName("lab_room_name")]
    public string? LabRoomName { get; set; }

    [JsonPropertyName("exam_room_id")]
    public int? ExamRoomId { get; set; }
    
    [JsonPropertyName("exam_room_name")]
    public string? ExamRoomName { get; set; }
    
    //days
    [JsonPropertyName("duration")]
    public int Duration { get; set; }
    
    [JsonPropertyName("start_date")]
    public DateTime StartDate { get; set; }
    
    [JsonPropertyName("end_date")]
    public DateTime EndDate { get; set; }
    
    [JsonPropertyName("theory_exam_date")]
    public DateTime? TheoryExamDate { get; set; }
    
    [JsonPropertyName("practical_exam_date")]
    public DateTime? PracticalExamDate { get; set; }
    
    [JsonPropertyName("class_hour_start")]
    public TimeSpan ClassHourStart { get; set; }
    
    [JsonPropertyName("class_hour_end")]
    public TimeSpan ClassHourEnd { get; set; }
    
    [JsonPropertyName("note")]
    public string? Note { get; set; }
    
    [JsonPropertyName("sessions")]
    public List<SessionResponse> Sessions { get; set; }
    
    [JsonPropertyName("working_time_id")]
    public int WorkingTimeId { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

