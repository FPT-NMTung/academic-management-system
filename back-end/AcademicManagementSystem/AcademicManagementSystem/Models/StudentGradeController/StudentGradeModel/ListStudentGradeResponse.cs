using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.BasicResponse;

namespace AcademicManagementSystem.Models.StudentGradeController.StudentGradeModel;

public class ListStudentGradeResponse
{
    [JsonPropertyName("class")]
    public BasicClassResponse Class { get; set; }

    [JsonPropertyName("module")]
    public BasicModuleResponse Module { get; set; }

    [JsonPropertyName("students")]
    public List<StudentInfoAndGradeResponse> Students { get; set; }
}