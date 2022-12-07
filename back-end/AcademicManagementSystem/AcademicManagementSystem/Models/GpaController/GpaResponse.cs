using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.GpaController;

public class GpaResponse
{
    [JsonPropertyName("average_gpa")]
    public double AverageGpa { get; set; }
    
    [JsonPropertyName("comments")]
    public List<string?> Comments { get; set; }
}