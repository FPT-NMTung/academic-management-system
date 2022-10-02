using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;

namespace AcademicManagementSystem.Models.AddressController;

public class AddressResponse
{
    [JsonPropertyName("province")]
    public ProvinceResponse? Province { get; set; }
    
    [JsonPropertyName("district")]
    public DistrictResponse? District { get; set; }
    
    [JsonPropertyName("ward")]
    public WardResponse? Ward { get; set; }
}