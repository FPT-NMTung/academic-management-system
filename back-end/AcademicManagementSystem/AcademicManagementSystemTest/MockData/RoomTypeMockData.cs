using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class RoomTypeMockData
{
    public static readonly List<RoomType> RoomTypes = new List<RoomType>
    {
        new RoomType
        {
            Value = "Theory"
        },
        new RoomType
        {
            Value = "Practical"
        }
    };
}