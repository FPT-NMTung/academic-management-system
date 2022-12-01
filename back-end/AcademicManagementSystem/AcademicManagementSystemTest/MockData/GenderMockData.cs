using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class GenderMockData
{
    public static List<Gender> Genders = new List<Gender>
    {
        new Gender()
        {
            Id = 1,
            Value = "Nam"
        },
        new Gender()
        {
            Id = 2,
            Value = "Nữ"
        },
        new Gender()
        {
            Id = 3,
            Value = "Khác"
        },
        new Gender()
        {
            Id = 4,
            Value = "Không xác định"
        }
    };
}