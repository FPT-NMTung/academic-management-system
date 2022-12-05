using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class WardMockData
{
    public static readonly List<Ward> Wards = new List<Ward>
    {
        new Ward
        {
            Id = 1,
            DistrictId = 1,
            ProvinceId = 1,
            Name = "An Phú Tây",
            Prefix = "Xã"
        },
        new Ward
        {
            Id = 327,
            DistrictId = 2,
            ProvinceId = 25,
            Name = "Cống Vị",
            Prefix = "Phường"
        },
    };
}