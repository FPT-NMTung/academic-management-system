using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.ClassController;

public class MergeClassRequest
{
    [JsonPropertyName("classId")]
    public int ClassId { get; set; }
}