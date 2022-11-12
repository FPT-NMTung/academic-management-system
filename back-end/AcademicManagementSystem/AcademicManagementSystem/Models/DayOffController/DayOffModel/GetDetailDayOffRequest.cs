using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.DayOffController.DayOffModel;

public class GetDetailDayOffRequest
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}