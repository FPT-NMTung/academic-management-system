using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    [HttpPost]
    [Route("api/test/read-file-excel")]
    public IActionResult Test()
    {
        // get location of file Template1.xlsx
        var path = Path.Combine(Directory.GetCurrentDirectory(), "TemplateExcel\\Template1.xlsx");
        using (var workbook = new XLWorkbook(path))
        {
            var worksheet = workbook.Worksheets.Worksheet(1);
            worksheet.Cell("B1").Value = "Hello World!";
            worksheet.Cell("B2").Value = "Hello World";
            workbook.SaveAs("GeneratedExcel\\HelloWorld.xlsx");
        }
        return Ok("Hello World");
    }
}