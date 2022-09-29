using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.RoomController.RoomModel;

public class RoomResponse
{
    [JsonPropertyName("room_code")]
    public string RoomCode { get; set; }
    
    // [JsonPropertyName("center_id")]
    // public int CenterId { get; set; }
    //
    // [JsonPropertyName("room_type_id")]
    // public int RoomTypeId { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }
}