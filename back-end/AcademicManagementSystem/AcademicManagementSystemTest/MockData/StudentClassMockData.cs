using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class StudentClassMockData
{
    public static readonly List<StudentClass> StudentsClasses = new List<StudentClass>()
    {
        new StudentClass()
        {
            StudentId = 6,
            ClassId = 1,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = true
        },
        new StudentClass()
        {
            StudentId = 6,
            ClassId = 3,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = false // not in class anymore
        },
        new StudentClass()
        {
            StudentId = 7,
            ClassId = 2,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = true
        },
        new StudentClass()
        {
            StudentId = 8,
            ClassId = 1,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = true
        },
        new StudentClass()
        {
            StudentId = 100,
            ClassId = 100,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = true
        },
        new StudentClass()
        {
            StudentId = 101,
            ClassId = 100,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = false
        },
        new StudentClass()
        {
            StudentId = 102,
            ClassId = 100,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = true
        },
        new StudentClass()
        {
            StudentId = 103,
            ClassId = 100,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = true
        },
    };
}