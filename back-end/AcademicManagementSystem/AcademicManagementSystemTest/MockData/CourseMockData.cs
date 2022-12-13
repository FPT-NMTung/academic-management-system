using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class CourseMockData
{
    public static readonly List<Course> Courses = new List<Course>()
    {
        new Course()
        {
            Code = "COURSE CODE 1",
            CourseFamilyCode = "COURSE FAMILY CODE 1",
            Name = "Course 1",
            SemesterCount = 2,
            IsActive = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Course()
        {
            Code = "COURSE CODE 2",
            CourseFamilyCode = "COURSE FAMILY CODE 2",
            Name = "Course 2",
            SemesterCount = 3,
            IsActive = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Course()
        {
            Code = "COURSE CODE 555",
            CourseFamilyCode = "COURSE FAMILY CODE 2",
            Name = "Course 555",
            SemesterCount = 3,
            IsActive = false,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        }
    };
}