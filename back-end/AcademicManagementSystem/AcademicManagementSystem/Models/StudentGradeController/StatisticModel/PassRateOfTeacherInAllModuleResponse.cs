using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.StudentGradeController.StatisticModel;

public class PassRateOfTeacherInAllModuleResponse
{
    [JsonPropertyName("number_of_all_students")]
    public int NumberOfAllStudents { get; set; }
    
    [JsonPropertyName("number_of_passed_students")]
    public int NumberOfPassStudents { get; set; }
}