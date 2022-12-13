using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.ClassScheduleController.StatisticsAttendance;

public class StatisticsAttendanceResponse
{
    [JsonPropertyName("total_attendance")]
    public int TotalAttendance { get; set; }
    
    [JsonPropertyName("total_absence")]
    public int TotalAbsence { get; set; }

    [JsonPropertyName("average_attendance")]
    public double AverageAttendance { get; set; }
}