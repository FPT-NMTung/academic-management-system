using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class ClassDayMockData
{
    public static readonly List<ClassDays> ClassDays = new List<ClassDays>()
    {
        new ClassDays()
        {
            Id = 1,
            Value = "2_4_6"
        },
        new ClassDays()
        {
            Id = 2,
            Value = "3_5_7"
        }
    };
}