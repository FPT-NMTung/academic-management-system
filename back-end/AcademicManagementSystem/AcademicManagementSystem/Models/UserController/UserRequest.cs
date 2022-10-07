using System.Text.Json.Serialization;
using AcademicManagementSystem.Handlers;

namespace AcademicManagementSystem.Models.UserController;

public class UserRequest
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("role_id")]
    public string? RoleId { get; set; }
    
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
    
    [JsonPropertyName("province_id")]
    public int ProvinceId { get; set; }
    
    [JsonPropertyName("district_id")]
    public int DistrictId { get; set; }
    
    [JsonPropertyName("ward_id")]
    public int WardId { get; set; }
    
    [JsonPropertyName("genderId")]
    public int GenderId { get; set; }

    [JsonPropertyName("birthday")]
    public DateTime Birthday { get; set; }

    [JsonPropertyName("center_id")]
    public int CenterId { get; set; }
    
    [JsonPropertyName("citizen_identity_card_no")]
    public string? CitizenIdentityCardNo { get; set; }
    
    [JsonPropertyName("citizen_identity_card_date")]
    public DateTime CitizenIdentityCardDate { get; set; }
    
    [JsonPropertyName("citizen_identity_card_place")]
    public string? CitizenIdentityCardPlace { get; set; }
    
}
