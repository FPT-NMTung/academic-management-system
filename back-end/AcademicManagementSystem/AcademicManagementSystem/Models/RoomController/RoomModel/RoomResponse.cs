using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.RoomController.RoomModel;

public class RoomResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("center_id")]
    public int CenterId { get; set; }
    
    [JsonPropertyName("center_name")]
    public string CenterName { get; set; }
    
    [JsonPropertyName("room_type_id")]
    public int RoomTypeId { get; set; }
    
    [JsonPropertyName("room_type_value")]
    public string RoomTypeValue { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }
}