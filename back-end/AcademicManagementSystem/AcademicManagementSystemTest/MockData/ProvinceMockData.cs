using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class ProvinceMockData
{
    public static readonly List<Province> Provinces = new List<Province>
    {
        new Province()
        {
            Id = 1,
            Name = "Hồ Chí Minh",
            Code = "SG"
        },
        new Province()
        {
            Id = 2,
            Name = "Hà Nội",
            Code = "HN"
        }
    };
}