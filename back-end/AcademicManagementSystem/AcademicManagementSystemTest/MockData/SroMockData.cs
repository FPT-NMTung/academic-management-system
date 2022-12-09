using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class SroMockData
{
    public static readonly List<Sro> Sros = new List<Sro>()
    {
        new Sro()
        {
            UserId = 3,
        },
        new Sro()
        {
            UserId = 10,
        },
    };
}