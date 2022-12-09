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
        },
        new CourseModuleSemester()
        {
            CourseCode = "COURSE CODE 1",
            ModuleId = 3,
            SemesterId = 1
        },
        new CourseModuleSemester()
        {
            CourseCode = "COURSE CODE 1",
            ModuleId = 4,
            SemesterId = 1
        },
        new CourseModuleSemester()
        {
            CourseCode = "COURSE CODE 1",
            ModuleId = 5,
            SemesterId = 1
        }
    };
}