using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.AddressController.ProvinceModel;

public class ProvinceResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
        
    [JsonPropertyName("code")]
    public string? Code { get; set; }
        
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}