using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class SessionMockData
{
    public static readonly List<Session> Sessions = new List<Session>()
    {
        new Session()
        {
            Id = 1,
            ClassScheduleId = 1,
            SessionTypeId = 1,
            RoomId = 1,
            Title = "Session 1",
            LearningDate = new DateTime(2022, 01, 03),
            StartTime = new TimeSpan(8,0,0),
            EndTime = new TimeSpan(12,0,0)
        },
        new Session()
        {
            Id = 2,
            ClassScheduleId = 1,
            SessionTypeId = 1,
            RoomId = 1,
            Title = "Session 2",
            LearningDate = new DateTime(2022, 01, 05),
            StartTime = new TimeSpan(8,0,0),
            EndTime = new TimeSpan(12,0,0)
        },
        new Session()
        {
            Id = 3,
            ClassScheduleId = 1,
            SessionTypeId = 1,
            RoomId = 1,
            Title = "Session 3",
            LearningDate = new DateTime(2022, 01, 07),
            StartTime = new TimeSpan(8,0,0),
            EndTime = new TimeSpan(12,0,0)
        },
        new Session()
        {
            Id = 4,
            ClassScheduleId = 1,
            SessionTypeId = 1,
            RoomId = 1,
            Title = "Session 4",
            LearningDate = new DateTime(2022, 01, 10),
            StartTime = new TimeSpan(8,0,0),
            EndTime = new TimeSpan(12,0,0)
        },
        new Session()
        {
            Id = 5,
            ClassScheduleId = 1,
            SessionTypeId = 1,
            RoomId = 1,
            Title = "Session 5",
            LearningDate = new DateTime(2022, 01, 12),
            StartTime = new TimeSpan(8,0,0),
            EndTime = new TimeSpan(12,0,0)
        },
        new Session()
        {
            Id = 6,
            ClassScheduleId = 2,
            SessionTypeId = 1,
            RoomId = 1,
            Title = "Session 1",
            LearningDate = DateTime.Today.AddYears(2).AddDays(1),
            StartTime = new TimeSpan(8,0,0),
            EndTime = new TimeSpan(12,0,0)
        },
        new Session()
        {
            Id = 7,
            ClassScheduleId = 2,
            SessionTypeId = 1,
            RoomId = 1,
            Title = "Session 2",
            LearningDate = DateTime.Today.AddYears(2).AddDays(2),
            StartTime = new TimeSpan(8,0,0),
            EndTime = new TimeSpan(12,0,0)
        },
        new Session()
        {
            Id = 8,
            ClassScheduleId = 2,
            SessionTypeId = 1,
            RoomId = 1,
            Title = "Session 3",
            LearningDate = DateTime.Today.AddYears(2).AddDays(3),
            StartTime = new TimeSpan(8,0,0),
            EndTime = new TimeSpan(12,0,0)
        },
        new Session()
        {
            Id = 9,
            ClassScheduleId = 2,
            SessionTypeId = 1,
            RoomId = 1,
            Title = "Session 4",
            LearningDate = DateTime.Today.AddYears(2).AddDays(4),
            StartTime = new TimeSpan(8,0,0),
            EndTime = new TimeSpan(12,0,0)
        },
        new Session()
        {
            Id = 10,
            ClassScheduleId = 2,
            SessionTypeId = 1,
            RoomId = 1,
            Title = "Session 5",
            LearningDate = DateTime.Today.AddYears(2).AddDays(5),
            StartTime = new TimeSpan(8,0,0),
            EndTime = new TimeSpan(12,0,0)
        },
        new Session()
        {
            Id = 11,
            ClassScheduleId = 3,
            SessionTypeId = 1,
            RoomId = 1,
            Title = "Session 1",
            LearningDate = DateTime.Today.AddYears(3).AddDays(1),
            StartTime = new TimeSpan(8,0,0),
            EndTime = new TimeSpan(12,0,0)
        },
        new Session()
        {
            Id = 12,
            ClassScheduleId = 3,
            SessionTypeId = 1,
            RoomId = 1,
            Title = "Session 2",
            LearningDate = DateTime.Today.AddYears(3).AddDays(2),
            StartTime = new TimeSpan(8,0,0),
            EndTime = new TimeSpan(12,0,0)
        },
        new Session()
        {
            Id = 13,
            ClassScheduleId = 3,
            SessionTypeId = 1,
            RoomId = 1,
            Title = "Session 3",
            LearningDate = DateTime.Today.AddYears(3).AddDays(3),
            StartTime = new TimeSpan(8,0,0),
            EndTime = new TimeSpan(12,0,0)
        },
        new Session()
        {
            Id = 14,
            ClassScheduleId = 3,
            SessionTypeId = 1,
            RoomId = 1,
            Title = "Session 4",
            LearningDate = DateTime.Today.AddYears(3).AddDays(4),
            StartTime = new TimeSpan(8,0,0),
            EndTime = new TimeSpan(12,0,0)
        },
        new Session()
        {
            Id = 15,
            ClassScheduleId = 3,
            SessionTypeId = 1,
            RoomId = 1,
            Title = "Session 5",
            LearningDate = DateTime.Today.AddYears(3).AddDays(5),
            StartTime = new TimeSpan(8,0,0),
            EndTime = new TimeSpan(12,0,0)
        },
    };
}