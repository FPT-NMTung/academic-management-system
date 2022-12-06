using System.Net;
using System.Text.Json.Serialization;

namespace AcademicManagementSystem.Controllers;

public static class CustomResponse
{
    public static object Ok(string message, object data)
    {
        return new ResponseCustom()
        {
            StatusCode = HttpStatusCode.OK,
            Message = message,
            Data = data
        };
    }
    
    public static object BadRequest(string message, string typeError)
    {
        return new ResponseCustomBadRequest()
        {
            StatusCode = HttpStatusCode.BadRequest,
            TypeError = typeError,
            Message = message
        };
    }
    
    public static object Unauthorized(string message)
    {
        return new ResponseCustom()
        {
            StatusCode = HttpStatusCode.Unauthorized,
            Message = message
        };
    }
    
    public static object NotFound(string message)
    {
        return new ResponseCustom()
        {
            StatusCode = HttpStatusCode.NotFound,
            Message = message
        };
    }
}

public class ResponseCustom
{
    [JsonPropertyName("status_code")] 
    public HttpStatusCode StatusCode { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("data")] 
    public object? Data { get; set; }
}

public class ResponseCustomBadRequest : ResponseCustom
{
    [JsonPropertyName("type_error")] 
    public string TypeError { get; set; }
}