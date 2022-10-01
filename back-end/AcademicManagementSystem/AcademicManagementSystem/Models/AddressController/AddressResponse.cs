using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;

namespace AcademicManagementSystem.Models.AddressController;

public class AddressResponse
{
    [JsonPropertyName("province")]
    public string Province { get; set; }
    
    [JsonPropertyName("district")]
    public string District { get; set; }
    
    [JsonPropertyName("ward")]
    public string Ward { get; set; }
}