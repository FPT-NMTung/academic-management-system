using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.ClassScheduleController.ClassScheduleModel;

public class CreateClassScheduleRequest
{
    [JsonPropertyName("module_id")]
    public int ModuleId { get; set; }
    
    [JsonPropertyName("teacher_id")]
    public int TeacherId { get; set; }
    
    [JsonPropertyName("class_days_id")]
    public int ClassDaysId { get; set; }

    [JsonPropertyName("working_time_id")]
    public int WorkingTimeId { get; set; }
    
    [JsonPropertyName("theory_room_id")]
    public int? TheoryRoomId { get; set; }
    
    [JsonPropertyName("lab_room_id")]
    public int? LabRoomId { get; set; }

    [JsonPropertyName("exam_room_id")]
    public int? ExamRoomId { get; set; }
    
    //days
    [JsonPropertyName("duration")]
    public int Duration { get; set; }
    
    [JsonPropertyName("practice_session")]
    public List<int>? PracticeSession { get; set; }

    [JsonPropertyName("start_date")]
    public DateTime StartDate { get; set; }
    
    [JsonPropertyName("class_hour_start")]
    public TimeSpan ClassHourStart { get; set; }
    
    [JsonPropertyName("class_hour_end")]
    public TimeSpan ClassHourEnd { get; set; }
    
    // 1: morning, 2: afternoon, 3: evening
    [JsonPropertyName("slot")]
    public int Slot { get; set; }
    
    [JsonPropertyName("note")]
    public string? Note { get; set; }
}