using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Models.BasicResponse;

public class BasicTeacherInformationResponse
{
    [JsonPropertyName("id")] 
    public int? Id { get; set; }

    [JsonPropertyName("first_name")] 
    public string FirstName { get; set; }

    [JsonPropertyName("last_name")] 
    public string LastName { get; set; }
    
    [JsonPropertyName("email_organization")]
    public string EmailOrganization { get; set; }
}