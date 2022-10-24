using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class CenterMockData
{
    public static readonly List<Center> Centers = new List<Center>()
    {
        new Center()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            Name = "center 1",
            IsActive = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Center()
        {
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            Name = "center 2",
            IsActive = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Center()
        {
            ProvinceId = 2,
            DistrictId = 2,
            WardId = 2,
            Name = "center 3",
            IsActive = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
    };
}