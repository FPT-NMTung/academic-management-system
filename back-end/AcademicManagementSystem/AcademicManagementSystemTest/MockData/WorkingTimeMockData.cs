using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class WorkingTimeMockData
{
    public static readonly List<WorkingTime> WorkingTimes = new List<WorkingTime>()
    {
        new WorkingTime()
        {
            Id = 1,
            Value = "Morning",
        },
        new WorkingTime()
        {
            Id = 2,
            Value = "Afternoon",
        },
        new WorkingTime()
        {
            Id = 3,
            Value = "Evening",
        },
        new WorkingTime()
        {
            Id = 4,
            Value = "Morning + Afternoon",
        },
        new WorkingTime()
        {
            Id = 5,
            Value = "Morning + Evening",
        },
        new WorkingTime()
        {
            Id = 6,
            Value = "Afternoon + Evening",
        },
        new WorkingTime()
        {
            Id = 7,
            Value = "All day",
        }
    };
}