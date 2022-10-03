using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.RoomController.RoomModel;

public class UpdateRoomRequest
{
    [JsonPropertyName("room_type_id")]
    public int RoomTypeId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }
}