using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.ClassDaysController;

public class ClassDaysResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; }
}