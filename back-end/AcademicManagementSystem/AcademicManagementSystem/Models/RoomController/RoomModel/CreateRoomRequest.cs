using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.RoomController.RoomModel;

public class CreateRoomRequest
{

    [JsonPropertyName("center_id")]
    public int CenterId { get; set; }
    
    [JsonPropertyName("room_type_id")]
    public int RoomTypeId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }

}