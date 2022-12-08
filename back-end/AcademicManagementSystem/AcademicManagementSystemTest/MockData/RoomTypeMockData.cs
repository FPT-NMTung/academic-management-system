using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class RoomTypeMockData
{
    public static readonly List<RoomType> RoomTypes = new List<RoomType>
    {
        new RoomType
        {
            Id = 1,
            Value = "Theory"
        },
        new RoomType
        {
            Id = 2,
            Value = "Practical"
        }
    };
}