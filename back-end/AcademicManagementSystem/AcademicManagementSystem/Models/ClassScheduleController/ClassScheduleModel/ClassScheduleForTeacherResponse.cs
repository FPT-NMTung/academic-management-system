using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;
using AcademicManagementSystem.Models.ClassDaysController;
using AcademicManagementSystem.Models.ClassStatusController;
using AcademicManagementSystem.Models.ModuleController;

namespace AcademicManagementSystem.Models.ClassScheduleController.ClassScheduleModel;

public class ClassScheduleForTeacherResponse
{
    [JsonPropertyName("schedule_id")]
    public int ScheduleId { get; set; }
    
    [JsonPropertyName("class")]
    public BasicClassResponse Class { get; set; }
    
    [JsonPropertyName("module")]
    public BasicModuleResponse Module { get; set; }
}