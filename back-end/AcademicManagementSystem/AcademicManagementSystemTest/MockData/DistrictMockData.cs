using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class DistrictMockData
{
    public static readonly List<District> Districts = new List<District>
    {
        new District
        {
            Id = 1,
            ProvinceId = 1,
            Name = "Bình Chánh",
            Prefix = "Huyện"
        },
        new District
        {
            Id = 25,
            ProvinceId = 2,
            Name = "Ba Đình",
            Prefix = "Quận"
        },
    };
}