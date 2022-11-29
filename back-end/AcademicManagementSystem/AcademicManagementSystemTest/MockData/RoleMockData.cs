using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class RoleMockData
{
    public static List<Role> Roles = new List<Role>
    {
        new Role()
        {
            Id = 1,
            Value = "admin"
        },
        new Role()
        {
            Id = 2,
            Value = "sro"
        },
        new Role()
        {
            Id = 3,
            Value = "teacher"
        },
        new Role()
        {
            Id = 4,
            Value = "student"
        }
    };
}