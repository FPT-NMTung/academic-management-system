using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.CenterController;

public class CreateCenterRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("province_id")]
    public int ProvinceId { get; set; }

    [JsonPropertyName("district_id")]
    public int DistrictId { get; set; }
    
    [JsonPropertyName("ward_id")]
    public int WardId { get; set; }
}