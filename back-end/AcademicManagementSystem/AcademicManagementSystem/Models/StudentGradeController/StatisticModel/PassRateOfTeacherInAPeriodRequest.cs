using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.StudentGradeController.StatisticModel;

public class PassRateOfTeacherInAPeriodRequest
{
    [JsonPropertyName("from_date")]
    public DateTime FromDate { get; set; }
    
    [JsonPropertyName("to_date")]
    public DateTime ToDate { get; set; }
}