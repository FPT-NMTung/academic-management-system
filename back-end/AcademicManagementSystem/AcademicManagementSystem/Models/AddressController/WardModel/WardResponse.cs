using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.AddressController.WardModel;

public class WardResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
        
    [JsonPropertyName("name")]
    public string? Name { get; set; }
        
    [JsonPropertyName("prefix")]
    public string? Prefix { get; set; }
    
    // [JsonPropertyName("districtId")]
    // public int DistrictId { get; set; }
    //
    // [JsonPropertyName("provinceId")]
    // public int ProvinceId { get; set; }
}