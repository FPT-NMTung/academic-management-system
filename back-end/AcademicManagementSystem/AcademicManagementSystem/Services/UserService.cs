namespace AcademicManagementSystem.Services;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;
    }
}