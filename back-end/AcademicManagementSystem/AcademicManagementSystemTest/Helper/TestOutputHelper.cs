using AcademicManagementSystem.Controllers;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using Xunit.Abstractions;

namespace AcademicManagementSystemTest.Helper;

public class TestOutputHelper : ITestOutputHelper
{
    private readonly ITestOutputHelper _output;

    public TestOutputHelper(ITestOutputHelper output)
    {
        _output = output;
    }

    public void WriteLine(string message)
    {
        _output.WriteLine(message);
    }

    public void WriteLine(string format, params object[] args)
    {
        _output.WriteLine(format, args);
    }
    
    public void PrintMessage(IActionResult result)
    {
        if (result is BadRequestObjectResult badRequestObjectResult)
        {
            var value = badRequestObjectResult.Value as ResponseCustomBadRequest;
            var type = value?.TypeError;
            var statusCode = value?.StatusCode;
            var message = value?.Message;
            _output.WriteLine("Error type: " + type);
            _output.WriteLine("Status code: " + statusCode.GetHashCode() + " - " + statusCode);
            _output.WriteLine("Message: " + message);
        }

        if (result is OkObjectResult okObjectResult)
        {
            var value = okObjectResult.Value as ResponseCustom;
            var statusCode = value?.StatusCode;
            var message = value?.Message;
            var data = value?.Data;
            _output.WriteLine("Status code: " + statusCode.GetHashCode() + " - " + statusCode);
            _output.WriteLine("Message: " + message);
            _output.WriteLine("Data: " + data.ToJson());
        }

        if (result is NotFoundObjectResult notFoundObjectResult)
        {
            var value = notFoundObjectResult.Value as ResponseCustom;
            var statusCode = value?.StatusCode;
            var message = value?.Message;
            _output.WriteLine("Status code: " + statusCode.GetHashCode() + " - " + statusCode);
            _output.WriteLine("Message: " + message);
        }

        if (result is UnauthorizedObjectResult unauthorizedObjectResult)
        {
            var value = unauthorizedObjectResult.Value as ResponseCustom;
            var statusCode = value?.StatusCode;
            var message = value?.Message;
            _output.WriteLine("Status code: " + statusCode.GetHashCode() + " - " + statusCode);
            _output.WriteLine("Message: " + message);
        }
    }
    
    private static string RandomString()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 32)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }
}
