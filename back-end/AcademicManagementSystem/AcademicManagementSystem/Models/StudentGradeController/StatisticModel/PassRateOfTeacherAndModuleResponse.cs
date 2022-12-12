using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;

namespace AcademicManagementSystem.Models.StudentGradeController.StatisticModel;

public class PassRateOfTeacherAndModuleResponse
{
    [JsonPropertyName("module")]
    public BasicModuleResponse Module { get; set; }

    [JsonPropertyName("number_of_student_in_all_class")]
    public int NumberOfStudentInAllClass { get; set; }
    
    [JsonPropertyName("number_of_passed_students")]
    public int NumberOfPassStudents { get; set; }
}