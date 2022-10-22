using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;

namespace AcademicManagementSystem.Models.CenterController;

public class CenterResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("province")]
    public ProvinceResponse Province { get; set; }
    
    [JsonPropertyName("district")]
    public DistrictResponse District { get; set; }
    
    [JsonPropertyName("ward")]
    public WardResponse Ward { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("can_delete")]
    public bool? CanDelete { get; set; }
    
    [JsonPropertyName("is_active")]
    public bool? IsActive { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}