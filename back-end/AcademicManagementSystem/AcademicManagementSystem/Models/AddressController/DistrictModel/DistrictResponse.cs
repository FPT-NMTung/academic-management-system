using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.AddressController.DistrictModel;

public class DistrictResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("prefix")]
    public string? Prefix { get; set; }
    
    // [JsonProperty("province_id")]
    // public int ProvinceId { get; set; }
}