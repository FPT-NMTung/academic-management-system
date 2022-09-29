using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.RoomController.RoomTypeModel;

public class RoomTypeResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; }
}