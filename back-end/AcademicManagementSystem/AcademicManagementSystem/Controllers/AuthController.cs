using System.Text;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Models;
using AcademicManagementSystem.Models.AuthController.RefreshTokenModel;
using Google.Apis.Auth;
using Jose;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    // private readonly AmsContext _context;
    // private readonly IConfigurationRoot _configuration;
    //
    // public AuthController(AmsContext context)
    // {
    //     _configuration = new ConfigurationBuilder()
    //         .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    //         .AddJsonFile("appsettings.json")
    //         .Build();
    //     _context = context;
    // }
    //
    // [HttpPost("api/auth/login")]
    // public IActionResult Login([FromBody] LoginRequest request)
    // {
    //     var googleClientId = _configuration.GetChildren().First(x => x.Key == "GoogleClientId").Value!;
    //     
    //     // Validate the token with Google
    //     var payload = GoogleJsonWebSignature.ValidateAsync(request.TokenGoogle,
    //         new GoogleJsonWebSignature.ValidationSettings()
    //         {
    //             Audience = new[] { googleClientId }
    //         }).Result!;
    //     
    //     // Check domain of the user
    //     // if (!payload.HostedDomain.Equals("fpt.edu.vn"))
    //     // {
    //     //     return BadRequest("You are not a FPT student");
    //     // }
    //     
    //     // Get and check the user from the database
    //     var selectUser = _context.Users.FirstOrDefault(user => user.EmailOrganization == payload.Email);
    //     if (selectUser == null)
    //     {
    //         return BadRequest(CustomResponse.BadRequest("User not found", "auth-error-000001"));
    //     }
    //     
    //     // Generate the JWT token
    //     DateTimeOffset dateCreateToken = DateTimeOffset.Now;
    //     DateTimeOffset dateExpireToken = dateCreateToken.AddMinutes(30);
    //     DateTimeOffset dateExpireRefreshToken = dateCreateToken.AddMonths(1);
    //     var accessToken = GenerateToken(selectUser.Id, dateCreateToken.ToUnixTimeSeconds(), dateExpireToken.ToUnixTimeSeconds(), selectUser.RoleId);
    //     var refreshToken = GenerateRefreshToken(selectUser.Id, dateCreateToken.ToUnixTimeSeconds(), dateExpireRefreshToken.ToUnixTimeSeconds());
    //     _context.ActiveRefreshTokens.Add(new ActiveRefreshToken()
    //     {
    //         UserId = selectUser.Id,
    //         RefreshToken = refreshToken,
    //         ExpDate = dateExpireRefreshToken.DateTime
    //     });
    //     _context.SaveChanges();
    //
    //     return Ok(CustomResponse.Ok("Create token success", new LoginResponse()
    //     {
    //         AccessToken = accessToken,
    //         RefreshToken = refreshToken
    //     }));
    // }
    //
    // [HttpPost("api/auth/refresh-token")]
    // public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
    // {
    //     var selectRefreshToken = _context.ActiveRefreshTokens.FirstOrDefault(x => x.RefreshToken == request.RefreshToken && x.ExpDate > DateTime.Now);
    //     
    //     if (selectRefreshToken == null)
    //         return Unauthorized(CustomResponse.Unauthorized("Unauthorized"));
    //     
    //     var selectUser = _context.Users.FirstOrDefault(x => x.Id == selectRefreshToken.UserId);
    //     
    //     if (selectUser == null)
    //         return Unauthorized(CustomResponse.Unauthorized("Unauthorized"));
    //     
    //     // Generate the JWT token
    //     DateTimeOffset dateCreateToken = DateTimeOffset.Now;
    //     DateTimeOffset dateExpireToken = dateCreateToken.AddMinutes(30);
    //     DateTimeOffset dateExpireRefreshToken = dateCreateToken.AddMonths(1);
    //     var accessToken = GenerateToken(selectUser.Id, dateCreateToken.ToUnixTimeSeconds(), dateExpireToken.ToUnixTimeSeconds(), selectUser.RoleId);
    //     var refreshToken = GenerateRefreshToken(selectUser.Id, dateCreateToken.ToUnixTimeSeconds(), dateExpireRefreshToken.ToUnixTimeSeconds());
    //     
    //     // Update refresh token in database
    //     _context.ActiveRefreshTokens.Remove( selectRefreshToken );
    //     _context.ActiveRefreshTokens.Add(new ActiveRefreshToken()
    //     {
    //         UserId = selectUser.Id,
    //         RefreshToken = refreshToken,
    //         ExpDate = dateExpireRefreshToken.DateTime
    //     });
    //     _context.SaveChanges();
    //
    //     return Ok(CustomResponse.Ok("Refresh token success", new LoginResponse()
    //     {
    //         AccessToken = accessToken,
    //         RefreshToken = refreshToken
    //     }));
    // }
    //
    // private string GenerateToken(int userId, long createTime, long expireTime, int userRole)
    // {
    //     Jwk keyToken =
    //         new Jwk(Encoding.ASCII.GetBytes(_configuration.GetChildren().First(x => x.Key == "SecretKeyAccessToken").Value!));
    //
    //     Console.Out.WriteLine("Key: " + keyToken);
    //
    //     string token = Jose.JWT.Encode(new Dictionary<string, object>()
    //     {
    //         { "uid", userId },
    //         { "iat", createTime },
    //         { "exp", expireTime },
    //         { "role", userRole }
    //     }, keyToken, JwsAlgorithm.HS256);
    //
    //     return token;
    // }
    //
    // private string GenerateRefreshToken(int userId, long createTime, long expireTime)
    // {
    //     Jwk keyRefreshToken =
    //         new Jwk(Encoding.ASCII.GetBytes(_configuration.GetChildren().First(x => x.Key == "SecretKeyRefreshToken").Value!));
    //
    //     Console.Out.WriteLine("SecretKeyRefreshToken: " + _configuration.GetChildren().First(x => x.Key == "SecretKeyRefreshToken").Value!);
    //
    //     string refreshToken = Jose.JWT.Encode(new Dictionary<string, object>()
    //     {
    //         { "uid", userId },
    //         { "iat", createTime },
    //         { "exp", expireTime }
    //     }, keyRefreshToken, JwsAlgorithm.HS256);
    //
    //     return refreshToken;
    // }
}