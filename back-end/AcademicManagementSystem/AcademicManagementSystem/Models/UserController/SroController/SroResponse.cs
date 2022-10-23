using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.GenderController;
using AcademicManagementSystem.Models.RoleController;

namespace AcademicManagementSystem.Models.UserController.SroController;

public class SroResponse
{
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
    
    [JsonPropertyName("role")]
    public RoleResponse Role { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }
    
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }
    
    [JsonPropertyName("mobile_phone")]
    public string? MobilePhone { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("email_organization")]
    public string? EmailOrganization { get; set; }
    
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }
    
    [JsonPropertyName("province")]
    public ProvinceResponse Province { get; set; }
 
    [JsonPropertyName("district")]
    public DistrictResponse District { get; set; }
    
    [JsonPropertyName("ward")]
    public WardResponse Ward { get; set; }
    
    [JsonPropertyName("gender")]
    public GenderResponse Gender { get; set; }

    [JsonPropertyName("birthday")]
    public DateTime Birthday { get; set; }

    [JsonPropertyName("center_id")]
    public int CenterId { get; set; }
    
    [JsonPropertyName("center_name")]
    public string? CenterName { get; set; }
    
    [JsonPropertyName("citizen_identity_card_no")]
    public string? CitizenIdentityCardNo { get; set; }
    
    [JsonPropertyName("citizen_identity_card_published_date")]
    public DateTime CitizenIdentityCardPublishedDate { get; set; }
    
    [JsonPropertyName("citizen_identity_card_published_place")]
    public string? CitizenIdentityCardPublishedPlace { get; set; }
    
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }
    
    [JsonPropertyName("create_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("update_at")]
    public DateTime UpdatedAt { get; set; }
    
}