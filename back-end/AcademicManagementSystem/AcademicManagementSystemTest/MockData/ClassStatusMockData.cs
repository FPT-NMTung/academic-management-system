using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class ClassStatusMockData
{
    public static readonly List<ClassStatus> ClassStatuses = new List<ClassStatus>()
    {
        new ClassStatus()
        {
            Id = 1,
            Value = "Scheduled"
        },
        new ClassStatus()
        {
            Id = 2,
            Value = "Learning"
        },
        new ClassStatus()
        {
            Id = 3,
            Value = "Completed"
        },
        new ClassStatus()
        {
            Id = 4,
            Value = "Cancel"
        },
        new ClassStatus()
        {
            Id = 5,
            Value = "Not Scheduled"
        },
        new ClassStatus()
        {
            Id = 6,
            Value = "Merged"
        }
    };
}