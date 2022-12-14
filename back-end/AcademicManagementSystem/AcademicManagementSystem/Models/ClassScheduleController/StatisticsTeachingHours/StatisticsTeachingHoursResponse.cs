using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.ClassScheduleController.StatisticTeachingHours;

public class StatisticsTeachingHoursResponse
{
    [JsonPropertyName("total_teaching_hours")]
    public double TotalTeachingHours { get; set; }
}