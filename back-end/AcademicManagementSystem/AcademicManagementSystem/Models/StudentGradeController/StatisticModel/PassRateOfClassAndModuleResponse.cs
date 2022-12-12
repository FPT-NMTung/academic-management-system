using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;

namespace AcademicManagementSystem.Models.StudentGradeController.StatisticModel;

public class PassRateOfClassAndModuleResponse
{
    [JsonPropertyName("class")]
    public BasicClassResponse Class { get; set; }
    
    [JsonPropertyName("module")]
    public BasicModuleResponse Module { get; set; }
    
    [JsonPropertyName("teacher")]
    public BasicTeacherInformationResponse Teacher { get; set; }
    
    [JsonPropertyName("number_of_students")]
    public int NumberOfStudents { get; set; }
    
    [JsonPropertyName("number_of_passed_students")]
    public int NumberOfPassStudents { get; set; }
}