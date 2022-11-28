using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.ClassController;
using AcademicManagementSystem.Models.ClassScheduleController.ClassScheduleModel;

namespace AcademicManagementSystem.Models.ClassScheduleController.ProgressModel;

public class ProgressModelResponse
{
    [JsonPropertyName("class")]
    public ClassResponse ClassResponse { get; set; }
    
    [JsonPropertyName("schedule")]
    public IEnumerable<ClassScheduleResponse> ClassScheduleResponses { get; set; }
}