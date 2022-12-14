using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.ClassScheduleController.StatisticsAttendance;

public class StatisticsAttendanceResponse
{
    [JsonPropertyName("total_attendance")]
    public int TotalAttendance { get; set; }
    
    [JsonPropertyName("total_absence")]
    public int TotalAbsence { get; set; }
    
    [JsonPropertyName("total_students_in_learning_session")]
    public int TotalStudentsInLearningSession { get; set; }

    [JsonPropertyName("average_attendance")]
    public double AverageAttendance { get; set; }
    
    [JsonPropertyName("average_absence")]
    public double AverageAbsence { get; set; }
}