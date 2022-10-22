using System.ComponentModel;
using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.UserController.SroController;

public class UpdateSroRequest
{
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

    [JsonPropertyName("province_id")]
    public int ProvinceId { get; set; }
    
    [JsonPropertyName("district_id")]
    public int DistrictId { get; set; }
    
    [JsonPropertyName("ward_id")]
    public int WardId { get; set; }
    
    [JsonPropertyName("gender_id")]
    public int GenderId { get; set; }
    
    [JsonPropertyName("birthday")]
    public DateTime Birthday { get; set; }

    [JsonPropertyName("citizen_identity_card_no")]
    public string CitizenIdentityCardNo { get; set; }
    
    [JsonPropertyName("citizen_identity_card_published_date")]
    public DateTime CitizenIdentityCardPublishedDate { get; set; }
    
    [JsonPropertyName("citizen_identity_card_published_place")]
    public string CitizenIdentityCardPublishedPlace { get; set; }
}