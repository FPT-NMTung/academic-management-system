using System.Text.Json.Serialization;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.CourseController;
using AcademicManagementSystem.Models.GenderController;
using AcademicManagementSystem.Models.RoleController;

namespace AcademicManagementSystem.Models.UserController.StudentController;

public class StudentResponse
{
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
    
    [JsonPropertyName("role")]
    public RoleResponse? Role { get; set; }

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
    public ProvinceResponse? Province { get; set; }
 
    [JsonPropertyName("district")]
    public DistrictResponse? District { get; set; }
    
    [JsonPropertyName("ward")]
    public WardResponse? Ward { get; set; }
    
    [JsonPropertyName("gender")]
    public GenderResponse? Gender { get; set; }

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
    
    [JsonPropertyName("create_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("update_at")]
    public DateTime UpdatedAt { get; set; }
    
    [JsonPropertyName("enroll_number")]
    public string? EnrollNumber { get; set; }
    
    [JsonPropertyName("course_code")]
    public string? CourseCode { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }
    
    [JsonPropertyName("status_date")]
    public DateTime StatusDate { get; set; }
    
    [JsonPropertyName("home_phone")]
    public string? HomePhone { get; set; }
    
    [JsonPropertyName("contact_phone")]
    public string? ContactPhone { get; set; }
    
    [JsonPropertyName("parental_name")]
    public string? ParentalName { get; set; }
    
    [JsonPropertyName("parental_relationship")]
    public string? ParentalRelationship { get; set; }
    
    [JsonPropertyName("contact_address")]
    public string? ContactAddress { get; set; }
    
    [JsonPropertyName("parental_phone")]
    public string? ParentalPhone { get; set; }
    
    [JsonPropertyName("application_date")]
    public DateTime ApplicationDate { get; set; }
    
    [JsonPropertyName("application_document")]
    public string? ApplicationDocument { get; set; }

    [JsonPropertyName("high_school")]
    public string? HighSchool { get; set; }
    
    [JsonPropertyName("university")]
    public string? University { get; set; }
    
    [JsonPropertyName("facebook_url")]
    public string? FacebookUrl { get; set; }
    
    [JsonPropertyName("portfolio_url")]
    public string? PortfolioUrl { get; set; }
    
    [JsonPropertyName("working_company")]
    public string? WorkingCompany { get; set; }
    
    [JsonPropertyName("company_salary")]
    public int? CompanySalary { get; set; }
    
    [JsonPropertyName("company_position")]
    public string? CompanyPosition { get; set; }
    
    [JsonPropertyName("company_address")]
    public string? CompanyAddress { get; set; }
    
    [JsonPropertyName("fee_plan")]
    public int FeePlan { get; set; }
    
    [JsonPropertyName("promotion")]
    public int Promotion { get; set; }
    
    [JsonPropertyName("is_draft")]
    public bool IsDraft { get; set; }
    
    [JsonPropertyName("course")]
    public CourseResponse? Course { get; set; }
    
    [JsonPropertyName("old_class")]
    public List<LearningClassResponse>? OldClass { get; set; }

    [JsonPropertyName("current_class")]
    public LearningClassResponse? CurrentClass { get; set; }
    
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }
}

public class LearningClassResponse
{
    [JsonPropertyName("class_id")] 
    public int? ClassId { get; set; }
    
    [JsonPropertyName("class_name")] 
    public string? ClassName { get; set; }
    
    [JsonPropertyName("start_date")]
    public DateTime? StartDate { get; set; }
}