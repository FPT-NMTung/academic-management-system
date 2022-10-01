using Newtonsoft.Json;

namespace AcademicManagementSystem.Models.AddressController.DistrictModel;

public class DistrictResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("prefix")]
    public string Prefix { get; set; }
    
    // [JsonProperty("province_id")]
    // public int ProvinceId { get; set; }
}