using System.ComponentModel;
using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.TeacherSkillController.Skill;

namespace AcademicManagementSystem.Models.UserController.TeacherController;

public class UpdateTeacherRequest
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
    
    [JsonPropertyName("teacher_type_id")]
    public int TeacherTypeId { get; set; }
    
    [JsonPropertyName("working_time_id")]
    public int WorkingTimeId { get; set; }

    [JsonPropertyName("nickname")]
    [DefaultValue(null)]
    public string? Nickname { get; set; }
    
    [JsonPropertyName("company_address")]
    [DefaultValue(null)]
    public string? CompanyAddress { get; set; }
    
    [JsonPropertyName("start_working_date")]
    public DateTime StartWorkingDate { get; set; }
    
    [JsonPropertyName("salary")]
    public decimal Salary { get; set; }
    
    [JsonPropertyName("tax_code")]
    public string TaxCode { get; set; }
}