using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.ClassController;

public class MergeClassRequest
{
    [JsonPropertyName("class_id")]
    public int ClassId { get; set; }
}