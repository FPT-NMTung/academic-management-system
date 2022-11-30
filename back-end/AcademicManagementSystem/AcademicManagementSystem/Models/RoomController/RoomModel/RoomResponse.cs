using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.RoomController.RoomTypeModel;

namespace AcademicManagementSystem.Models.RoomController.RoomModel;

public class RoomResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("center_id")]
    public int CenterId { get; set; }
    
    [JsonPropertyName("center_name")]
    public string? CenterName { get; set; }
    
    [JsonPropertyName("room_type")]
    public RoomTypeResponse? RoomType { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }
    
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }
}