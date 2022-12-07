using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class CourseModuleSemesterMockData
{
    public static readonly List<CourseModuleSemester> CoursesModulesSemesters = new List<CourseModuleSemester>()
    {
        new CourseModuleSemester()
        {
            CourseCode = "COURSE CODE 1",
            ModuleId = 1,
            SemesterId = 1
        },
        new CourseModuleSemester()
        {
            CourseCode = "COURSE CODE 1",
            ModuleId = 2,
            SemesterId = 1
        }
    };
}