using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.BasicResponse;

public class BasicStudentResponse
{
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
    
    [JsonPropertyName("enroll_number")]
    public string EnrollNumber { get; set; }
    
    [JsonPropertyName("email_organization")]
    public string EmailOrganization { get; set; }
    
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }
    
    [JsonPropertyName("last_name")]
    public string LastName { get; set; }
    
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }
}