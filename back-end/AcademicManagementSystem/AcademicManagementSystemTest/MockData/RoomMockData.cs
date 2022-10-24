using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class RoomMockData
{
    public static readonly List<Room> Rooms = new List<Room>()
    {
        new Room()
        {
            CenterId = 1,
            RoomTypeId = 1,
            Name = "Room 1",
            Capacity = 20
        },
        new Room()
        {
            CenterId = 2,
            RoomTypeId = 2,
            Name = "Room 2",
            Capacity = 30
        },
        new Room()
        {
            CenterId = 1,
            RoomTypeId = 1,
            Name = "Room 3",
            Capacity = 20
        }
    };
}