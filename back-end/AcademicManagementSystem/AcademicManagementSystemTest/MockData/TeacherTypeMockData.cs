using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class TeacherTypeMockData
{
    public static readonly List<TeacherType> TeacherTypes = new List<TeacherType>()
    {
        new TeacherType()
        {
            Id = 1,
            Value = "Full time"
        },
        new TeacherType()
        {
            Id = 2,
            Value = "Part time"
        }
    };
}