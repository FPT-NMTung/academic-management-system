using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Models.AddressController.DistrictModel;
using AcademicManagementSystem.Models.AddressController.ProvinceModel;
using AcademicManagementSystem.Models.AddressController.WardModel;
using AcademicManagementSystem.Models.GenderController;
using AcademicManagementSystem.Models.RoleController;
using AcademicManagementSystem.Models.UserController;
using AcademicManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly AmsContext _context;
    private readonly IUserService _userService;
    private readonly IConfigurationRoot _configuration;

    public UserController(AmsContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
    }

    [HttpGet]
    [Route("api/users/information")]
    [Authorize(Roles = "admin, sro, teacher, student")]
    public IActionResult GetUsersInformation()
    {
        var id = _userService.GetUserId();
        var user = GetAllUsers().FirstOrDefault(u => u.UserId.ToString() == id);
        if (user == null)
        {
            return NotFound(CustomResponse.NotFound("User not found"));
        }
        return Ok(CustomResponse.Ok("Get user by id successfully", user));
    }
    
    [HttpPost]
    [Route("api/admin/users/{userId:int}/avatar")]
    [RequestSizeLimit(20_000_000)]
    [Authorize(Roles = "admin")]
    public IActionResult UpdateAvatarAdmin(int userId, [FromBody] AvatarRequest request)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return NotFound(CustomResponse.NotFound("User not found"));
        }

        if (request.Image.Length > 2 * 1024 * 1024)
        {
            return BadRequest(CustomResponse.BadRequest("Image size must be less than 2MB", null!));
        }

        var status = UploadImageToAzureStorage(request.Image, user);
        if (status == false)
        {
            return BadRequest(CustomResponse.BadRequest("Upload image to Azure Storage failed", null!));
        }
        
        return Ok(CustomResponse.Ok("Update avatar successfully", null!));
    }
    
    [HttpPost]
    [Route("api/sro/users/{userId:int}/avatar")]
    [RequestSizeLimit(20_000_000)]
    [Authorize(Roles = "sro")]
    public IActionResult UpdateAvatarSro(int userId, [FromBody] AvatarRequest request)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return NotFound(CustomResponse.NotFound("User not found"));
        }
        
        if (user.RoleId != 4)
        {
            return BadRequest(CustomResponse.BadRequest("You can only update avatar for student", null!));
        }

        if (request.Image.Length > 2 * 1024 * 1024)
        {
            return BadRequest(CustomResponse.BadRequest("Image size must be less than 2MB", null!));
        }

        var status = UploadImageToAzureStorage(request.Image, user);
        if (status == false)
        {
            return BadRequest(CustomResponse.BadRequest("Upload image to Azure Storage failed", null!));
        }
        
        return Ok(CustomResponse.Ok("Update avatar successfully", null!));
    }
    
    private bool UploadImageToAzureStorage(string image, User user)
    {
        try
        {
            var azureStorageAccountName = _configuration.GetChildren().First(x => x.Key == "AzureStorageAccountName").Value!;
            var azureStorageAccessKey = _configuration.GetChildren().First(x => x.Key == "AzureStorageAccessKey").Value!;
        
            StorageCredentials creden = new StorageCredentials(azureStorageAccountName, azureStorageAccessKey);
            CloudStorageAccount acc = new CloudStorageAccount(creden, useHttps: true);
            CloudBlobClient client = acc.CreateCloudBlobClient();
            CloudBlobContainer cont = client.GetContainerReference("ams");
        
            cont.CreateIfNotExists();
            cont.SetPermissions(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });
        
            var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            CloudBlockBlob cblob = cont.GetBlockBlobReference($"avatar/user-id-{user.Id}-{time}.png");
            cblob.Properties.ContentType = "image/png";
        
            var bytes = Convert.FromBase64String(image.Split(',')[1]);
            using (var stream = new MemoryStream(bytes))
            {
                cblob.UploadFromStream(stream);
            }
            var blobUrl = cblob.Uri.AbsoluteUri;
        
            user.Avatar = blobUrl;
            user.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }

    private IQueryable<UserResponse> GetAllUsers()
    {
        return _context.Users.Select(u => new UserResponse()
        {
            UserId = u.Id,
            Role = new RoleResponse()
            {
                Id = u.Role.Id,
                Value = u.Role.Value
            },
            FirstName = u.FirstName,
            LastName = u.LastName,
            MobilePhone = u.MobilePhone,
            Email = u.Email,
            EmailOrganization = u.EmailOrganization,
            Avatar = u.Avatar,
            Province = new ProvinceResponse()
            {
                Id = u.Province.Id,
                Code = u.Province.Code,
                Name = u.Province.Name
            },
            District = new DistrictResponse()
            {
                Id = u.District.Id,
                Name = u.District.Name,
                Prefix = u.District.Prefix
            },
            Ward = new WardResponse()
            {
                Id = u.Ward.Id,
                Name = u.Ward.Name,
                Prefix = u.Ward.Prefix
            },
            Gender = new GenderResponse()
            {
                Id = u.Gender.Id,
                Value = u.Gender.Value
            },
            Birthday = u.Birthday,
            CenterId = u.CenterId,
            CenterName = u.Center.Name,
            CitizenIdentityCardNo = u.CitizenIdentityCardNo,
            CitizenIdentityCardPublishedDate = u.CitizenIdentityCardPublishedDate,
            CitizenIdentityCardPublishedPlace = u.CitizenIdentityCardPublishedPlace,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        });
    }
}