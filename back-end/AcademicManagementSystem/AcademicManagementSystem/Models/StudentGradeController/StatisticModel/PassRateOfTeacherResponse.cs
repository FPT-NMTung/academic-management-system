using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;

namespace AcademicManagementSystem.Models.StudentGradeController.StatisticModel;

public class PassRateOfTeacherResponse
{
    [JsonPropertyName("teacher")]
    public BasicTeacherInformationResponse Teacher { get; set; }
    
    [JsonPropertyName("number_of_all_students")]
    public int NumberOfAllStudents { get; set; }
    
    [JsonPropertyName("number_of_passed_students")]
    public int NumberOfPassStudents { get; set; }
}