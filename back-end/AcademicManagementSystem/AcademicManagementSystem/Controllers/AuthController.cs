using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Models;
using AcademicManagementSystem.Models.AuthController.RefreshTokenModel;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly AmsContext _context;
    private readonly IConfigurationRoot _configuration;

    public AuthController(AmsContext context)
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        _context = context;
    }

    [HttpPost("api/auth/login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var googleClientId = _configuration.GetChildren().First(x => x.Key == "GoogleClientId").Value!;

        // Validate the token with Google
        var payload = GoogleJsonWebSignature.ValidateAsync(request.TokenGoogle,
            new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { googleClientId }
            }).Result!;

        // Get and check the user from the database
        var selectUser = _context.Users.Include(e => e.Role)
            .FirstOrDefault(user => user.EmailOrganization == payload.Email);
        if (selectUser == null)
        {
            return BadRequest(CustomResponse.BadRequest("User not found", "auth-error-000001"));
        }
        
        if (!selectUser.IsActive)
        {
            return BadRequest(CustomResponse.BadRequest("User is not active", "auth-error-000002"));
        }
        
        if (selectUser.RoleId == 4)
        {
            var selectStudent = _context.Students.FirstOrDefault(x => x.UserId == selectUser.Id);
            if (selectStudent?.IsDraft == true)
                return Unauthorized(CustomResponse.Unauthorized("Unauthorized"));
        }

        var accessToken = GenerateToken(selectUser.Id, selectUser.Role.Value);
        var refreshToken = GenerateRefreshToken(selectUser.Id);
        _context.ActiveRefreshTokens.Add(new ActiveRefreshToken()
        {
            UserId = selectUser.Id,
            RefreshToken = refreshToken,
            ExpDate = DateTime.Now.AddHours(1)
        });
        _context.SaveChanges();

        return Ok(CustomResponse.Ok("Create token success", new LoginResponse()
        {
            UserId = selectUser.Id,
            RoleId = selectUser.RoleId,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        }));
    }

    [HttpPost("api/auth/refresh-token")]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var selectRefreshToken =
            _context.ActiveRefreshTokens.FirstOrDefault(x =>
                x.RefreshToken == request.RefreshToken && x.ExpDate > DateTime.Now);

        if (selectRefreshToken == null)
            return Unauthorized(CustomResponse.Unauthorized("Unauthorized"));

        var selectUser = _context.Users.Include(e => e.Role).FirstOrDefault(x => x.Id == selectRefreshToken.UserId);

        if (selectUser == null)
            return Unauthorized(CustomResponse.Unauthorized("Unauthorized"));
        
        if (!selectUser.IsActive)
        {
            return Unauthorized(CustomResponse.Unauthorized("Unauthorized"));
        }

        if (selectUser.RoleId == 4)
        {
            var selectStudent = _context.Students.FirstOrDefault(x => x.UserId == selectUser.Id);
            if (selectStudent?.IsDraft == true)
                return Unauthorized(CustomResponse.Unauthorized("Unauthorized"));
        }

        // Generate the JWT token
        var accessToken = GenerateToken(selectUser.Id, selectUser.Role.Value);
        var refreshToken = GenerateRefreshToken(selectUser.Id);

        // Update refresh token in database
        // try
        // {
        //     _context.ActiveRefreshTokens.Remove(selectRefreshToken);
        //     _context.SaveChanges();
        // }
        // catch (Exception)
        // {
        //     return NoContent();
        // }

        _context.ActiveRefreshTokens.Add(new ActiveRefreshToken
        {
            UserId = selectUser.Id,
            RefreshToken = refreshToken,
            ExpDate = DateTime.Now.AddHours(1)
        });
        _context.SaveChanges();

        Console.Out.WriteLine("refreshToken = " + refreshToken);
        return Ok(CustomResponse.Ok("Refresh token success", new LoginResponse()
        {
            UserId = selectUser.Id,
            RoleId = selectUser.RoleId,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        }));
    }

    private string GenerateToken(int userId, string userRole)
    {
        var secretKey = _configuration.GetSection("SecretKeyAccessToken").Value!;
        List<Claim> claims = new()
        {
            new Claim("uid", userId.ToString()),
            new Claim("role", userRole),
            new Claim("unique_string", RandomString())
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken(int userId)
    {
        var secretKey = _configuration.GetSection("SecretKeyRefreshToken").Value!;
        List<Claim> claims = new()
        {
            new Claim("uid", userId.ToString()),
            new Claim("unique_string", RandomString())
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // random string generator length 30
    private static string RandomString()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 32)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }
}