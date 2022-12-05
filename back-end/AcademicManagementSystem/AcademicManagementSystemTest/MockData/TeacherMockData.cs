using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class TeacherMockData
{
    public static readonly List<Teacher> Teachers = new List<Teacher>
    {
        new Teacher()
        {
            UserId = 4,
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "0000000001"
        },
        new Teacher()
        {
            UserId = 5,
            TeacherTypeId = 1,
            WorkingTimeId = 2,
            Nickname = null,
            CompanyAddress = null,
            StartWorkingDate = DateTime.Today,
            Salary = 1000,
            TaxCode = "0000000002"
        }
    };
}