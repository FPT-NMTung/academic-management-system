using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.CourseModuleSemester;

namespace AcademicManagementSystem.Models.SemesterController;

public class SemesterResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
}