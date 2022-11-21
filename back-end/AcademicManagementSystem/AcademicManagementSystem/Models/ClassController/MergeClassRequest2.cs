using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.ClassController;

public class MergeClassRequest2
{
    [JsonPropertyName("first_class_id")]
    public int FirstClassId { get; set; }
    
    [JsonPropertyName("second_class_id")]
    public int SecondClassId { get; set; }
}