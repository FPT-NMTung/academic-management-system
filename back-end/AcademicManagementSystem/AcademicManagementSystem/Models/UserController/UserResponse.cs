using System.Text.Json.Serialization;
using AcademicManagementSystem.Handlers;

namespace AcademicManagementSystem.Models.UserController;

public class UserResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("role_id")]
    public int RoleId { get; set; }
    
    [JsonPropertyName("role_value")]
    public string RoleValue { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }
    
    [JsonPropertyName("last_name")]
    public string LastName { get; set; }
    
    [JsonPropertyName("mobile_phone")]
    public string MobilePhone { get; set; }
    
    [JsonPropertyName("email")]
    public string Email { get; set; }
    
    [JsonPropertyName("email_organization")]
    public string EmailOrganization { get; set; }
    
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }
    
    [JsonPropertyName("province_id")]
    public int ProvinceId { get; set; }
    
    [JsonPropertyName("province_code")]
    public string ProvinceCode { get; set; }
    
    [JsonPropertyName("province_name")]
    public string ProvinceName { get; set; } 
 
    [JsonPropertyName("district_id")]
    public int DistrictId { get; set; }
    
    [JsonPropertyName("district_name")]
    public string DistrictName { get; set; }
    
    [JsonPropertyName("district_prefix")]
    public string DistrictPrefix { get; set; }
    
    [JsonPropertyName("ward_id")]
    public int WardId { get; set; }
    
    [JsonPropertyName("ward_name")]
    public string WardName { get; set; }
    
    [JsonPropertyName("ward_prefix")]
    public string WardPrefix { get; set; }
    
    [JsonPropertyName("genderId")]
    public int GenderId { get; set; }
    
    [JsonPropertyName("gender_value")]
    public string GenderValue { get; set; }

    [JsonPropertyName("birthday")]
    [Newtonsoft.Json.JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy")]
    public DateTime Birthday { get; set; }

    [JsonPropertyName("center_id")]
    public int CenterId { get; set; }
    
    [JsonPropertyName("center_name")]
    public string CenterName { get; set; }
    
    [JsonPropertyName("citizen_identity_card_no")]
    public string CitizenIdentityCardNo { get; set; }
    
    [JsonPropertyName("citizen_identity_card_published_date")]
    [Newtonsoft.Json.JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy")]
    public DateTime CitizenIdentityCardPublishedDate { get; set; }
    
    [JsonPropertyName("citizen_identity_card_published_place")]
    public string CitizenIdentityCardPublishedPlace { get; set; }
}