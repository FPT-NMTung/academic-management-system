using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.RoomController.RoomModel;

public class CheckRoomCanDeleteResponse
{
    [JsonPropertyName("can_delete")]
    public bool CanDelete { get; set; }
}