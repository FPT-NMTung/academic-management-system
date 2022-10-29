using System.ComponentModel;
using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.CourseController;
using AcademicManagementSystem.Models.GenderController;
using AcademicManagementSystem.Models.RoleController;

namespace AcademicManagementSystem.Models.ClassController;

public class AddStudentToClassRequest
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
    
    [JsonPropertyName("enroll_number")]
    public string EnrollNumber { get; set; }
    
    [JsonPropertyName("course_code")]
    public string CourseCode { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("home_phone")]
    [DefaultValue(null)]
    public string? HomePhone { get; set; }
    
    [JsonPropertyName("contact_phone")]
    public string ContactPhone { get; set; }
    
    [JsonPropertyName("parental_name")]
    public string ParentalName { get; set; }
    
    [JsonPropertyName("parental_relationship")]
    public string ParentalRelationship { get; set; }
    
    [JsonPropertyName("contact_address")]
    public string ContactAddress { get; set; }
    
    [JsonPropertyName("parental_phone")]
    public string ParentalPhone { get; set; }
    
    [JsonPropertyName("application_date")]
    public DateTime ApplicationDate { get; set; }
    
    [JsonPropertyName("application_document")]
    [DefaultValue(null)]
    public string? ApplicationDocument { get; set; }

    [JsonPropertyName("high_school")]
    [DefaultValue(null)]
    public string? HighSchool { get; set; }
    
    [JsonPropertyName("university")]
    [DefaultValue(null)]
    public string? University { get; set; }
    
    [JsonPropertyName("facebook_url")]
    [DefaultValue(null)]
    public string? FacebookUrl { get; set; }
    
    [JsonPropertyName("portfolio_url")]
    [DefaultValue(null)]
    public string? PortfolioUrl { get; set; }
    
    [JsonPropertyName("working_company")]
    [DefaultValue(null)]
    public string? WorkingCompany { get; set; }
    
    [JsonPropertyName("company_salary")]
    [DefaultValue(null)]
    public int? CompanySalary { get; set; }
    
    [JsonPropertyName("company_position")]
    [DefaultValue(null)]
    public string? CompanyPosition { get; set; }
    
    [JsonPropertyName("company_address")]
    [DefaultValue(null)]
    public string? CompanyAddress { get; set; }
    
    [JsonPropertyName("fee_plan")]
    public int FeePlan { get; set; }
    
    [JsonPropertyName("promotion")]
    public int Promotion { get; set; }
}