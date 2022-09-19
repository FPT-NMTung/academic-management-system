using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    [HttpPost]
    [Route("api/test/create-file-excel")]
    public IActionResult Test()
    {
        // get location of file Template1.xlsx
        var path = Path.Combine(Directory.GetCurrentDirectory(), "TemplateExcel/Template1.xlsx");
        using (var workbook = new XLWorkbook(path))
        {
            var worksheet = workbook.Worksheets.Worksheet(1);
            worksheet.Cell("B1").Value = "Hello World!";
            worksheet.Cell("B2").Value = "Hello World";
            workbook.SaveAs("GeneratedExcel/HelloWorld.xlsx");
        }
        return Ok("Hello World");
    }
    
    [HttpGet]
    [Route("api/test/read-file-excel")]
    public IActionResult Test2()
    {
        // get location of file Template1.xlsx
        var path = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedExcel/HelloWorld.xlsx");
        using (var workbook = new XLWorkbook(path))
        {
            var worksheet = workbook.Worksheets.Worksheet(1);
            var value = worksheet.Cell("B2").Value;
            return Ok(value);
        }
    }
    
    [HttpGet]
    [Route("api/test/download-file-excel")]
    public IActionResult DownloadFileExcel()
    {
        // get location of file Template1.xlsx
        var path = Path.Combine(Directory.GetCurrentDirectory(), "TemplateExcel/Template1.xlsx");
        using (var workbook = new XLWorkbook(path))
        {
            var worksheet = workbook.Worksheets.Worksheet(1);
            worksheet.Cell("B1").Value = "Hello World!";
            worksheet.Cell("B2").Value = "Hello World";
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "HelloWorld.xlsx");
            }
        }
    }

    [HttpPost]
    [Route("api/test/send-email")]
    public IActionResult TestSendEmail()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = configuration.GetConnectionString("AzureEmailConnectionString");
        EmailClient emailClient = new EmailClient(connectionString);
        
        //Replace with your domain and modify the content, recipient details as required
        
        EmailContent emailContent = new EmailContent("Welcome to Azure Communication Service Email APIs.");
        emailContent.PlainText = "This email message is sent from Azure Communication Service Email using .NET SDK.";
        List<EmailAddress> emailAddresses = new List<EmailAddress> { new EmailAddress("admin@nmtung.dev") { DisplayName = "Nguyen Manh Tung" }};
        EmailRecipients emailRecipients = new EmailRecipients(emailAddresses);
        EmailMessage emailMessage = new EmailMessage("ams-no-reply@nmtung.dev", emailContent, emailRecipients);
        SendEmailResult emailResult = emailClient.Send(emailMessage,CancellationToken.None);

        return Ok(emailResult);
    }
}