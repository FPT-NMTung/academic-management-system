using Newtonsoft.Json.Converters;

namespace AcademicManagementSystem.Handlers;

public class DateFormatConverter : IsoDateTimeConverter
{
    public DateFormatConverter(string format)
    {
        DateTimeFormat = format;
    }
    
}