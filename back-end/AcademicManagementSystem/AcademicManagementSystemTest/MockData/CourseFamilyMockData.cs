using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class CourseFamilyMockData
{
    public static readonly List<CourseFamily> CourseFamilies = new List<CourseFamily>()
    {
        new CourseFamily()
        {
            Code = "COURSE FAMILY CODE 1",
            Name = "COURSE FAMILY 1",
            PublishedYear = 2020,
            IsActive = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new CourseFamily()
        {
            Code = "COURSE FAMILY CODE 2",
            Name = "COURSE FAMILY 2",
            PublishedYear = DateTime.Today.Year + 1,  
            IsActive = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new CourseFamily()
        {
            Code = "COURSE FAMILY CODE 3",
            Name = "COURSE FAMILY 3",
            PublishedYear = 2010,  
            IsActive = false,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        }
    };
}